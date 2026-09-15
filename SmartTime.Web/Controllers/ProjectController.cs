using Microsoft.AspNetCore.Mvc;
using SmartTime.Repository.Entities;
using SmartTime.Repository.Interfaces;
using SmartTime.Web.DTOs;

namespace SmartTime.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public ProjectController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var projects = await _unitOfWork.Repository<Project>().GetAllAsync();
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var project = await _unitOfWork.Repository<Project>().GetByIdAsync(id);
        if (project is null) return NotFound();
        return Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectRequest request)
    {
        var project = new Project { Name = request.Name };
        await _unitOfWork.Repository<Project>().AddAsync(project);
        await _unitOfWork.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProjectRequest request)
    {
        var project = await _unitOfWork.Repository<Project>().GetByIdAsync(id);
        if (project is null) return NotFound();

        project.Name = request.Name;
        _unitOfWork.Repository<Project>().Update(project);
        await _unitOfWork.SaveChangesAsync();
        return Ok(project);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await _unitOfWork.Repository<Project>().GetByIdAsync(id);
        if (project is null) return NotFound();

        _unitOfWork.Repository<Project>().Remove(project);
        await _unitOfWork.SaveChangesAsync();
        return NoContent();
    }
}