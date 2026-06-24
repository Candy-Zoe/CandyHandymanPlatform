using CandyHandyman.Application.DTOs;
using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IRepository<SkillCategory> _categoryRepo;
    private readonly IRepository<Service> _serviceRepo;

    public CategoriesController(IRepository<SkillCategory> categoryRepo, IRepository<Service> serviceRepo)
    {
        _categoryRepo = categoryRepo;
        _serviceRepo = serviceRepo;
    }

    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll()
    {
        var categories = (await _categoryRepo.GetAllAsync()).OrderBy(c => c.SortOrder).ToList();
        var tree = BuildTree(categories, null);
        return Ok(tree);
    }

    [HttpGet("{id}/services")]
    public async Task<ActionResult<List<ServiceDto>>> GetByCategoryId(Guid id)
    {
        var services = (await _serviceRepo.GetAllAsync())
            .Where(s => s.CategoryId == id)
            .OrderByDescending(s => s.CreatedAt)
            .Select(s => new ServiceDto
            {
                Id = s.Id,
                Title = s.Title,
                Description = s.Description,
                Price = s.Price,
                Unit = s.Unit,
                PricingType = s.PricingType,
                CategoryId = s.CategoryId,
                CreatedAt = s.CreatedAt
            })
            .ToList();

        return Ok(services);
    }

    private static List<CategoryDto> BuildTree(List<SkillCategory> all, Guid? parentId)
    {
        return all
            .Where(c => c.ParentId == parentId)
            .Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Icon = c.Icon,
                ParentId = c.ParentId,
                SortOrder = c.SortOrder,
                Children = BuildTree(all, c.Id)
            })
            .ToList();
    }
}