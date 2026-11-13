using AutoMapper;
using MusicPortal.BLL.DTO;
using MusicPortal.BLL.Interfaces;
using MusicPortal.DAL.Entities;
using MusicPortal.DAL.Interfaces;
using System.Security.Cryptography;

namespace MusicPortal.BLL.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork database;
    private readonly IMapper mapper;

    public UserService(IUnitOfWork database, IMapper mapper)
    {
        this.database = database;
        this.mapper = mapper;
    }

    public async Task<IEnumerable<UserDTO>> GetAllUsers()
    {
        var users = await database.Users.GetAll();
        return mapper.Map<IEnumerable<UserDTO>>(users);
    }

    public async Task<UserDTO?> GetUser(int id)
    {
        var user = await database.Users.Get(id);
        return mapper.Map<UserDTO?>(user);
    }

    public async Task<UserDTO?> Authenticate(string username, string password)
    {
        var user = await database.Users.Get(username.Trim());
        if (user == null || string.IsNullOrWhiteSpace(user.Salt) || string.IsNullOrWhiteSpace(user.Password))
            return null;

        try
        {
            var salt = Convert.FromBase64String(user.Salt);
            var storedHash = Convert.FromBase64String(user.Password);
            var enteredHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100000,
                HashAlgorithmName.SHA256, storedHash.Length);

            return CryptographicOperations.FixedTimeEquals(storedHash, enteredHash)
                ? mapper.Map<UserDTO>(user)
                : null;
        }
        catch (FormatException)
        {
            return null;
        }
    }

    public async Task CreateUser(RegisterDTO user)
    {
        var users = await database.Users.GetAll();
        if (users.Any(x => x.Username.Equals(user.Username.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Користувач із таким ім'ям уже існує.");
        if (users.Any(x => x.Email.Equals(user.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Користувач із такою електронною поштою вже існує.");

        var saltBytes = RandomNumberGenerator.GetBytes(16);
        var passwordHash = Rfc2898DeriveBytes.Pbkdf2(
            user.Password, saltBytes, 100000, HashAlgorithmName.SHA256, 32);

        var newUser = new User
        {
            Username = user.Username.Trim(),
            Email = user.Email.Trim(),
            Salt = Convert.ToBase64String(saltBytes),
            Password = Convert.ToBase64String(passwordHash),
            IsAdmin = false,
            IsRegistrationConfirmed = false
        };

        await database.Users.Create(newUser);
        await database.Save();
    }

    public async Task UpdateUser(UserDTO user)
    {
        var existingUser = await database.Users.Get(user.Id);
        if (existingUser == null) return;

        var allUsers = await database.Users.GetAll();
        if (allUsers.Any(x => x.Id != user.Id &&
            x.Username.Equals(user.Username.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Користувач із таким ім'ям уже існує.");
        if (allUsers.Any(x => x.Id != user.Id &&
            x.Email.Equals(user.Email.Trim(), StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Користувач із такою електронною поштою вже існує.");

        existingUser.Username = user.Username.Trim();
        existingUser.Email = user.Email.Trim();
        existingUser.IsAdmin = user.IsAdmin;
        existingUser.IsRegistrationConfirmed = user.IsRegistrationConfirmed;
        database.Users.Update(existingUser);
        await database.Save();
    }

    public async Task DeleteUser(int id)
    {
        await database.Users.Delete(id);
        await database.Save();
    }
}
