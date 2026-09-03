# Liman Gemi Takip Sistemi (Port Tracking System)

Bu proje, limana gelen ve limandan ayrılan gemilerin, taşıdıkları yüklerin ve mürettebat bilgilerinin takibini sağlayan tam yığın (full-stack) bir web uygulamasıdır.

## 🛠️ Kullanılan Teknolojiler

- **Backend:** ASP.NET Core 8 Web API, Entity Framework Core, SQL Server
- **Frontend:** React, Vite, Axios
- **Test:** xUnit, Moq

## 🚀 Projeyi Çalıştırma Rehberi

Projeyi yerel bilgisayarınızda (lokal) çalıştırmak için aşağıdaki adımları sırasıyla izleyin.

### Ön Koşullar (Gereksinimler)
Bilgisayarınızda şunların kurulu olduğundan emin olun:
1. [.NET 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
2. [Node.js](https://nodejs.org/) (Frontend paketleri için)
3. SQL Server Express (Eğer farklı bir SQL versiyonu kullanıyorsanız `appsettings.json` dosyasındaki `Server=` kısmını güncellemeniz gerekir).

---

### Adım 1: Veritabanını Hazırlama ve Backend'i Çalıştırma

Backend projesini ayağa kaldırmak ve veritabanını oluşturmak için bir terminal (Komut İstemcisi veya PowerShell) açın:

1. **API klasörüne gidin:**
   ```bash
   cd PortTrackingSystem.API
   ```

2. **Veritabanını oluşturun (Migration işlemi):**
   *(Eğer Visual Studio kullanıyorsanız Package Manager Console üzerinden `Update-Database` komutunu da çalıştırabilirsiniz)*
   ```bash
   dotnet ef database update
   ```
   *Not: Bu komut, `appsettings.json` dosyasındaki bilgilere göre SQL Server üzerinde `PortTrackingDB` adında bir veritabanı oluşturacaktır.*

3. **Backend projesini çalıştırın:**
   ```bash
   dotnet run
   ```
   *Proje çalıştığında Swagger dokümantasyonuna genellikle `https://localhost:7176/swagger` veya `http://localhost:5184/swagger` adresinden erişebilirsiniz.*

---

### Adım 2: Frontend'i (React) Çalıştırma

Backend çalışır durumdayken, frontend tarafını çalıştırmak için **yeni bir terminal penceresi** açın:

1. **Frontend klasörüne gidin:**
   ```bash
   cd frontend
   ```

2. **Gerekli paketleri indirin:**
   *(Bu işlem `node_modules` klasörünü oluşturacaktır ve sadece ilk seferde yapılması yeterlidir)*
   ```bash
   npm install
   ```

3. **React uygulamasını başlatın:**
   ```bash
   npm run dev
   ```

4. **Uygulamayı Görüntüleyin:**
   Terminalde beliren adrese (genellikle `http://localhost:5173/`) tıklayarak veya tarayıcınıza kopyalayarak projeyi kullanmaya başlayabilirsiniz.
