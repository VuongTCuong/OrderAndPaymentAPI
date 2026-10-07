
using OrderAndPaymentAPI.Entities;

public interface IUserService
{
    Task AddAsync(User user);
    Task<User> GetByEmailAsync(string email);
    Task<User> GetByIdAsync(int Id);
}

public class UserService: IUserService
{
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepo)
    {
        _userRepo = userRepo;
    }

    public Task AddAsync(User user) => _userRepo.AddAsync(user);

    public async Task<User> GetByEmailAsync(string email)
    {
        return await _userRepo.GetByEmailAsync(email);
    }

    public async Task<User> GetByIdAsync(int Id)
    {
        return await _userRepo.GetByIdAsync(Id);
    }
}
