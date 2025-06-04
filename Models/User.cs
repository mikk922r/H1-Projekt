namespace Projekt.Models
{
    public class User : IUser
    {
        public int Id { get; set; }
        
        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
        
        public int PhoneCode { get; set; }
        
        public string PhoneNumber { get; set; } = string.Empty;
        
        public string Password { get; set; } = string.Empty;
    }
}