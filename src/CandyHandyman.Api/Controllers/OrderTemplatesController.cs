using System.Security.Claims;
using CandyHandyman.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderTemplatesController : ControllerBase
{
    private readonly IOrderTemplateService _templateService;

    public OrderTemplatesController(IOrderTemplateService templateService) => _templateService = templateService;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderTemplateDto dto)
    {
        var template = await _templateService.CreateAsync(GetUserId(), dto);
        return Ok(template);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyTemplates()
    {
        var templates = await _templateService.GetUserTemplatesAsync(GetUserId());
        return Ok(templates);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CreateOrderTemplateDto dto)
    {
        await _templateService.UpdateAsync(GetUserId(), id, dto);
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _templateService.DeleteAsync(GetUserId(), id);
        return Ok();
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
