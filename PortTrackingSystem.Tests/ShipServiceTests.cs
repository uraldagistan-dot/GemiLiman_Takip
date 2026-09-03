using Moq;
using Xunit;
using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Repositories;
using PortTrackingSystem.Core.Services;

namespace PortTrackingSystem.Tests
{
    public class ShipServiceTests
    {
        private static Ship ValidShip() => new()
        {
            Name = "MSC Zoe",
            IMO = "IMO9703291",
            Type = "Kargo",
            Flag = "TR",
            YearBuilt = 2015
        };

        // EKRAN GEREKSİNİMİ: IMO numarası benzersiz olmalı
        [Fact]
        public async Task AddShipAsync_DuplicateImo_ThrowsException()
        {
            var mockRepo = new Mock<IRepository<Ship>>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Ship>
            {
                new Ship { ShipId = 1, IMO = "IMO9703291" }
            });

            var service = new ShipService(mockRepo.Object);

            var ex = await Assert.ThrowsAsync<Exception>(() => service.AddShipAsync(ValidShip()));
            Assert.Equal("Bu IMO numarasına sahip bir gemi zaten sistemde kayıtlı!", ex.Message);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<Ship>()), Times.Never);
        }

        [Fact]
        public async Task AddShipAsync_ValidShip_CallsRepositoryAddAsync()
        {
            var mockRepo = new Mock<IRepository<Ship>>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Ship>());

            var service = new ShipService(mockRepo.Object);
            var ship = ValidShip();

            await service.AddShipAsync(ship);

            mockRepo.Verify(r => r.AddAsync(ship), Times.Once);
        }

        // Güncellemede gemi KENDİ IMO'suna takılmamalı
        [Fact]
        public async Task UpdateShipAsync_SameShipKeepsItsOwnImo_Succeeds()
        {
            var existing = new Ship { ShipId = 1, IMO = "IMO9703291", Name = "Eski Ad", Type = "Kargo", Flag = "TR", YearBuilt = 2015 };

            var mockRepo = new Mock<IRepository<Ship>>();
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Ship> { existing });

            var service = new ShipService(mockRepo.Object);

            var updated = ValidShip();
            updated.Name = "Yeni Ad"; // IMO aynı kalıyor

            await service.UpdateShipAsync(1, updated);

            Assert.Equal("Yeni Ad", existing.Name);
            mockRepo.Verify(r => r.Update(existing), Times.Once);
        }

        // Başka bir geminin IMO'su alınamaz
        [Fact]
        public async Task UpdateShipAsync_ImoBelongsToAnotherShip_ThrowsException()
        {
            var existing = new Ship { ShipId = 1, IMO = "IMO1111111" };
            var other = new Ship { ShipId = 2, IMO = "IMO9703291" };

            var mockRepo = new Mock<IRepository<Ship>>();
            mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Ship> { existing, other });

            var service = new ShipService(mockRepo.Object);

            var ex = await Assert.ThrowsAsync<Exception>(() => service.UpdateShipAsync(1, ValidShip()));
            Assert.Equal("Bu IMO numarasına sahip başka bir gemi zaten sistemde kayıtlı!", ex.Message);
            mockRepo.Verify(r => r.Update(It.IsAny<Ship>()), Times.Never);
        }
    }
}
