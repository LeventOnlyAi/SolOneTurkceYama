# Sol One Türkçe Yama

Sol One için Levent Parlak Gaming tarafından paylaşılan ücretsiz, resmî olmayan Türkçe yama. Yapay zekâ desteğiyle hazırlanmış; uzun rehber metinleri, oyun terimleri ve Türkçe karakterler ayrıca düzenlenmiştir.

**Desteklenen oyun: 0.1.5EA — 17 Kasım 2026 derlemesi (`1d72058507e6e226`).**

**Yama sürümü: 1.0.0 · Windows 64 bit.**

## İndir

[**Kuruluma hazır güncel yamayı indir → Releases**](https://github.com/LeventOnlyAi/SolOneTurkceYama/releases/latest)

Releases sayfasındaki **`SolOne_TurkceYama_0.1.5EA_v1.0.0.zip`** dosyasını indirin. GitHub’ın otomatik eklediği “Source code” arşivleri yerine bu ZIP’i kullanın.

## Kurulum — adım adım

1. **Oyunu tamamen kapatın.**
2. İndirdiğiniz ZIP’e sağ tıklayıp **Tümünü Ayıkla** seçeneğini kullanın. ZIP’in içinden doğrudan çalıştırmayın; `Kur.exe`, `manifest.json` ve altı `patch-*.soltr` dosyası aynı klasörde bulunmalı.
3. Çıkardığınız klasördeki **`Kur.exe`** dosyasını açın. Ek Python, Unity aracı veya yönetici izni gerekmez. Bilgisayarınızdaki güvenlik korumalarını kapatmayın.
4. **`Sol One.exe` dosyasının bulunduğu oyun klasörünü seçin.** `Sol One_Data` klasörünün kendisini seçmeyin.
5. Kurulum penceresinde **Evet** seçeneğine basın. **Hayır**, kurulum için değil, yamayı kaldırmak içindir.
6. “Türkçe yama kuruldu” mesajını gördüğünüzde oyunu normal şekilde açın. Ayrı bir dil seçmeniz gerekmez.

Doğru oyun klasörünün görünümü:

```text
Sol One oyun klasörü/        ← seçilecek klasör
├── Sol One.exe
├── GameAssembly.dll
└── Sol One_Data/
```

Steam kullanıyorsanız: **Kütüphane → Sol One → dişli simgesi → Yönet → Yerel dosyalara göz at.** Açılan klasörü kurucuda seçin. Uyumluluk yalnız sürüm yazısına göre değil, altı dosyanın SHA-256 değerine göre denetlenir; aynı sürüm numarasına sahip farklı bir derleme de reddedilebilir.

## Neler Türkçe?

- Ana menü, yeni keşif, kayıt/yükleme ve ayar ekranları.
- Eşya, yapı, makine ve araştırma açıklamaları.
- Eğitim hedefleri ve oyun içi göstergeler.
- 28 konuluk Saha Rehberi; uzun açıklamalar elle düzenlendi.
- Sekiz yazı tipinde eksik Türkçe harfler tamamlandı.

Yerel yama çalışmasında **1.159 farklı metin** işlendi; yinelenen kullanımlar dahil altı dosyada **2.613 metin değişimi** yapıldı. Bu sayı oyundaki her olası ekranın baştan sona oynanarak doğrulandığı anlamına gelmez.

**Kapsam sınırı:** Seslendirme ile rehberin içine gömülü özgün örnek ekran görüntüleri İngilizce kalır. Motor ve komut adlarıyla ortak kullanılan bazı dinamik tek sözcüklü etiketler de İngilizce görünebilir. Ana menü, yeni keşif, rehberde eşya/kısayol gösterimi, açılış ve oyun içi HUD kontrol edildi; tam oyun bitirilerek test edilmedi.

## Yedekleme ve kaldırma

Kurucu altı özgün dosyayı oyun klasöründeki şu dizine yedekler:

```text
SolOne_TurkceYama_OrijinalYedek_0.1.5EA_v1/
```

**Bu klasörü silmeyin.** Tekrar kurulumda temiz yedeğin üzerine yazılmaz. Kayıt dosyaları yamalanmaz.

Yamayı kaldırmak için:

1. Oyunu kapatın.
2. Aynı yama paketindeki `Kur.exe` dosyasını açın ve aynı oyun klasörünü seçin.
3. **Hayır** seçeneğine basın. Kurucu doğrulanmış yerel yedekten özgün dosyaları geri yükler; yedeği korur.

Yedek kayıpsa Steam’in **Yüklü Dosyalar → Oyun dosyalarının bütünlüğünü doğrula** özelliği özgün dosyaları yeniden indirebilir.

## Hata alırsanız

| Mesaj / durum | Yapılacak işlem |
|---|---|
| `Sol One_Data bulunamadı` | `Sol One.exe` dosyasının bulunduğu üst klasörü seçin. |
| `Önce Sol One oyununu kapatın` | Oyundan çıkın; arka planda çalışmadığını kontrol edin. |
| `Uyumsuz veya değiştirilmiş oyun dosyası` | Yama yalnız belirtilen derlemeye uygundur. Farklı sürüme zorla uygulamayın. Başka modları kaldırıp özgün dosyaları doğrulayın veya uyumlu yama sürümünü bekleyin. |
| `Yama dosyası bozuk` | Kuruluma hazır ZIP’i yeniden indirip tüm dosyaları birlikte çıkarın. |
| `Mevcut yedek bu sürüme ait değil` | Yedeği koruyun. Başka sürümün yedeğini yeni oyunun üzerine geri yüklemeyin. |
| Windows uygulamayı çalıştırmıyor | Güvenlik korumalarını kapatmayın. SHA-256 değerini kontrol edin; kurucu kaynak kodu bu depoda incelenebilir. |

Oyun güncellenince yama kaldırılabilir. **Eski yedekleri yeni oyun sürümünün üzerine yüklemeyin.** Yeni sürüm için hazırlanmış yamayı kullanın.

## Komut satırı ve kaynak kodu

İsteğe bağlı kullanım:

```powershell
.\Kur.exe --game-dir "D:\Oyunlar\Sol One" --verify
.\Kur.exe --game-dir "D:\Oyunlar\Sol One"
.\Kur.exe --game-dir "D:\Oyunlar\Sol One" --restore
```

`--verify`, çıktıları geçici bir klasörde üretip doğrular; asıl oyun dosyalarını değiştirmez. Kurulum için diskte yedek ve geçici dosyalar adına **en az 4 GB boş alan** bulundurun.

Kurucu büyük dosyaları akış halinde işler. Tüm girdiler ve üretilen dosyalar SHA-256 ile kontrol edilir. Uyumsuz sürüm ön kontrolde reddedilir; yazma hatasında değiştirilen dosyalar geri alınır. Kurucu ağ bağlantısı kurmaz, kimlik bilgisi istemez ve güvenlik ayarı değiştirmez.

`Kur.cs` kaynak kodudur. Windows’un .NET Framework C# derleyicisiyle 64 bit olarak derlenebilir:

```powershell
& "$env:WINDIR\Microsoft.NET\Framework64\v4.0.30319\csc.exe" /nologo /target:winexe /platform:x64 /optimize+ /reference:System.Web.Extensions.dll /reference:System.Windows.Forms.dll /out:Kur.exe Kur.cs
```

Kurulum, tekrar kurulum, kaldırma, tekrar kaldırma, yazmadan doğrulama ve uyumsuz sürümün değişiklik yapılmadan reddedilmesi temiz bir test kopyasında denetlendi. Ayrıntı: [TEST_SONUCU.json](TEST_SONUCU.json).

Paket yalnız değişiklik verilerini ve kurucuyu içerir; tam oyun, oyun çalıştırılabilir dosyası, kişisel kayıt veya özgün oyun yedeği içermez. Sol One ve oyun içeriği hak sahiplerine aittir. Bu proje oyun geliştiricisiyle bağlantılı değildir. Eklenen Exo 2 karakterleri için [yazı tipi lisansı](EXO2_OFL.txt) dahildir.
