using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.Windows.Forms;

public sealed class Entry {
    public string file, original_sha256, patched_sha256, patch, patch_sha256;
    public long patched_length;
}
public sealed class Manifest { public string game, game_version, game_build, patch_version; public Entry[] files; }
public sealed class Plan { public string Dest, Saved, Stage, Rollback, Expected; public bool Applied; }
public static class Installer {
    static readonly string[] Allowed={"Sol One_Data/level0","Sol One_Data/level1","Sol One_Data/resources.assets","Sol One_Data/sharedassets0.assets","Sol One_Data/sharedassets1.assets","Sol One_Data/il2cpp_data/Metadata/global-metadata.dat"};
    static string Hash(string path) {
        using(var f=File.OpenRead(path)) using(var h=SHA256.Create())
            return BitConverter.ToString(h.ComputeHash(f)).Replace("-","").ToLowerInvariant();
    }
    static string Inside(string root,string relative) {
        string path=Path.GetFullPath(Path.Combine(root,relative.Replace('/',Path.DirectorySeparatorChar)));
        if(!path.StartsWith(root.TrimEnd(Path.DirectorySeparatorChar)+Path.DirectorySeparatorChar,StringComparison.OrdinalIgnoreCase)) throw new Exception("Geçersiz dosya yolu.");
        string at=path;
        while(at!=null && at.Length>=root.Length) {
            if((File.Exists(at)||Directory.Exists(at)) && (File.GetAttributes(at)&FileAttributes.ReparsePoint)!=0) throw new Exception("Oyun içindeki bağlantılı klasörler desteklenmiyor: "+relative);
            at=Path.GetDirectoryName(at);
        }
        return path;
    }
    static void CopyBytes(Stream from,Stream to,long length) {
        var buffer=new byte[1024*1024];
        while(length>0) {
            int n=from.Read(buffer,0,(int)Math.Min(buffer.Length,length));
            if(n==0) throw new Exception("Eksik yama verisi.");
            to.Write(buffer,0,n);length-=n;
        }
    }
    static void Decode(string original,string patch,string target,long expectedLength) {
        using(var source=File.OpenRead(original)) using(var r=new BinaryReader(File.OpenRead(patch)))
        using(var output=new FileStream(target,FileMode.CreateNew,FileAccess.Write,FileShare.None)) {
            if(Encoding.ASCII.GetString(r.ReadBytes(8))!="SOLTRP1\0") throw new Exception("Geçersiz yama başlığı.");
            long expected=r.ReadInt64();
            if(expected!=expectedLength || expected<=0 || expected>2L*1024*1024*1024) throw new Exception("Geçersiz çıktı boyutu.");
            while(true) {
                byte op=r.ReadByte();if(op==255)break;
                if(op==0) {
                    long offset=r.ReadInt64();int length=r.ReadInt32();
                    if(offset<0||length<0||offset>source.Length-length||length>expected-output.Position)throw new Exception("Geçersiz kopyalama aralığı.");
                    source.Position=offset;CopyBytes(source,output,length);
                } else if(op==1) {
                    int length=r.ReadInt32();
                    if(length<0||length>r.BaseStream.Length-r.BaseStream.Position||length>expected-output.Position)throw new Exception("Geçersiz ekleme aralığı.");
                    CopyBytes(r.BaseStream,output,length);
                } else throw new Exception("Geçersiz yama komutu.");
            }
            if(output.Length!=expected||r.BaseStream.Position!=r.BaseStream.Length)throw new Exception("Yama boyutu uyuşmuyor.");
            output.Flush(true);
        }
    }
    [STAThread]
    public static int Main(string[] args) {
        bool gui=args.Length==0;string stageRoot=null;
        var plans=new List<Plan>();
        try {
            bool restore=false,verify=false;string root=null;
            for(int i=0;i<args.Length;i++) {
                if(args[i]=="--restore")restore=true;
                else if(args[i]=="--verify")verify=true;
                else if(args[i]=="--game-dir"&&i+1<args.Length)root=args[++i];
                else throw new Exception("Kullanım: Kur.exe --game-dir OYUN_KLASÖRÜ [--restore] [--verify]");
            }
            if(gui) {
                Application.EnableVisualStyles();
                using(var dialog=new FolderBrowserDialog()) {
                    dialog.Description="Sol One.exe dosyasının bulunduğu oyun klasörünü seçin (Sol One_Data klasörünün kendisini seçmeyin).";
                    dialog.ShowNewFolderButton=false;
                    if(dialog.ShowDialog()!=DialogResult.OK)return 0;root=dialog.SelectedPath;
                }
                var choice=MessageBox.Show("Evet: Türkçe yamayı kur\nHayır: Özgün dosyaları yerel yedekten geri yükle\nİptal: Çık","Sol One Türkçe Yama v1.0.0",MessageBoxButtons.YesNoCancel);
                if(choice==DialogResult.Cancel)return 0;restore=choice==DialogResult.No;
            }
            if(String.IsNullOrWhiteSpace(root))throw new Exception("Oyun klasörü gerekli.");
            root=Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar);
            if(!Directory.Exists(Path.Combine(root,"Sol One_Data")))throw new Exception("Sol One_Data bulunamadı. Sol One.exe dosyasının bulunduğu klasörü seçin.");
            if(!verify&&Process.GetProcessesByName("Sol One").Length>0)throw new Exception("Önce Sol One oyununu kapatın.");
            string package=AppDomain.CurrentDomain.BaseDirectory;
            var manifest=new JavaScriptSerializer().Deserialize<Manifest>(File.ReadAllText(Path.Combine(package,"manifest.json"),Encoding.UTF8));
            if(manifest.game!="Sol One"||manifest.game_version!="0.1.5EA"||manifest.files==null||manifest.files.Length!=6||manifest.files.Select(e=>e.file).Distinct().Count()!=6)throw new Exception("Geçersiz manifest.");
            string backup=Inside(root,"SolOne_TurkceYama_OrijinalYedek_0.1.5EA_v1");
            // Validate the entire installation and all payloads before staging or mutation.
            foreach(var e in manifest.files) {
                if(!Allowed.Contains(e.file)||!System.Text.RegularExpressions.Regex.IsMatch(e.patch,"^patch-[1-6]\\.soltr$"))throw new Exception("Geçersiz manifest yolu.");
                string dest=Inside(root,e.file);string saved=Inside(backup,e.file);string current=Hash(dest);
                if(current!=e.original_sha256&&current!=e.patched_sha256)throw new Exception("Uyumsuz veya değiştirilmiş oyun dosyası. Hiçbir dosya değiştirilmedi: "+e.file);
                if(File.Exists(saved)&&Hash(saved)!=e.original_sha256)throw new Exception("Mevcut yedek bu sürüme ait değil; üzerine yazılmadı: "+e.file);
                if(restore&&current==e.patched_sha256&&(!File.Exists(saved)||Hash(saved)!=e.original_sha256))throw new Exception("Temiz yerel yedek yok: "+e.file);
                if(!restore&&Hash(Path.Combine(package,e.patch))!=e.patch_sha256)throw new Exception("Yama dosyası bozuk: "+e.patch);
                if((restore&&current==e.original_sha256)||(!restore&&current==e.patched_sha256))continue;
                plans.Add(new Plan{Dest=dest,Saved=saved,Expected=restore?e.original_sha256:e.patched_sha256});
            }
            if(plans.Count==0) { Show(gui,restore?"Oyun dosyaları zaten özgün durumda.":"Türkçe yama zaten kurulu.");return 0; }
            stageRoot=Inside(root,".SolOneTurkceYama_islem_"+Guid.NewGuid().ToString("N"));Directory.CreateDirectory(stageRoot);
            for(int i=0;i<plans.Count;i++) {
                var plan=plans[i];plan.Stage=Path.Combine(stageRoot,i+".new");plan.Rollback=Path.Combine(stageRoot,i+".old");
                var entry=manifest.files.Single(e=>Inside(root,e.file)==plan.Dest);
                if(restore)File.Copy(plan.Saved,plan.Stage);
                else Decode(plan.Dest,Path.Combine(package,entry.patch),plan.Stage,entry.patched_length);
                if(Hash(plan.Stage)!=plan.Expected)throw new Exception("Üretilen dosyanın SHA-256 değeri uyuşmuyor: "+entry.file);
            }
            if(verify){Show(gui,"Doğrulama başarılı. Oyun dosyalarına yazılmadı.");return 0;}
            if(!restore)foreach(var plan in plans) {
                Directory.CreateDirectory(Path.GetDirectoryName(plan.Saved));
                if(!File.Exists(plan.Saved))File.Copy(plan.Dest,plan.Saved);
            }
            try {
                foreach(var plan in plans) {
                    File.Replace(plan.Stage,plan.Dest,plan.Rollback);plan.Applied=true;
                    if(Hash(plan.Dest)!=plan.Expected)throw new Exception("Yazma doğrulaması başarısız.");
                }
            } catch {
                foreach(var plan in plans.Where(p=>p.Applied))File.Copy(plan.Rollback,plan.Dest,true);
                throw;
            }
            Show(gui,restore?"Özgün oyun dosyaları geri yüklendi. Yedekler korundu.":"Türkçe yama kuruldu. Oyunu normal şekilde açabilirsiniz. Özgün dosyalar oyun klasöründeki yedek dizininde korundu.");
            return 0;
        } catch(Exception ex) {
            Console.Error.WriteLine(ex.Message);
            if(gui)MessageBox.Show(ex.Message,"Yama uygulanamadı",MessageBoxButtons.OK,MessageBoxIcon.Error);
            return 1;
        } finally {
            // Only the unique temporary directory created by this run is removed.
            if(stageRoot!=null&&Directory.Exists(stageRoot)) {
                try{Directory.Delete(stageRoot,true);}catch{}
            }
        }
    }
    static void Show(bool gui,string text){Console.WriteLine(text);if(gui)MessageBox.Show(text,"Sol One Türkçe Yama");}
}
