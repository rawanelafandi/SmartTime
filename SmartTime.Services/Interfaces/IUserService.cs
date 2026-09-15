using SmartTime.Repository.Entities;

namespace SmartTime.Services.Interfaces;

public interface IUserService
{
    Task<IReadOnlyList<AppUser>> GetAllAsync();
    Task<AppUser?> GetByIdAsync(int id);
    Task<AppUser> CreateAsync(string name, string email);
    Task<AppUser?> UpdateAsync(int id, string name, string email);
    Task<bool> DeleteAsync(int id);
}