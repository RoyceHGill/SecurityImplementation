using JokesMVC2023.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JokesMVC2023.Models.Data

{

    public class JokeDBContext : IdentityDbContext<AppUser>
    {
        public JokeDBContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Joke> Jokes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Joke>()
                .HasOne(j => j.AppUser)
                .WithMany(a => a.Jokes)
                .HasForeignKey(j => j.AppUserId);


            //builder.Entity<Joke>().HasData(
            //    new Joke { Id = 1, JokeQuestion = "What do you get if you lock a monkey in a room with a typewriter for 8 hours?", JokeAnswer = "A regular expression." },
            //    new Joke { Id = 2, JokeQuestion = "How do you generate a random string?", JokeAnswer = "Put a Windows user in front of Vim and tell them to exit." },
            //    new Joke { Id = 3, JokeQuestion = "Why did the database administrator leave his wife?", JokeAnswer = "She had one-to-many relationships." },
            //    new Joke { Id = 4, JokeQuestion = "How many programmers does it take to screw in a light bulb?", JokeAnswer = "None. It's a hardware problem." },
            //    new Joke { Id = 5, JokeQuestion = "Why does no one like SQLrillex?", JokeAnswer = "He keeps dropping the database." },
            //    new Joke { Id = 6, JokeQuestion = "Why are Assembly programmers always soaking wet?", JokeAnswer = "They work below C-level." },
            //    new Joke { Id = 7, JokeQuestion = "What's the difference between a poorly dressed man on a unicycle and a well dressed man on a bicycle?", JokeAnswer = "Attire." },
            //    new Joke { Id = 8, JokeQuestion = "What time did the man go to the dentist?", JokeAnswer = "Tooth hurt-y." },
            //    new Joke { Id = 9, JokeQuestion = "So I made a graph of all my past relationships.", JokeAnswer = "It has an ex axis and a why axis." },
            //    new Joke { Id = 10, JokeQuestion = "I just got fired from my job at the keyboard factory.", JokeAnswer = "They told me I wasn't putting in enough shifts." }
            //    );
        }
    }
}
