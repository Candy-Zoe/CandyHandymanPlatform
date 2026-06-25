using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CandyHandyman.Infrastructure.Services;

public class OrderTemplateService : IOrderTemplateService
{
    private readonly AppDbContext _context;

    public OrderTemplateService(AppDbContext context) => _context = context;

    public async Task<OrderTemplateDto> CreateAsync(Guid userId, CreateOrderTemplateDto dto)
    {
        if (dto.IsDefault)
        {
            var defaults = await _context.OrderTemplates.Where(t => t.UserId == userId && t.IsDefault).ToListAsync();
            foreach (var d in defaults) d.IsDefault = false;
        }

        var template = new OrderTemplate
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = dto.Name,
            Address = dto.Address,
            ContactPhone = dto.ContactPhone,
            Description = dto.Description,
            IsDefault = dto.IsDefault
        };

        _context.OrderTemplates.Add(template);
        await _context.SaveChangesAsync();

        return MapToDto(template);
    }

    public async Task<List<OrderTemplateDto>> GetUserTemplatesAsync(Guid userId)
    {
        return await _context.OrderTemplates
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.IsDefault).ThenByDescending(t => t.CreatedAt)
            .Select(t => MapToDto(t))
            .ToListAsync();
    }

    public async Task UpdateAsync(Guid userId, Guid templateId, CreateOrderTemplateDto dto)
    {
        var template = await _context.OrderTemplates.FirstOrDefaultAsync(t => t.Id == templateId && t.UserId == userId);
        if (template == null) return;

        if (dto.IsDefault)
        {
            var defaults = await _context.OrderTemplates.Where(t => t.UserId == userId && t.IsDefault).ToListAsync();
            foreach (var d in defaults) d.IsDefault = false;
        }

        template.Name = dto.Name;
        template.Address = dto.Address;
        template.ContactPhone = dto.ContactPhone;
        template.Description = dto.Description;
        template.IsDefault = dto.IsDefault;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid userId, Guid templateId)
    {
        var template = await _context.OrderTemplates.FirstOrDefaultAsync(t => t.Id == templateId && t.UserId == userId);
        if (template == null) return;
        _context.OrderTemplates.Remove(template);
        await _context.SaveChangesAsync();
    }

    private static OrderTemplateDto MapToDto(OrderTemplate t) => new()
    {
        Id = t.Id,
        Name = t.Name,
        Address = t.Address,
        ContactPhone = t.ContactPhone,
        Description = t.Description,
        IsDefault = t.IsDefault
    };
}
