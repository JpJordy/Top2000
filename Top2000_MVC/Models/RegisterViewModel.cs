using System.ComponentModel.DataAnnotations;

namespace Top2000_MVC.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Gebruikersnaam is verplicht.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht.")]
        [MinLength(6, ErrorMessage = "Wachtwoord moet minimaal 6 tekens lang zijn.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Bevestig je wachtwoord.")]
        [Compare("Password", ErrorMessage = "Wachtwoorden komen niet overeen.")]
        public string ConfirmPassword { get; set; }

    }
}
