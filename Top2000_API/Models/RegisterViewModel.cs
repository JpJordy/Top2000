namespace Top2000_API.Models
{
    public class RegisterViewModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; } = "User"; // Standaardrol
    }
}
