using Microsoft.AspNetCore.Mvc;
using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;
using SmartTime.Web.DTOs;

namespace SmartTime.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public UserController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _unitOfWork.Repository<AppUser>().GetAllAsync();
        return Ok(users);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _unitOfWork.Repository<AppUser>().GetByIdAsync(id);
        if (user is null) return NotFound();
        return Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var user = new AppUser { Name = request.Name, Email = request.Email };
        await _unitOfWork.Repository<AppUser>().AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        var user = await _unitOfWork.Repository<AppUser>().GetByIdAsync(id);
        if (user is null) return NotFound();

        user.Name = request.Name;
        user.Email = request.Email;
        _unitOfWork.Repository<AppUser>().Update(user);
        await _unitOfWork.SaveChangesAsync();
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var user = await _unitOfWork.Repository<AppUser>().GetByIdAsync(id);
        if (user is null) return NotFound();

        _unitOfWork.Repository<AppUser>().Remove(user);
        await _unitOfWork.SaveChangesAsync();
        return NoContent();
    }
}