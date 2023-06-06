using System.ComponentModel.DataAnnotations;

namespace JokesMVC2023.Models
{
    public class JokeCreateDTO
    {
        [Required]
        [StringLength(255)]
        public string JokeQuestion { get; set; }
        [Required]
        [StringLength(2000)]
        public string JokeAnswer { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
