using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;
using SmartTime.Services.Interfaces;

namespace SmartTime.Services.Services;

public class ProjectService : IProjectService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<Project>> GetAllAsync() =>
        await _unitOfWork.Repository<Project>().GetAllAsync();

    public async Task<Project?> GetByIdAsync(int id) =>
        await _unitOfWork.Repository<Project>().GetByIdAsync(id);

    public async Task<Project> CreateAsync(string name)
    {
        var project = new Project { Name = name };
        await _unitOfWork.Repository<Project>().AddAsync(project);
        await _unitOfWork.SaveChangesAsync();
        return project;
    }

    public async Task<Project?> UpdateAsync(int id, string name)
    {
        var project = await _unitOfWork.Repository<Project>().GetByIdAsync(id);
        if (project is null) return null;

        project.Name = name;
        _unitOfWork.Repository<Project>().Update(project);
        await _unitOfWork.SaveChangesAsync();
        return project;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var project = await _unitOfWork.Repository<Project>().GetByIdAsync(id);
        if (project is null) return false;

        _unitOfWork.Repository<Project>().Remove(project);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}