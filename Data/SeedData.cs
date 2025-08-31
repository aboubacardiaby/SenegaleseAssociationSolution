using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SenegaleseAssociation.Constants;
using SenegaleseAssociation.Models;

namespace SenegaleseAssociation.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Seed Roles
            foreach (var roleName in Roles.AllRoles)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed Super Admin User (with ultimate access)
            await SeedAdminUser(userManager, "superadmin@samn.org", "Super", "Admin", "SuperAdmin2024!@#", Roles.SuperAdmin);
            
            // Seed Admin Users
            await SeedAdminUser(userManager, "admin@samn.org", "Admin", "User", "Admin123!", Roles.Admin);
            await SeedAdminUser(userManager, "finance@samn.org", "Finance", "Manager", "Finance123!", Roles.Finance);
            await SeedAdminUser(userManager, "organization@samn.org", "Organization", "Manager", "Org123!", Roles.Organization);

            // Look for any existing data
            if (context.Events.Any() || context.Services.Any() || context.Leadership.Any())
            {
                return; // DB has been seeded
            }

            // Seed Services
            context.Services.AddRange(
                new Service
                {
                    Title = "Immigration Support",
                    Description = "Comprehensive assistance with visa applications, green card processes, and citizenship documentation.",
                    IconClass = "fas fa-passport",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new Service
                {
                    Title = "Cultural Integration",
                    Description = "Programs to help new immigrants adapt to American culture while preserving Senegalese traditions.",
                    IconClass = "fas fa-handshake",
                    DisplayOrder = 2,
                    IsActive = true
                },
                new Service
                {
                    Title = "Language Classes",
                    Description = "English as Second Language (ESL) classes and Wolof language preservation programs.",
                    IconClass = "fas fa-language",
                    DisplayOrder = 3,
                    IsActive = true
                },
                new Service
                {
                    Title = "Legal Assistance",
                    Description = "Access to immigration lawyers and legal aid for document preparation and court representation.",
                    IconClass = "fas fa-balance-scale",
                    DisplayOrder = 4,
                    IsActive = true
                },
                new Service
                {
                    Title = "Community Events",
                    Description = "Regular cultural celebrations, festivals, and networking events for the Senegalese community.",
                    IconClass = "fas fa-calendar-alt",
                    DisplayOrder = 5,
                    IsActive = true
                },
                new Service
                {
                    Title = "Youth Programs",
                    Description = "Educational support, scholarships, and mentorship programs for Senegalese-American youth.",
                    IconClass = "fas fa-graduation-cap",
                    DisplayOrder = 6,
                    IsActive = true
                }
            );

            // Seed Leadership
            context.Leadership.AddRange(
                new Leadership
                {
                    FirstName = "Abdou",
                    LastName = "Diaby",
                    Position = "President",
                    Bio = "Community leader with over 15 years of experience in immigration advocacy and cultural preservation.",
                    WelcomeMessage = "Assalamu Alaikum and welcome to the Senegalese Association of Minnesota! As your president, I am honored to serve our vibrant community and help preserve our rich Senegalese heritage while embracing the opportunities America offers. Together, we build bridges between our homeland and our new home, supporting each other through every challenge and celebrating every success. Insha'Allah, we will continue to grow stronger as one united community.",
                    ImageUrl = "/images/president-abdou.jpg",
                    Email = "president@samn.org",
                    ServiceStartDate = new DateTime(2020, 1, 1),
                    DisplayOrder = 1,
                    IsActive = true,
                    IsPresident = true
                },
                new Leadership
                {
                    FirstName = "Fatou",
                    LastName = "Ba",
                    Position = "Vice President",
                    Bio = "Dedicated community organizer focused on women's empowerment and family support services.",
                    WelcomeMessage = "",
                    ImageUrl = "/images/vp-fatou.jpg",
                    Email = "vp@samn.org",
                    ServiceStartDate = new DateTime(2021, 3, 1),
                    DisplayOrder = 2,
                    IsActive = true,
                    IsPresident = false
                },
                new Leadership
                {
                    FirstName = "Mamadou",
                    LastName = "Ndiaye",
                    Position = "Secretary",
                    Bio = "Former educator passionate about youth development and educational advancement.",
                    WelcomeMessage = "",
                    ImageUrl = "/images/secretary-mamadou.jpg",
                    Email = "secretary@samn.org",
                    ServiceStartDate = new DateTime(2021, 1, 1),
                    DisplayOrder = 3,
                    IsActive = true,
                    IsPresident = false
                }
            );

            // Seed Events
            context.Events.AddRange(
                new Event
                {
                    Title = "Tabaski Celebration 2024",
                    Description = "Join us for our annual Eid al-Adha celebration with traditional food, music, and prayers.",
                    Date = new DateTime(2024, 6, 16),
                    Time = "2:00 PM - 8:00 PM",
                    Location = "Minneapolis Convention Center",
                    Category = "Religious",
                    IsActive = true
                },
                new Event
                {
                    Title = "Independence Day Gala",
                    Description = "Celebrate Senegal's independence with cultural performances and networking.",
                    Date = new DateTime(2024, 4, 4),
                    Time = "6:00 PM - 11:00 PM",
                    Location = "Hyatt Regency Minneapolis",
                    Category = "Cultural",
                    IsActive = true
                },
                new Event
                {
                    Title = "Youth Scholarship Awards",
                    Description = "Annual ceremony recognizing academic excellence in our community's youth.",
                    Date = new DateTime(2024, 8, 15),
                    Time = "3:00 PM - 6:00 PM",
                    Location = "University of Minnesota",
                    Category = "Education",
                    IsActive = true
                },
                new Event
                {
                    Title = "Immigration Workshop",
                    Description = "Free legal consultation and information session on immigration processes.",
                    Date = new DateTime(2024, 9, 20),
                    Time = "10:00 AM - 2:00 PM",
                    Location = "SAM Community Center",
                    Category = "Legal",
                    IsActive = true
                }
            );

            // Seed Community Highlights
            context.CommunityHighlights.AddRange(
                new CommunityHighlight
                {
                    Title = "Strong Community",
                    Description = "Building lasting connections and support networks within our Senegalese community in Minnesota.",
                    IconClass = "fas fa-users",
                    BackgroundClass = "bg-gradient-primary",
                    Stat1Number = "500+",
                    Stat1Label = "Members",
                    Stat2Number = "50+",
                    Stat2Label = "Events",
                    Stat3Number = "15",
                    Stat3Label = "Years",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new CommunityHighlight
                {
                    Title = "Cultural Heritage",
                    Description = "Preserving and celebrating the rich traditions of Senegal while embracing American opportunities.",
                    IconClass = "fas fa-star-and-crescent",
                    BackgroundClass = "bg-gradient-success",
                    Stat1Number = "12",
                    Stat1Label = "Festivals",
                    Stat2Number = "8",
                    Stat2Label = "Languages",
                    Stat3Number = "100%",
                    Stat3Label = "Pride",
                    DisplayOrder = 2,
                    IsActive = true
                },
                new CommunityHighlight
                {
                    Title = "Education First",
                    Description = "Supporting academic excellence and providing scholarships for the next generation.",
                    IconClass = "fas fa-graduation-cap",
                    BackgroundClass = "bg-gradient-info",
                    Stat1Number = "$50K+",
                    Stat1Label = "Scholarships",
                    Stat2Number = "95%",
                    Stat2Label = "Success Rate",
                    Stat3Number = "200+",
                    Stat3Label = "Students",
                    DisplayOrder = 3,
                    IsActive = true
                },
                new CommunityHighlight
                {
                    Title = "Immigration Help",
                    Description = "Providing comprehensive support for immigration processes and legal documentation.",
                    IconClass = "fas fa-passport",
                    BackgroundClass = "bg-gradient-warning",
                    Stat1Number = "1000+",
                    Stat1Label = "Cases Helped",
                    Stat2Number = "85%",
                    Stat2Label = "Success Rate",
                    Stat3Number = "24/7",
                    Stat3Label = "Support",
                    DisplayOrder = 4,
                    IsActive = true
                },
                new CommunityHighlight
                {
                    Title = "Family Unity",
                    Description = "Strengthening family bonds and supporting families through all stages of integration.",
                    IconClass = "fas fa-heart",
                    BackgroundClass = "bg-gradient-danger",
                    Stat1Number = "300+",
                    Stat1Label = "Families",
                    Stat2Number = "20+",
                    Stat2Label = "Programs",
                    Stat3Number = "∞",
                    Stat3Label = "Love",
                    DisplayOrder = 5,
                    IsActive = true
                }
            );

            // Seed Testimonials
            context.Testimonials.AddRange(
                new Testimonial
                {
                    AuthorName = "Aisha Diop",
                    AuthorTitle = "Community Member",
                    Content = "The Senegalese Association has been like a second family to us. When we first arrived from Dakar, they helped us with everything from finding housing to understanding the school system. Alhamdulillah for this amazing community!",
                    AuthorImageUrl = "/images/testimonial1.jpg",
                    Rating = 5,
                    IsActive = true,
                    IsFeatured = true
                },
                new Testimonial
                {
                    AuthorName = "Ibrahim Fall",
                    AuthorTitle = "Business Owner",
                    Content = "Through the association's networking events, I was able to connect with other Senegalese entrepreneurs and learn about business opportunities in Minnesota. Now I own three successful restaurants in the Twin Cities!",
                    AuthorImageUrl = "/images/testimonial2.jpg",
                    Rating = 5,
                    IsActive = true,
                    IsFeatured = true
                },
                new Testimonial
                {
                    AuthorName = "Mariam Sow",
                    AuthorTitle = "University Student",
                    Content = "Thanks to the scholarship program, I'm pursuing my dreams of becoming a doctor. The mentorship and support I receive from the community keeps me motivated and connected to my roots.",
                    AuthorImageUrl = "/images/testimonial3.jpg",
                    Rating = 5,
                    IsActive = true,
                    IsFeatured = false
                }
            );

            context.SaveChanges();
        }
        
        private static async Task SeedAdminUser(UserManager<ApplicationUser> userManager, string email, string firstName, string lastName, string password, string role)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EmailConfirmed = true,
                    IsActive = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}