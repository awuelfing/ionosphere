using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace DXws.Models.Web
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Enter Username")]
        public string? Username { get; set; }
        [Required]
        [Display(Name = "Enter Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
