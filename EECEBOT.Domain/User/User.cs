using System.Globalization;
using EECEBOT.Domain.Common.Enums;

namespace EECEBOT.Domain.User;

public class User
{
    private User(Guid id,
        string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string password,
        Role role)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Password = password;
        Role = role;
    }

    public Guid Id { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string Email { get; private set; }
    public string PhoneNumber { get; private set; }
    public string Password { get; private set; }
    public Role Role { get; private set; }
    
    public static User Create(string firstName,
        string lastName,
        string email,
        string phoneNumber,
        string password,
        Role role) => new User(Guid.NewGuid(),
        new CultureInfo("en-US", false).TextInfo.ToTitleCase(firstName.ToLower()),
        new CultureInfo("en-US", false).TextInfo.ToTitleCase(lastName.ToLower()),
        email.ToLower(),
        phoneNumber,
        password,
        role);
    
    public string FullName => $"{FirstName} {LastName}";
}