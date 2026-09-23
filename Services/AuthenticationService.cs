using Marketplace_Group_Project.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Marketplace_Group_Project.Services
{
    public class AuthenticationService
    {
        private const int SaltSize = 16;
        private const int HashSize = 32;
        private const int Iterations = 100_000;
        private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;
        private readonly MarketplaceDbContext _context;

        public AuthenticationService(MarketplaceDbContext context)
        {
            _context = context;
        }

        // Создать хеш пароля
        public static string HashPassword(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                password, salt, Iterations, Algorithm, HashSize);

            // соль + хеш - base64
            byte[] combined = new byte[SaltSize + HashSize];
            Buffer.BlockCopy(salt, 0, combined, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, combined, SaltSize, HashSize);

            return Convert.ToBase64String(combined);
        }

        // Проверка пароль против хеша
        public static bool VerifyPassword(string storedHash, string password)
        {
            byte[] bytes = Convert.FromBase64String(storedHash);
            if (bytes.Length != SaltSize + HashSize)
                return false;

            byte[] salt = bytes.AsSpan(0, SaltSize).ToArray();
            byte[] stored = bytes.AsSpan(SaltSize).ToArray();

            byte[] computed = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

            return CryptographicOperations.FixedTimeEquals(stored, computed);
        }

        // Занят ли логин
        public async Task<bool> IsLoginExists(string login)
        {
            return await _context.Users.AnyAsync(u => u.Login == login);
        }

        // Занят ли email
        public async Task<bool> IsEmailExists(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        // Создать нового пользователя
        // true успех / false если логин или email занят
        public async Task<bool> Register(string login, string email, string password, RoleEnum? role = null)
        {
            if (await IsLoginExists(login))
                return false;

            if (await IsEmailExists(email))
                return false;

            var user = new User
            {
                Login = login,
                Email = email,
                PasswordHash = HashPassword(password),
                Role = role ?? RoleEnum.User,
                CreatedAt = DateTime.UtcNow,
                Orders = new List<Order>()
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // Вход
        // Возвращает пользователя, иначе null. После авторизации проверяется роль.
        public async Task<User?> Login(string login, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Login == login);

            if (user == null)
                return null;

            if (!VerifyPassword(user.PasswordHash, password))
                return null;

            // После авторизации проверяется роль пользователя (RoleEnum.Admin или RoleEnum.User)
            return user;
        }

        // Выход (сброс сессии)
        public void Logout()
        {
            CurrentUser = null;
        }

        // Текущий авторизованный пользователь
        public User? CurrentUser { get; private set; }
    }
}
