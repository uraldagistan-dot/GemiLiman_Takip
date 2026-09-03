using Moq;
using Xunit;
using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Repositories;
using PortTrackingSystem.Core.Services;

namespace PortTrackingSystem.Tests
{
    public class ShipVisitServiceTests
    {
        // Ziyaret servisi artık gemi ve limanın gerçekten var olduğunu da kontrol ediyor.
        // Tarih kuralını izole test edebilmek için bu iki dublörü "kayıt var" diyecek şekilde kuruyoruz.
        private static (Mock<IRepository<Ship>>, Mock<IRepository<Port>>) MockRelatedRepos()
        {
            var mockShipRepo = new Mock<IRepository<Ship>>();
            mockShipRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Ship { ShipId = 1 });

            var mockPortRepo = new Mock<IRepository<Port>>();
            mockPortRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Port { PortId = 1 });

            return (mockShipRepo, mockPortRepo);
        }

        // 1. SINIR DEĞER (Edge Case) TESTİ: Geliş tarihi ve ayrılış tarihi birebir EŞİT olursa
        [Fact]
        public async Task AddVisitAsync_ArrivalEqualsDeparture_ThrowsException()
        {
            // Arrange
            var mockRepo = new Mock<IRepository<ShipVisits>>(); // Entities klasöründeki ismine göre ShipVisits yazdım
            var (mockShipRepo, mockPortRepo) = MockRelatedRepos();
            var service = new ShipVisitService(mockRepo.Object, mockShipRepo.Object, mockPortRepo.Object);

            var visit = new ShipVisits
            {
                ShipId = 1,
                PortId = 1,
                Purpose = "Yükleme",
                ArrivalDate = new DateTime(2026, 1, 1, 12, 0, 0), // Aynı gün, aynı saat
                DepartureDate = new DateTime(2026, 1, 1, 12, 0, 0)
            };

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => service.AddVisitAsync(visit));
            Assert.Equal("Hata: Gemi geliş tarihi, ayrılış tarihinden daha önce olmalıdır!", ex.Message);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<ShipVisits>()), Times.Never);
        }

        // 2. KÖTÜ SENARYO: Geliş tarihi, ayrılış tarihinden BÜYÜK (sonra) olursa
        [Fact]
        public async Task AddVisitAsync_ArrivalAfterDeparture_ThrowsException()
        {
            var mockRepo = new Mock<IRepository<ShipVisits>>();
            var (mockShipRepo, mockPortRepo) = MockRelatedRepos();
            var service = new ShipVisitService(mockRepo.Object, mockShipRepo.Object, mockPortRepo.Object);

            var visit = new ShipVisits
            {
                ShipId = 1,
                PortId = 1,
                Purpose = "Bakım",
                ArrivalDate = new DateTime(2026, 1, 5), // Ayın 5'inde geldi
                DepartureDate = new DateTime(2026, 1, 1)  // Ayın 1'inde gitti (Mantıksız)
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => service.AddVisitAsync(visit));
            Assert.Equal("Hata: Gemi geliş tarihi, ayrılış tarihinden daha önce olmalıdır!", ex.Message);
        }

        // 3. İYİ SENARYO: Her şey kurallara uygun
        [Fact]
        public async Task AddVisitAsync_ArrivalBeforeDeparture_SavesSuccessfully()
        {
            var mockRepo = new Mock<IRepository<ShipVisits>>();
            var (mockShipRepo, mockPortRepo) = MockRelatedRepos();
            var service = new ShipVisitService(mockRepo.Object, mockShipRepo.Object, mockPortRepo.Object);

            var visit = new ShipVisits
            {
                ShipId = 1,
                PortId = 1,
                Purpose = "Yükleme",
                ArrivalDate = new DateTime(2026, 1, 1),
                DepartureDate = new DateTime(2026, 1, 5)
            };

            await service.AddVisitAsync(visit);
            mockRepo.Verify(r => r.AddAsync(visit), Times.Once); // Veritabanı kayıt komutu tam 1 kere çalışmalı
        }

        // 4. VERİ ÇEKME (Mock Setup) TESTİ: Sistemden liste istersek dublör doğru davranıyor mu?
        [Fact]
        public async Task GetAllVisitsAsync_ReturnsExpectedVisits()
        {
            // Arrange
            var mockRepo = new Mock<IRepository<ShipVisits>>();
            var (mockShipRepo, mockPortRepo) = MockRelatedRepos();

            // Sahte bir liste oluşturuyoruz
            var expectedVisits = new List<ShipVisits>
            {
                new ShipVisits { VisitId = 1, Purpose = "Yük İndirme" },
                new ShipVisits { VisitId = 2, Purpose = "Yakıt İkmali" }
            };

            // MOCK SETUP: Servis, gemi/liman adını da göstermek için ilişkili sorguyu kullanıyor.
            // Dublöre "Sana GetAllWithNestedIncludeAsync dendiğinde bizim sahte listeyi ver" diyoruz.
            mockRepo.Setup(r => r.GetAllWithNestedIncludeAsync(It.IsAny<string[]>()))
                    .ReturnsAsync(expectedVisits);

            var service = new ShipVisitService(mockRepo.Object, mockShipRepo.Object, mockPortRepo.Object);

            // Act
            var result = await service.GetAllVisitsAsync();

            // Assert
            Assert.NotNull(result); // Gelen veri boş(null) olmamalı
            Assert.Equal(2, result.Count()); // Listede tam 2 eleman olmalı
            Assert.Equal(expectedVisits, result); // Gelen liste bizim verdiğimiz listeyle birebir aynı olmalı
        }

        // 5. YENİ KURAL TESTİ: Sistemde olmayan bir liman seçilemez
        [Fact]
        public async Task AddVisitAsync_PortDoesNotExist_ThrowsException()
        {
            var mockRepo = new Mock<IRepository<ShipVisits>>();
            var (mockShipRepo, _) = MockRelatedRepos();

            var mockPortRepo = new Mock<IRepository<Port>>();
            mockPortRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Port)null!);

            var service = new ShipVisitService(mockRepo.Object, mockShipRepo.Object, mockPortRepo.Object);

            var visit = new ShipVisits
            {
                ShipId = 1,
                PortId = 999,
                Purpose = "Yükleme",
                ArrivalDate = new DateTime(2026, 1, 1),
                DepartureDate = new DateTime(2026, 1, 5)
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => service.AddVisitAsync(visit));
            Assert.Equal("Seçilen liman sistemde bulunamadı!", ex.Message);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<ShipVisits>()), Times.Never);
        }
    }
}
