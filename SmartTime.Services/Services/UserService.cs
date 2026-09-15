using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;
using SmartTime.Services.Interfaces;

namespace SmartTime.Services.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<AppUser>> GetAllAsync() =>
        await _unitOfWork.Repository<AppUser>().GetAllAsync();

    public async Task<AppUser?> GetByIdAsync(int id) =>
        await _unitOfWork.Repository<AppUser>().GetByIdAsync(id);

    public async Task<AppUser> CreateAsync(string name, string email)
    {
        var user = new AppUser { Name = name, Email = email };
        await _unitOfWork.Repository<AppUser>().AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return user;
    }

    public async Task<AppUser?> UpdateAsync(int id, string name, string email)
    {
        var user = await _unitOfWork.Repository<AppUser>().GetByIdAsync(id);
        if (user is null) return null;

        user.Name = name;
        user.Email = email;
        _unitOfWork.Repository<AppUser>().Update(user);
        await _unitOfWork.SaveChangesAsync();
        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _unitOfWork.Repository<AppUser>().GetByIdAsync(id);
        if (user is null) return false;

        _unitOfWork.Repository<AppUser>().Remove(user);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}