# AirportAutomation (MAUI)

Çift platform (Android + Windows) terminal otomasyon demosu. Uçuş, kapı, check-in, rezervasyon ve AI asistan modülleri içerir. Rol bazlı görünürlük ve yeni eklenen Home sekmesiyle basit bir giriş deneyimi sunar.

## Mimari
- `AirportAutomation.Core`: Entity tanımları, `AirportDbContext`, seed (`DbSeeder`).
- `AirportAutomation.Interface`: Servis arayüzleri.
- `AirportAutomation.Service`: Servis implementasyonları (Flight, Passenger, Gate, Auth, Booking, Assistant).
- `AirportAutomation.UI`: .NET MAUI istemcisi (Android, Windows). DI, sayfalar, Shell, rol bazlı sekmeler, AI istemcisi.

## Katmanlar (Frontend / Backend / Entegrasyon)
- **Frontend (UI)**: `AirportAutomation.UI` (.NET MAUI). Sekmeler, sayfalar, rol bazlı görünürlük, ikonlar, AI istemcisi. Platformlar: Android, Windows.
- **Backend (Uygulama katmanı)**: `AirportAutomation.Service` (iş mantığı), `AirportAutomation.Interface` (sözleşmeler), `AirportAutomation.Core` (entity, context, seed). EF Core + SQLite.
- **Entegrasyon**:
  - AI: `Services/AiChatService.cs` (Gemini endpoint’leri, API key `Api:Key`).
  - Google OAuth (demo): `Services/GoogleAuthService.cs` (ClientId konfigürasyonu gerekiyor).
  - NuGet feed: `NuGet.Config` (nuget.org + opsiyonel DevExpress 25.2 offline).
  - MAUI asset’leri: ikon/splash/font/resimler.

## Özellikler
- Home sekmesi: Hızlı kısayollar (Flights, Gates, Passenger/Check-In, Map, AI).
- Uçuş listesi: `MainPage`.
- Kapı yönetimi: Yalnızca Staff/Admin sekmesi görünür; Passenger gizli. Sayfa içinde de rol kontrolü var.
- Yolcu işlemleri: PNR ile sorgu, check-in; Admin tüm yolcuları görebilir (liste).
- Rezervasyon, AI asistan sekmeleri.
- Rol bazlı sekme gizleme (`AppShell.xaml.cs`): Admin dışındakiler Admin sekmesini, Staff/Admin dışındakiler Gate sekmesini göremez.
- İkonlar: `Resources/Images` (flight, gate, passenger, assistant, booking, admin, map, home).

## Gereksinimler
- .NET 8 SDK
- MAUI workload'ları:
  - Windows için: `dotnet workload install maui-windows`
  - Android için: `dotnet workload install maui-android` (Android SDK/NDK/platform-tools gerekir)
- Git
- (Opsiyonel) DevExpress 25.2 offline paketleri: `NuGet.Config` içinde `C:\Program Files\DevExpress 25.2\Components\Offline Packages` kaynağı tanımlı. Bu kaynak yoksa ya kurun ya da `NuGet.Config`ten kaldırın.

## Yapılandırma
- AI anahtarı: `MauiProgram.cs` içinde `Api:Key` örnek bir Gemini API anahtarıyla belleğe eklenmiş. Gerçek anahtarınızı buraya veya güvenli bir gizli yönetime taşıyın.
- Google OAuth (demo): `Services/GoogleAuthService.cs` içinde `ClientIdAndroid` ve `ClientIdIos` için kendi client ID'lerinizi girmeniz gerekir; aksi halde buton sadece demo mesajı döner.

## Derleme ve Çalıştırma
### Ortak adımlar
```bash
dotnet restore --configfile NuGet.Config
```

### Windows (masaüstü)
```bash
dotnet build AirportAutomation.UI/AirportAutomation.UI.csproj -f net8.0-windows10.0.19041.0
dotnet run --project AirportAutomation.UI/AirportAutomation.UI.csproj -f net8.0-windows10.0.19041.0 --no-restore
```

### Android (cihaz/emülatör)
```bash
dotnet build AirportAutomation.UI/AirportAutomation.UI.csproj -f net8.0-android
```
Publish (APK, AOT kapalı):
```bash
dotnet publish AirportAutomation.UI/AirportAutomation.UI.csproj -f net8.0-android -c Release --no-restore /p:RunAOTCompilation=false /p:AndroidPackageFormat=apk
```
Oluşan APK: `AirportAutomation.UI/bin/Release/net8.0-android/publish/com.companyname.airportautomation.ui-Signed.apk`
Yükleme:
```bash
adb install -r "AirportAutomation.UI/bin/Release/net8.0-android/publish/com.companyname.airportautomation.ui-Signed.apk"
```

## Roller ve yetkiler
- Passenger: Uçuş ve check-in ekranlarına erişir; Gate/Admin sekmeleri görünmez.
- Staff: Gate sekmesine erişir, Admin sekmesi gizli.
- Admin: Tüm sekmeler; Passenger sayfasında tüm yolcular listelenir.

## Veri ve seed
- `DbSeeder.Seed` açılışta çalışır; veritabanı yerel SQLite (varsayılan MAUI dosya yolu). Demo verileri yükler.

## Notlar
- `.gitignore` bin/obj/publish ve platform çıktıları için güncellendi.
- Android publish'te AOT kapalı tutuldu; ihtiyaca göre AOT açılabilir.
- DevExpress 25.2 feed'i yoksa `NuGet.Config` içindeki ilgili satırı kaldırın veya doğru yolu ayarlayın.

## Lisans
Bu repo için lisans belirtilmemiştir; gerekirse ekleyin.
