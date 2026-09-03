using Moq;
using Xunit;
using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Repositories;
using PortTrackingSystem.Core.Services;

namespace PortTrackingSystem.Tests
{
    public class ShipCrewAssignmentServiceTests
    {
        private static (Mock<IRepository<Ship>>, Mock<IRepository<CrewMembers>>) MockRelatedRepos()
        {
            var mockShipRepo = new Mock<IRepository<Ship>>();
            mockShipRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new Ship { ShipId = 1 });

            var mockCrewRepo = new Mock<IRepository<CrewMembers>>();
            mockCrewRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new CrewMembers { CrewId = 1 });

            return (mockShipRepo, mockCrewRepo);
        }

        // EKRAN GEREKSİNİMİ: Aynı mürettebat aynı gemide AYNI TARİHTE birden fazla kez atanamaz
        [Fact]
        public async Task AddAssignmentAsync_SameShipCrewAndDate_ThrowsException()
        {
            var mockRepo = new Mock<IRepository<ShipCrewAssignments>>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ShipCrewAssignments>
            {
                // Saat farklı ama gün aynı: yine de mükerrer sayılmalı
                new ShipCrewAssignments { ShipId = 1, CrewId = 1, AssignmentDate = new DateTime(2026, 5, 10, 8, 0, 0) }
            });

            var (mockShipRepo, mockCrewRepo) = MockRelatedRepos();
            var service = new ShipCrewAssignmentService(mockRepo.Object, mockShipRepo.Object, mockCrewRepo.Object);

            var assignment = new ShipCrewAssignments
            {
                ShipId = 1,
                CrewId = 1,
                AssignmentDate = new DateTime(2026, 5, 10, 17, 30, 0)
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => service.AddAssignmentAsync(assignment));
            Assert.Equal("Bu personel bu gemiye aynı tarihte zaten atanmış!", ex.Message);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<ShipCrewAssignments>()), Times.Never);
        }

        // FARKLI TARİH: Aynı personel aynı gemiye başka bir günde tekrar atanabilmeli
        [Fact]
        public async Task AddAssignmentAsync_SameShipAndCrewButDifferentDate_Succeeds()
        {
            var mockRepo = new Mock<IRepository<ShipCrewAssignments>>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ShipCrewAssignments>
            {
                new ShipCrewAssignments { ShipId = 1, CrewId = 1, AssignmentDate = new DateTime(2026, 5, 10) }
            });

            var (mockShipRepo, mockCrewRepo) = MockRelatedRepos();
            var service = new ShipCrewAssignmentService(mockRepo.Object, mockShipRepo.Object, mockCrewRepo.Object);

            var assignment = new ShipCrewAssignments
            {
                ShipId = 1,
                CrewId = 1,
                AssignmentDate = new DateTime(2026, 5, 11) // Ertesi gün
            };

            await service.AddAssignmentAsync(assignment);

            mockRepo.Verify(r => r.AddAsync(assignment), Times.Once);
        }

        [Fact]
        public async Task AddAssignmentAsync_CrewDoesNotExist_ThrowsException()
        {
            var mockRepo = new Mock<IRepository<ShipCrewAssignments>>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<ShipCrewAssignments>());

            var (mockShipRepo, _) = MockRelatedRepos();
            var mockCrewRepo = new Mock<IRepository<CrewMembers>>();
            mockCrewRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((CrewMembers)null!);

            var service = new ShipCrewAssignmentService(mockRepo.Object, mockShipRepo.Object, mockCrewRepo.Object);

            var assignment = new ShipCrewAssignments { ShipId = 1, CrewId = 999, AssignmentDate = DateTime.Now };

            var ex = await Assert.ThrowsAsync<Exception>(() => service.AddAssignmentAsync(assignment));
            Assert.Equal("Seçilen mürettebat sistemde bulunamadı!", ex.Message);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<ShipCrewAssignments>()), Times.Never);
        }
    }
}
