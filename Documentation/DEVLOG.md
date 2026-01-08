# Geliştirme Günlüğü (AirportAutomationV3)

## Teknolojiler
- .NET 8
- .NET MAUI (Android, iOS, MacCatalyst, Windows)
- Entity Framework Core Sqlite
- Katmanlar: Core (Entities/DbContext/Seed/Helpers), Interface (I*), Service (S*), UI (MAUI)

## Adımlar
1. Yeni çözüm oluşturuldu: `AirportAutomationV3.sln`
2. Projeler eklendi: `AirportAutomation.Core`, `AirportAutomation.Interface`, `AirportAutomation.Service`, `AirportAutomation.UI (MAUI)`
3. Core:
   - Entity’ler: Flight, Passenger, Gate
   - DbContext: SQLite (LocalAppData/AirportAutomation/airport.db)
   - Seed: Sahte uçuş/yolcu üretimi, `DbSeeder.Seed`
   - Helper: `CommonFunction` (Report/Template path)
   - Simulation: `SimulationService.SimulateOnce` (rötar/kapı değişimi)
4. Interface:
   - `IFlightService`, `IPassengerService`, `IGateService`, `IAssistantService`, `IBookingService`
5. Service:
   - `SFlightService`, `SPassengerService`, `SGateService`, `SAssistantService`, `SBookingService` (try-catch, string hata, DbContext metod içinde)
6. UI (MAUI):
   - DI: DbContext, Flight/Passenger/Gate/Assistant/Booking servisleri
   - AppShell -> MainPage / GatePage / PassengerPage / AssistantPage / BookingPage / AdminPage
   - MainPage: Uçuş listesi, yenile
   - GatePage: Kapı listesi, yenile
   - PassengerPage: PNR arama, check-in (koltuk), uçuş bilgisi
   - AssistantPage: Basit AI demo (ChatLog kaydı)
   - BookingPage: Kaynak PNR’dan seçili uçuşa yeni bilet oluşturma
   - AdminPage: Uçuş/yolcu/ciro, popüler destinasyonlar
7. Simulation: `SimulationService.SimulateOnce` (rötar/kapı değişimi)

## Yapılacaklar
- UI: Gate güncelleme aksiyonu, kalan modüller (harita, admin vb.)
- Build/test: Uygun shell’de `dotnet restore/build AirportAutomationV3.sln`

## Kimlik / Roller (Prototip)
- Varsayılan hesaplar (seed): admin/admin123, personel1/personel123
- Yolcu: kayıt olabilir (kullanıcı adı/şifre), rol Passenger
- Personel/Admin: giriş kullanıcı adı/şifre ile
- Shell rol kısıtı: Passenger → Admin/Gate tab kapalı, Staff → Admin kapalı, Admin → hepsi açık
- Sayfa guard: AdminPage sadece Admin, GatePage Admin/Staff
- Google ile giriş: şu an demo, gerçek client ID eklenince aktifleştirilebilir (buton uyarı gösteriyor)

