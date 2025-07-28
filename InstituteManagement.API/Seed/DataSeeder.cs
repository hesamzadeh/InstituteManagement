using InstituteManagement.Core.Common.ValueObjects;
using InstituteManagement.Core.Entities;
using InstituteManagement.Core.Entities.Profiles;
using InstituteManagement.Infrastructure;

namespace InstituteManagement.API.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedSampleData(AppDbContext context)
        {
            if (!context.People.Any())
            {
                var persons = new List<Person>
            {
                new Person
                {
                    NationalCode = "0011223344",
                    FirstName = "Alice",
                    LastName = "Doe",
                    Birthday = new DateOnly(1990, 1, 1)
                },
                new Person
                {
                    NationalCode = "0022334455",
                    FirstName = "Bob",
                    LastName = "Smith",
                    Birthday = new DateOnly(1985, 5, 10)
                },
                new Person
                {
                    NationalCode = "0033445566",
                    FirstName = "Charlie",
                    LastName = "Brown",
                    Birthday = new DateOnly(2000, 3, 15)
                },
                new Person
                {
                    NationalCode = "0044556677",
                    FirstName = "Dana",
                    LastName = "White",
                    Birthday = new DateOnly(1995, 8, 25)
                },
                new Person
                {
                    NationalCode = "0055667788",
                    FirstName = "Eve",
                    LastName = "Black",
                    Birthday = new DateOnly(1992, 11, 30)
                }
            };

                context.People.AddRange(persons);
                await context.SaveChangesAsync();

                // Now assign profiles using object initializer
                var institute = new InstituteProfile
                {
                    PersonId = persons[0].Id,
                    DisplayName = "Future Minds Institute",
                    NationalCode = "INST-001",
                    VerifiedAt = DateTime.UtcNow,
                    Website = "https://futureminds.org",
                    EmailAddresses = [EmailAddress.Create("info@futureminds.org")],
                    Phones = [PhoneNumber.Create("+1-800-123-4567")],
                    SocialLinks = [new SocialLink { Url = "https://instagram.com/futureminds", Platform = "Instagram" }]
                };

                var gym = new GymProfile
                {
                    PersonId = persons[1].Id,
                    DisplayName = "Strong Body Gym",
                    NationalCode = "GYM-001",
                    VerifiedAt = DateTime.UtcNow,
                    Website = "https://strongbody.fit",
                    EmailAddresses = [EmailAddress.Create("hello@strongbody.fit")],
                    Phones = [PhoneNumber.Create("+1-800-555-6789")],
                    SocialLinks = [new SocialLink { Url = "https://facebook.com/strongbody", Platform = "Facebook" }]
                };

                // Use concrete DbSets
                context.InstituteProfiles.Add(institute);
                context.GymProfiles.Add(gym);

                await context.SaveChangesAsync();
            }
        }
    }

}
