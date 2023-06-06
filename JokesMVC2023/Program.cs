
using JokesMVC2023;
using JokesMVC2023.Areas.Identity.Data;
using JokesMVC2023.Models.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Configuration;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        string connectionString = "";
#if DEBUG
        connectionString = builder.Configuration.GetConnectionString("JokesDBSQL");
#else
        connectionString = Environment.GetEnvironmentVariable("JokesDBSQL");
#endif
        builder.Services.AddScoped<ProfilePictureUploader>();
        builder.Services.AddScoped<EncryptionService>();
        builder.Services.AddDbContext<JokeDBContext>(c => c.
        UseSqlServer(connectionString));

        builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<JokeDBContext>();
        builder.Services.AddRazorPages();
        builder.Services.AddSession();
        builder.Services.AddDistributedMemoryCache();


        var app = builder.Build();


        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSession();
        app.UseRouting();
        app.UseAuthentication(); ;
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        using (var scope = app.Services.CreateScope())
        {
            var roleManger = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var roles = new[] { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManger.RoleExistsAsync(role)) await roleManger.CreateAsync(new IdentityRole(role));
            }

        }

        using (var scope = app.Services.CreateScope())
        {
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

            string email = "Unkown@Unkown.com";

            if (await userManager.FindByEmailAsync(email) == null)
            {
                var user = new AppUser();
                user.UserName = email;
                user.Email = email;

                var result = await userManager.CreateAsync(user);

                List<Joke> jokes = new List<Joke>
                {
                    new Joke { JokeQuestion = "What do you get if you lock a monkey in a room with a typewriter for 8 hours?", JokeAnswer = "A regular expression.", AppUserId = user.Id },
                    new Joke { JokeQuestion = "How do you generate a random string?", JokeAnswer = "Put a Windows user in front of Vim and tell them to exit.", AppUserId = user.Id  },
                    new Joke { JokeQuestion = "Why did the database administrator leave his wife?", JokeAnswer = "She had one-to-many relationships.", AppUserId = user.Id  },
                    new Joke { JokeQuestion = "How many programmers does it take to screw in a light bulb?", JokeAnswer = "None. It's a hardware problem.", AppUserId = user.Id  },
                    new Joke { JokeQuestion = "Why does no one like SQLrillex?", JokeAnswer = "He keeps dropping the database.", AppUserId = user.Id  },
                    new Joke { JokeQuestion = "Why are Assembly programmers always soaking wet?", JokeAnswer = "They work below C-level.", AppUserId = user.Id  },
                    new Joke { JokeQuestion = "What's the difference between a poorly dressed man on a unicycle and a well dressed man on a bicycle?", JokeAnswer = "Attire.", AppUserId = user.Id  },
                    new Joke { JokeQuestion = "What time did the man go to the dentist?", JokeAnswer = "Tooth hurt-y.", AppUserId = user.Id  },
                    new Joke { JokeQuestion = "So I made a graph of all my past relationships.", JokeAnswer = "It has an ex axis and a why axis.", AppUserId = user.Id  },
                    new Joke { JokeQuestion = "I just got fired from my job at the keyboard factory.", JokeAnswer = "They told me I wasn't putting in enough shifts.", AppUserId = user.Id  }
                };

                var jokeContext = scope.ServiceProvider.GetRequiredService<JokeDBContext>();

                foreach (var joke in jokes)
                {
                    var results = jokeContext.Add(joke);
                    jokeContext.SaveChanges();
                }

            }
        }

        app.MapRazorPages();
        app.Run();
    }
}

