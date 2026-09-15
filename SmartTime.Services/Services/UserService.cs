using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;
using SmartTime.Services.Interfaces;

namespace SmartTime.Services.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClockifyService _clockify;

    public UserService(IUnitOfWork unitOfWork, IClockifyService clockify)
    {
        _unitOfWork = unitOfWork;
        _clockify = clockify;
    }

    public async Task<IReadOnlyList<AppUser>> GetAllAsync() =>
        await _unitOfWork.Repository<AppUser>().GetAllAsync();

    public async Task<AppUser?> GetByIdAsync(int id) =>
        await _unitOfWork.Repository<AppUser>().GetByIdAsync(id);

    public async Task<AppUser> CreateAsync(string name, string email)
    {
        // Clockify can't create new users via API - only find existing workspace members.
        // So "create" succeeds only if this email already belongs to a Clockify user.
        var clockifyUserId = await _clockify.FindUserIdByEmailAsync(email);
        if (clockifyUserId is null)
        {
            throw new InvalidOperationException(
                $"No Clockify user found with email '{email}'. They must already be a member of the Clockify workspace.");
        }

        var user = new AppUser { Name = name, Email = email, ClockifyUserId = clockifyUserId };
        await _unitOfWork.Repository<AppUser>().AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return user;
    }

    public async Task<AppUser?> UpdateAsync(int id, string name, string email)
    {
        var user = await _unitOfWork.Repository<AppUser>().GetByIdAsync(id);
        if (user is null) return null;

        // Re-verify the (possibly new) email against Clockify before saving locally.
        var clockifyUserId = await _clockify.FindUserIdByEmailAsync(email);
        if (clockifyUserId is null)
        {
            throw new InvalidOperationException(
                $"No Clockify user found with email '{email}'. Cannot update.");
        }

        user.Name = name;
        user.Email = email;
        user.ClockifyUserId = clockifyUserId;
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