using Moq;
using Xunit;
using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Repositories;
using PortTrackingSystem.Core.Services;

namespace PortTrackingSystem.Tests
{
    public class CargoServiceTests
    {
        // Yük servisi artık "gemi gerçekten var mı" kontrolü de yaptığı için
        // testlerde gemi deposunun dublörünü de hazırlamamız gerekiyor.
        private static Mock<IRepository<Ship>> MockShipRepoReturning(Ship? ship)
        {
            var mockShipRepo = new Mock<IRepository<Ship>>();
            mockShipRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(ship!);
            return mockShipRepo;
        }

        // 1. Senaryo: Ağırlık 0 veya daha küçükse sistem hata fırlatmalı (KÖTÜ SENARYO)
        [Fact]
        public async Task AddCargoAsync_WeightIsZeroOrLess_ThrowsException()
        {
            // Hazırlık
            // Veritabanına gerçekten bağlanmamak için Repository'nin bir kopyasını (dublörünü) oluşturuyoruz.
            var mockRepo = new Mock<IRepository<Cargoes>>();
            var mockShipRepo = MockShipRepoReturning(new Ship { ShipId = 1 });
            var service = new CargoService(mockRepo.Object, mockShipRepo.Object); // Aşçıya sahte bıçağı verdik

            var invalidCargo = new Cargoes { WeightTon = 0, Description = "Hatalı Yük", CargoType = "Konteyner", ShipId = 1 };

            // Act & Assert (Eylem ve Doğrulama)
            // Ağırlık 0 olduğu için bu metodun "Exception" (Hata) fırlatmasını BEKLİYORUZ.
            var exception = await Assert.ThrowsAsync<Exception>(() => service.AddCargoAsync(invalidCargo));

            // Fırlatılan hatanın mesajı, bizim yazdığımız mesajla birebir aynı mı?
            Assert.Equal("Hata: Yük ağırlığı (WeightTon) 0'dan büyük olmalıdır!", exception.Message);

            // Ayrıca, hata fırladığı için veritabanına kaydetme metodu (AddAsync) HİÇ ÇAĞRILMAMIŞ olmalı!
            mockRepo.Verify(r => r.AddAsync(It.IsAny<Cargoes>()), Times.Never);
        }

        // 2. Senaryo: Ağırlık 0'dan büyükse sistem başarıyla kaydetmeli (İYİ SENARYO)
        [Fact]
        public async Task AddCargoAsync_WeightIsGreaterThanZero_CallsRepositoryAddAsync()
        {
            // Arrange
            var mockRepo = new Mock<IRepository<Cargoes>>();
            var mockShipRepo = MockShipRepoReturning(new Ship { ShipId = 1 });
            var service = new CargoService(mockRepo.Object, mockShipRepo.Object);

            var validCargo = new Cargoes { WeightTon = 500, Description = "Geçerli Yük", CargoType = "Konteyner", ShipId = 1 };

            // Act
            // Bu sefer ağırlık 500, yani geçerli. Metodu normalce çalıştırıyoruz.
            await service.AddCargoAsync(validCargo);

            // Assert
            // Hata fırlamadığı için, veritabanına kayıt metodu (AddAsync) tam olarak 1 KERE (Times.Once) çağrılmış olmalı!
            mockRepo.Verify(r => r.AddAsync(validCargo), Times.Once);
        }

        // 3. Senaryo: Yük, sistemde olmayan bir gemiye eklenemez
        [Fact]
        public async Task AddCargoAsync_ShipDoesNotExist_ThrowsException()
        {
            var mockRepo = new Mock<IRepository<Cargoes>>();
            var mockShipRepo = MockShipRepoReturning(null); // Depocu "böyle bir gemi yok" diyor
            var service = new CargoService(mockRepo.Object, mockShipRepo.Object);

            var cargo = new Cargoes { WeightTon = 100, Description = "Yük", CargoType = "Dökme", ShipId = 999 };

            var ex = await Assert.ThrowsAsync<Exception>(() => service.AddCargoAsync(cargo));
            Assert.Equal("Seçilen gemi sistemde bulunamadı!", ex.Message);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<Cargoes>()), Times.Never);
        }

        // 4. Senaryo: Açıklama boşsa kaydetmemeli (zorunlu alan kontrolü)
        [Fact]
        public async Task AddCargoAsync_DescriptionIsEmpty_ThrowsException()
        {
            var mockRepo = new Mock<IRepository<Cargoes>>();
            var mockShipRepo = MockShipRepoReturning(new Ship { ShipId = 1 });
            var service = new CargoService(mockRepo.Object, mockShipRepo.Object);

            var cargo = new Cargoes { WeightTon = 100, Description = "", CargoType = "Dökme", ShipId = 1 };

            var ex = await Assert.ThrowsAsync<Exception>(() => service.AddCargoAsync(cargo));
            Assert.Equal("Yük açıklaması zorunludur!", ex.Message);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<Cargoes>()), Times.Never);
        }
    }
}
