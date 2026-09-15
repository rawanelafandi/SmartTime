using SmartTime.Repository.Entities;

namespace SmartTime.Services.Interfaces;

public interface IProjectService
{
    Task<IReadOnlyList<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(int id);
    Task<Project> CreateAsync(string name);
    Task<Project?> UpdateAsync(int id, string name);
    Task<bool> DeleteAsync(int id);
}