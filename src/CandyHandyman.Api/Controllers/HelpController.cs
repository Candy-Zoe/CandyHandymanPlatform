using CandyHandyman.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelpController : ControllerBase
{
    private readonly IHelpService _helpService;

    public HelpController(IHelpService helpService) => _helpService = helpService;

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll()
    {
        var topics = await _helpService.GetAllAsync();
        return Ok(topics);
    }

    [HttpGet("category/{category}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByCategory(string category)
    {
        var topics = await _helpService.GetByCategoryAsync(category);
        return Ok(topics);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id)
    {
        var topic = await _helpService.GetByIdAsync(id);
        if (topic == null) return NotFound();
        return Ok(topic);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateHelpTopicDto dto)
    {
        var topic = await _helpService.CreateAsync(dto);
        return Ok(topic);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateHelpTopicDto dto)
    {
        await _helpService.UpdateAsync(id, dto);
        return Ok();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _helpService.DeleteAsync(id);
        return Ok();
    }
}
