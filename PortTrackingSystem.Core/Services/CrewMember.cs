using System.Text.RegularExpressions;
using PortTrackingSystem.Core.Entities;
using PortTrackingSystem.Core.Repositories;

namespace PortTrackingSystem.Core.Services
{
    public class CrewMemberService : ICrewMemberService
    {
        private readonly IRepository<CrewMembers> _crewRepository;

        // E-posta format kontrolü: ad@alan.uzanti
        private static readonly Regex EmailPattern = new(
            @"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$", RegexOptions.Compiled);

        // Telefon format kontrolü: +90 5XX XXX XX XX (boşluklu/boşluksuz, 0 ile veya önek olmadan)
        private static readonly Regex PhonePattern = new(
            @"^(\+90|0)?\s*5\d{2}\s*\d{3}\s*\d{2}\s*\d{2}$", RegexOptions.Compiled);

        public CrewMemberService(IRepository<CrewMembers> crewRepository)
        {
            _crewRepository = crewRepository;
        }

        public async Task<IEnumerable<CrewMembers>> GetAllCrewMembersAsync()
        {
            return await _crewRepository.GetAllAsync();
        }

        public async Task<CrewMembers?> GetCrewMemberByIdAsync(int id)
        {
            return await _crewRepository.GetByIdAsync(id);
        }

        public async Task AddCrewMemberAsync(CrewMembers crewMember)
        {
            Validate(crewMember);

            // İŞ KURALI: Aynı e-posta ile ikinci bir personel kaydedilemez
            var all = await _crewRepository.GetAllAsync();
            bool emailExists = all.Any(c =>
                !string.IsNullOrEmpty(c.Email) &&
                c.Email.ToLower() == crewMember.Email!.ToLower());

            if (emailExists)
            {
                throw new Exception("Bu e-posta adresiyle kayıtlı bir personel zaten var!");
            }

            await _crewRepository.AddAsync(crewMember);
        }

        public async Task UpdateCrewMemberAsync(int id, CrewMembers crewMember)
        {
            var existing = await _crewRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("Güncellenecek personel bulunamadı!");
            }

            Validate(crewMember);

            var all = await _crewRepository.GetAllAsync();
            bool emailExists = all.Any(c =>
                !string.IsNullOrEmpty(c.Email) &&
                c.Email.ToLower() == crewMember.Email!.ToLower() &&
                c.CrewId != id);

            if (emailExists)
            {
                throw new Exception("Bu e-posta adresiyle kayıtlı başka bir personel zaten var!");
            }

            existing.FirstName = crewMember.FirstName;
            existing.LastName = crewMember.LastName;
            existing.Email = crewMember.Email;
            existing.PhoneNumber = crewMember.PhoneNumber;
            existing.Role = crewMember.Role;

            _crewRepository.Update(existing);
        }

        public async Task DeleteCrewMemberAsync(int id)
        {
            var existing = await _crewRepository.GetByIdAsync(id);
            if (existing == null)
            {
                throw new Exception("Silinecek personel bulunamadı!");
            }

            // Personelin gemi atamaları FK cascade ile birlikte silinir
            _crewRepository.Delete(existing);
        }

        private static void Validate(CrewMembers crew)
        {
            if (string.IsNullOrWhiteSpace(crew.FirstName))
                throw new Exception("Ad zorunludur!");

            if (string.IsNullOrWhiteSpace(crew.LastName))
                throw new Exception("Soyad zorunludur!");

            // EKRAN GEREKSİNİMİ: Görev alanı zorunlu
            if (string.IsNullOrWhiteSpace(crew.Role))
                throw new Exception("Görev alanı zorunludur!");

            // EKRAN GEREKSİNİMİ: E-posta validasyonu
            if (string.IsNullOrWhiteSpace(crew.Email))
                throw new Exception("E-posta zorunludur!");

            if (!EmailPattern.IsMatch(crew.Email))
                throw new Exception("Geçersiz e-posta formatı! (örnek: ali@firma.com)");

            // EKRAN GEREKSİNİMİ: Telefon validasyonu
            if (string.IsNullOrWhiteSpace(crew.PhoneNumber))
                throw new Exception("Telefon numarası zorunludur!");

            if (!PhonePattern.IsMatch(crew.PhoneNumber))
                throw new Exception("Geçersiz telefon formatı! (örnek: +90 532 123 45 67)");
        }
    }
}
