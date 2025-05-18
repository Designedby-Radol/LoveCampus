using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CampusLove.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public int GenderId { get; set; }
        public string Gender { get; set; } // Navigation property
        public int CareerId { get; set; }
        public string Career { get; set; } // Navigation property
        public string Email { get; set; }
        public string Password { get; set; }
        public string ProfilePhrase { get; set; }
        public int AvailableCredits { get; set; }
        public int Capcoins { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastAccess { get; set; }
        public string Role { get; set; }
        public List<Interest> Interests { get; set; } = new List<Interest>();

        public string FormattedName => 
            string.IsNullOrEmpty(Name) ? "Usuario sin nombre" : 
            CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Name.ToLower());

        public string FormattedCredits => AvailableCredits.ToString("N0", CultureInfo.CurrentCulture);

        public string FormattedCapcoins => Capcoins.ToString("N0", CultureInfo.CurrentCulture);

        public User()
        {
            // Default constructor
            AvailableCredits = 5;
            Capcoins = 0;
            Role = "usuario";
            RegistrationDate = DateTime.Now;
            Name = string.Empty;
            Gender = string.Empty;
            Career = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            ProfilePhrase = string.Empty;
            Interests = new List<Interest>();
        }

        public override string ToString()
        {
            string interestsText = Interests.Count > 0 
                ? string.Join(", ", Interests.Select(i => i.Name)) 
                : "No interests specified";

            return $"Name: {FormattedName}\n" +
                   $"Age: {Age}\n" +
                   $"Gender: {Gender}\n" +
                   $"Career: {Career}\n" +
                   $"Interests: {interestsText}\n" +
                   $"Profile Phrase: {ProfilePhrase}\n" +
                   $"Available Credits: {FormattedCredits}\n";
        }

        public string GetShortProfile()
        {
            return $"{FormattedName}, {Age} - {Career}";
        }
    }
}