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

    public async Task CreateUser(RegisterDTO user)
    {
        var saltBytes = RandomNumberGenerator.GetBytes(16);

        var passwordHash = Rfc2898DeriveBytes.Pbkdf2(
            user.Password,
            saltBytes,
            100000,
            HashAlgorithmName.SHA256,
            32);

        var newUser = new User
        {
            Username = user.Username,
            Email = user.Email,
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

        if (existingUser == null)
            return;

        existingUser.Username = user.Username;
        existingUser.Email = user.Email;
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
