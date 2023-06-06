using JokesMVC2023.Areas.Identity.Data;

namespace JokesMVC2023.Models.Data
{
    public class Joke
    {
        public int Id { get; set; }
        public string JokeQuestion { get; set; }
        public string JokeAnswer { get; set; }

        public string? AppUserId { get; set; }

        public AppUser? AppUser { get; set; }
    }
}
