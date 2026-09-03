using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortTrackingSystem.Core.Data;
using PortTrackingSystem.Core.Entities;

namespace PortTrackingSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SeedController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("RunSeed")]
        public async Task<IActionResult> RunSeed()
        {
            // 1. Önceki tüm verileri sil (İlişkisel sıraya göre)
            _context.ShipCrewAssignments.RemoveRange(_context.ShipCrewAssignments);
            _context.Cargoes.RemoveRange(_context.Cargoes);
            _context.ShipVisits.RemoveRange(_context.ShipVisits);
            _context.CrewMembers.RemoveRange(_context.CrewMembers);
            _context.Ports.RemoveRange(_context.Ports);
            _context.Ships.RemoveRange(_context.Ships);
            
            await _context.SaveChangesAsync();

            var random = new Random();

            // 2. Limanları Oluştur
            var portCities = new[] { ("Ambarlı", "Türkiye"), ("Mersin", "Türkiye"), ("İzmir", "Türkiye"), ("Rotterdam", "Hollanda"), ("Hamburg", "Almanya"), ("Antwerp", "Belçika"), ("Singapur", "Singapur"), ("Şanghay", "Çin"), ("Dubai", "BAE"), ("Los Angeles", "ABD") };
            var ports = new List<Port>();
            foreach (var (city, country) in portCities)
            {
                ports.Add(new Port { Name = $"{city} Limanı", City = city, Country = country });
            }
            await _context.Ports.AddRangeAsync(ports);
            await _context.SaveChangesAsync();

            // 3. Gemileri Oluştur
            var shipNames = new[] { "MSC Gülsün", "CMA CGM Jacques Saadé", "Ever Alot", "HMM Algeciras", "Cosco Universe", "OOCL Hong Kong", "Madrid Maersk", "Emma Maersk", "Arkas Bosphorus", "Arkas Anatolia", "Gülcemal", "Piri Reis", "Barbaros", "Turgut Reis", "Oruç Reis" };
            var ships = new List<Ship>();
            for (int i = 0; i < shipNames.Length; i++)
            {
                ships.Add(new Ship
                {
                    Name = shipNames[i],
                    IMO = $"IMO{random.Next(9000000, 9999999)}",
                    Type = random.Next(2) == 0 ? "Konteyner" : "Dökme Yük",
                    Flag = random.Next(2) == 0 ? "Türkiye" : "Panama",
                    YearBuilt = random.Next(2005, 2024)
                });
            }
            await _context.Ships.AddRangeAsync(ships);
            await _context.SaveChangesAsync();

            // 4. Mürettebatı Oluştur
            var firstNames = new[] { "Ahmet", "Mehmet", "Ali", "Veli", "Can", "Deniz", "Burak", "Emre", "Kemal", "Yusuf", "Cem", "Oğuz", "Tolga", "Hakan", "Koray" };
            var lastNames = new[] { "Yılmaz", "Kaya", "Demir", "Çelik", "Şahin", "Yıldız", "Öztürk", "Aydın", "Özdemir", "Arslan", "Doğan", "Kılıç", "Aslan", "Çetin", "Koç" };
            var roles = new[] { "Kaptan", "İkinci Kaptan", "Başmühendis", "İkinci Mühendis", "Güverte Zabiti", "Makine Zabiti", "Gemici", "Aşçı", "Fiter" };
            var crews = new List<CrewMembers>();
            for (int i = 0; i < 50; i++)
            {
                crews.Add(new CrewMembers
                {
                    FirstName = firstNames[random.Next(firstNames.Length)],
                    LastName = lastNames[random.Next(lastNames.Length)],
                    Email = $"personel{i}@arkas.com",
                    PhoneNumber = $"+905{random.Next(30,55)}{random.Next(1000000, 9999999)}",
                    Role = roles[random.Next(roles.Length)]
                });
            }
            await _context.CrewMembers.AddRangeAsync(crews);
            await _context.SaveChangesAsync();

            // 5. Ziyaretleri Oluştur
            var visits = new List<ShipVisits>();
            for (int i = 0; i < 150; i++)
            {
                var ship = ships[random.Next(ships.Count)];
                var port = ports[random.Next(ports.Count)];
                var arrival = DateTime.Now.AddDays(-random.Next(1, 365));
                var departure = arrival.AddDays(random.Next(1, 10));

                visits.Add(new ShipVisits
                {
                    ShipId = ship.ShipId,
                    PortId = port.PortId,
                    ArrivalDate = arrival,
                    DepartureDate = departure,
                    Purpose = random.Next(2) == 0 ? "Yükleme" : "Boşaltma"
                });
            }
            await _context.ShipVisits.AddRangeAsync(visits);

            // 6. Kargoları Oluştur
            var cargoes = new List<Cargoes>();
            var cargoTypes = new[] { "Gıda", "Tehlikeli Madde", "Elektronik", "Tekstil", "Makine Parçası", "Otomobil", "Kimyasal", "Mobilya" };
            for (int i = 0; i < 200; i++)
            {
                var ship = ships[random.Next(ships.Count)];
                cargoes.Add(new Cargoes
                {
                    ShipId = ship.ShipId,
                    Description = $"{cargoTypes[random.Next(cargoTypes.Length)]} Konteyneri",
                    WeightTon = (decimal)(random.NextDouble() * 500 + 10),
                    CargoType = cargoTypes[random.Next(cargoTypes.Length)]
                });
            }
            await _context.Cargoes.AddRangeAsync(cargoes);

            // 7. Atamaları Oluştur
            var assignments = new List<ShipCrewAssignments>();
            // Aynı kişiyi aynı tarihte aynı gemiye atamamaya dikkat edelim
            var assignmentSet = new HashSet<string>();
            
            for (int i = 0; i < 120; i++)
            {
                var ship = ships[random.Next(ships.Count)];
                var crew = crews[random.Next(crews.Count)];
                var date = DateTime.Now.AddDays(-random.Next(1, 100)).Date;
                
                string key = $"{ship.ShipId}_{crew.CrewId}_{date:yyyyMMdd}";
                if (!assignmentSet.Contains(key))
                {
                    assignmentSet.Add(key);
                    assignments.Add(new ShipCrewAssignments
                    {
                        ShipId = ship.ShipId,
                        CrewId = crew.CrewId,
                        AssignmentDate = date
                    });
                }
            }
            await _context.ShipCrewAssignments.AddRangeAsync(assignments);

            await _context.SaveChangesAsync();

            return Ok(new { message = "Eski veriler silindi, yeni kalabalık veri seti başarıyla eklendi!" });
        }
    }
}
