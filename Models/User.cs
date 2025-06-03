namespace Projekt.Models
{
    public class User : IUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int PhoneCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
    }
}