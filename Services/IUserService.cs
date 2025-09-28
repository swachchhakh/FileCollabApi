public interface IUserService {
    Task<User> CreateAsync(string email, string password);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByIdAsync(string id);
    bool VerifyPassword(string hash, string password);
    string HashPassword(string password);
}
