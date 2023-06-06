using JokesMVC2023.Models.Data;
using Microsoft.AspNetCore.Identity;

namespace JokesMVC2023.Areas.Identity.Data;

// Add profile data for application users by adding properties to the AppUser class
public class AppUser : IdentityUser
{


    public string ProfilePhotoFileName { get; set; } = "19466d52-09b7-4104-a96e-95842ce49e15.png";
    public virtual ICollection<Joke>? Jokes { get; set; }
}

