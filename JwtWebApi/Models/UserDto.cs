using System.ComponentModel.DataAnnotations;

namespace JwtWebApi.Models
{
    public class UserDto
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
