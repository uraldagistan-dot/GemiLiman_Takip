using Moq;
using Xunit;
using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Repositories;
using PortTrackingSystem.Core.Services;

namespace PortTrackingSystem.Tests
{
    public class CrewMemberServiceTests
    {
        private static Mock<IRepository<CrewMembers>> EmptyRepo()
        {
            var mockRepo = new Mock<IRepository<CrewMembers>>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<CrewMembers>());
            return mockRepo;
        }

        private static CrewMembers ValidCrew() => new()
        {
            FirstName = "Ali",
            LastName = "Yılmaz",
            Email = "ali@firma.com",
            PhoneNumber = "+90 532 123 45 67",
            Role = "Kaptan"
        };

        // EKRAN GEREKSİNİMİ: Görev alanı zorunlu
        [Fact]
        public async Task AddCrewMemberAsync_RoleIsEmpty_ThrowsException()
        {
            var mockRepo = EmptyRepo();
            var service = new CrewMemberService(mockRepo.Object);

            var crew = ValidCrew();
            crew.Role = "";

            var ex = await Assert.ThrowsAsync<Exception>(() => service.AddCrewMemberAsync(crew));
            Assert.Equal("Görev alanı zorunludur!", ex.Message);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<CrewMembers>()), Times.Never);
        }

        // EKRAN GEREKSİNİMİ: E-posta format kontrolü
        [Theory]
        [InlineData("duz-yazi")]
        [InlineData("ali@")]
        [InlineData("@firma.com")]
        [InlineData("ali@firma")]
        public async Task AddCrewMemberAsync_InvalidEmail_ThrowsException(string email)
        {
            var mockRepo = EmptyRepo();
            var service = new CrewMemberService(mockRepo.Object);

            var crew = ValidCrew();
            crew.Email = email;

            var ex = await Assert.ThrowsAsync<Exception>(() => service.AddCrewMemberAsync(crew));
            Assert.Equal("Geçersiz e-posta formatı! (örnek: ali@firma.com)", ex.Message);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<CrewMembers>()), Times.Never);
        }

        // EKRAN GEREKSİNİMİ: Telefon format kontrolü (+90 5XX XXX XX XX)
        [Theory]
        [InlineData("123")]
        [InlineData("+90 432 123 45 67")]   // 5 ile başlamıyor
        [InlineData("+90 532 123 45")]      // eksik hane
        public async Task AddCrewMemberAsync_InvalidPhone_ThrowsException(string phone)
        {
            var mockRepo = EmptyRepo();
            var service = new CrewMemberService(mockRepo.Object);

            var crew = ValidCrew();
            crew.PhoneNumber = phone;

            var ex = await Assert.ThrowsAsync<Exception>(() => service.AddCrewMemberAsync(crew));
            Assert.Equal("Geçersiz telefon formatı! (örnek: +90 532 123 45 67)", ex.Message);
        }

        // İYİ SENARYO: Geçerli telefon formatlarının hepsi kabul edilmeli
        [Theory]
        [InlineData("+90 532 123 45 67")]
        [InlineData("05321234567")]
        [InlineData("5321234567")]
        public async Task AddCrewMemberAsync_ValidData_CallsRepositoryAddAsync(string phone)
        {
            var mockRepo = EmptyRepo();
            var service = new CrewMemberService(mockRepo.Object);

            var crew = ValidCrew();
            crew.PhoneNumber = phone;

            await service.AddCrewMemberAsync(crew);

            mockRepo.Verify(r => r.AddAsync(crew), Times.Once);
        }

        // İŞ KURALI: Aynı e-posta ile ikinci personel eklenemez
        [Fact]
        public async Task AddCrewMemberAsync_DuplicateEmail_ThrowsException()
        {
            var mockRepo = new Mock<IRepository<CrewMembers>>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<CrewMembers>
            {
                new CrewMembers { CrewId = 1, Email = "ali@firma.com" }
            });

            var service = new CrewMemberService(mockRepo.Object);

            var ex = await Assert.ThrowsAsync<Exception>(() => service.AddCrewMemberAsync(ValidCrew()));
            Assert.Equal("Bu e-posta adresiyle kayıtlı bir personel zaten var!", ex.Message);
            mockRepo.Verify(r => r.AddAsync(It.IsAny<CrewMembers>()), Times.Never);
        }
    }
}
