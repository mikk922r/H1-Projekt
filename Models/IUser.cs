namespace Projekt.Models
{
    public interface IUser
    {
        string Email { get; set; }

        string Name { get; set; }

        int PhoneCode { get; set; }

        string PhoneNumber { get; set; }

        string Password { get; set; }
    }
}