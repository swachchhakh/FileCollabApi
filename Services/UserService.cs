using MongoDB.Driver;
using System.Security.Cryptography;
using System.Text;

public class UserService : IUserService {
    private readonly IMongoCollection<User> _users;

    public UserService(IMongoDatabase db) {
        _users = db.GetCollection<User>("users");
    }

    public async Task<User> CreateAsync(string email, string password) {
        var u = new User { Email = email, PasswordHash = HashPassword(password), Roles = new List<string>{"Editor"} };
        await _users.InsertOneAsync(u);
        return u;
    }

    public async Task<User> GetByEmailAsync(string email) =>
        await _users.Find(u => u.Email == email).FirstOrDefaultAsync();

    public async Task<User> GetByIdAsync(string id) =>
        await _users.Find(u => u.Id == id).FirstOrDefaultAsync();

    public bool VerifyPassword(string hash, string password) {
        return hash == HashPassword(password); // demo only
    }

    public string HashPassword(string password) {
        // demo hashing (NOT production secure)
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
