using System.Security.Claims;
using CandyHandyman.Application.DTOs;
using CandyHandyman.Application.Interfaces;
using CandyHandyman.Core.Entities;
using CandyHandyman.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CandyHandyman.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IRepository<Order> _orderRepo;
    private readonly IRepository<Service> _serviceRepo;
    private readonly IRepository<User> _userRepo;
    private readonly INotificationService _notificationService;

    public OrdersController(
        IRepository<Order> orderRepo,
        IRepository<Service> serviceRepo,
        IRepository<User> userRepo,
        INotificationService notificationService)
    {
        _orderRepo = orderRepo;
        _serviceRepo = serviceRepo;
        _userRepo = userRepo;
        _notificationService = notificationService;
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderDto dto)
    {
        var userId = GetUserId();
        var service = await _serviceRepo.GetByIdAsync(dto.ServiceId);
        if (service == null) return NotFound(new { message = "服务不存在" });

        var providerUser = await _userRepo.GetByIdAsync(service.HandymanProfile.UserId);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            OrderNo = $"ORD{DateTime.UtcNow:yyyyMMddHHmmss}{Guid.NewGuid().ToString("N")[..8]}",
            ServiceId = dto.ServiceId,
            CustomerId = userId,
            ProviderId = providerUser!.Id,
            Quantity = dto.Quantity,
            UnitPrice = service.Price,
            TotalAmount = service.Price * dto.Quantity,
            Address = dto.Address,
            ContactPhone = dto.ContactPhone,
            Description = dto.Description,
            ScheduledAt = dto.ScheduledAt
        };

        await _orderRepo.AddAsync(order);

        await _notificationService.SendNotificationAsync(
            providerUser.Id,
            "新订单",
            $"您有一个新订单：{service.Title}",
            NotificationType.OrderUpdate,
            order.Id,
            "Order");

        return Ok(MapToDto(order, service, providerUser, await _userRepo.GetByIdAsync(userId)));
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll([FromQuery] OrderStatus? status)
    {
        var userId = GetUserId();
        var orders = (await _orderRepo.GetAllAsync())
            .Where(o => o.CustomerId == userId || o.ProviderId == userId)
            .AsQueryable();

        if (status.HasValue)
            orders = orders.Where(o => o.Status == status);

        var services = (await _serviceRepo.GetAllAsync()).ToDictionary(s => s.Id);
        var users = (await _userRepo.GetAllAsync()).ToDictionary(u => u.Id);

        var ordered = orders.OrderByDescending(o => o.CreatedAt).ToList();
        var result = new List<OrderDto>();
        foreach (var o in ordered)
        {
            services.TryGetValue(o.ServiceId, out var svc);
            users.TryGetValue(o.ProviderId, out var prov);
            users.TryGetValue(o.CustomerId, out var cust);
            result.Add(MapToDto(o, svc, prov, cust));
        }
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<OrderDto>> GetById(Guid id)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null) return NotFound();

        var service = await _serviceRepo.GetByIdAsync(order.ServiceId);
        var provider = await _userRepo.GetByIdAsync(order.ProviderId);
        var customer = await _userRepo.GetByIdAsync(order.CustomerId);

        return Ok(MapToDto(order, service, provider, customer));
    }

    [HttpPut("{id}/accept")]
    public async Task<IActionResult> Accept(Guid id)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null) return NotFound();
        if (order.Status != OrderStatus.Pending) return BadRequest(new { message = "订单状态不允许" });

        order.Status = OrderStatus.Accepted;
        order.AcceptedAt = DateTime.UtcNow;
        await _orderRepo.UpdateAsync(order);

        await _notificationService.SendNotificationAsync(
            order.CustomerId,
            "订单已接单",
            "工匠已接受您的订单，请等待服务",
            NotificationType.OrderUpdate,
            order.Id,
            "Order");

        return Ok();
    }

    [HttpPut("{id}/start")]
    public async Task<IActionResult> Start(Guid id)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null) return NotFound();
        if (order.Status != OrderStatus.Accepted) return BadRequest(new { message = "订单状态不允许" });

        order.Status = OrderStatus.InProgress;
        await _orderRepo.UpdateAsync(order);

        await _notificationService.SendNotificationAsync(
            order.CustomerId,
            "服务开始",
            "工匠已开始为您服务",
            NotificationType.OrderUpdate,
            order.Id,
            "Order");

        return Ok();
    }

    [HttpPut("{id}/complete")]
    public async Task<IActionResult> Complete(Guid id)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null) return NotFound();
        if (order.Status != OrderStatus.InProgress) return BadRequest(new { message = "订单状态不允许" });

        order.Status = OrderStatus.Completed;
        order.CompletedAt = DateTime.UtcNow;
        await _orderRepo.UpdateAsync(order);

        await _notificationService.SendNotificationAsync(
            order.CustomerId,
            "订单已完成",
            "服务已完成，请对工匠进行评价",
            NotificationType.OrderUpdate,
            order.Id,
            "Order");

        return Ok();
    }

    [HttpPut("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var order = await _orderRepo.GetByIdAsync(id);
        if (order == null) return NotFound();
        if (order.Status == OrderStatus.Completed || order.Status == OrderStatus.Cancelled)
            return BadRequest(new { message = "订单状态不允许" });

        var cancelledBy = GetUserId();
        var notifyTo = cancelledBy == order.CustomerId ? order.ProviderId : order.CustomerId;

        order.Status = OrderStatus.Cancelled;
        await _orderRepo.UpdateAsync(order);

        await _notificationService.SendNotificationAsync(
            notifyTo,
            "订单已取消",
            "订单已被取消",
            NotificationType.OrderUpdate,
            order.Id,
            "Order");

        return Ok();
    }

    private Guid GetUserId() => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static OrderDto MapToDto(Order o, Service? service, User? provider, User? customer) => new()
    {
        Id = o.Id,
        OrderNo = o.OrderNo,
        Service = service != null ? new ServiceDto
        {
            Id = service.Id,
            Title = service.Title,
            Price = service.Price,
            Unit = service.Unit
        } : null,
        Provider = provider != null ? new HandymanDto
        {
            Id = provider.Id,
            Nickname = provider.Nickname,
            Avatar = provider.Avatar
        } : null,
        Customer = customer != null ? new UserDto
        {
            Id = customer.Id,
            Nickname = customer.Nickname,
            Phone = customer.Phone,
            Avatar = customer.Avatar,
            Role = customer.Role.ToString(),
            Balance = customer.Balance
        } : null,
        Quantity = o.Quantity,
        UnitPrice = o.UnitPrice,
        TotalAmount = o.TotalAmount,
        Status = o.Status,
        Address = o.Address,
        ContactPhone = o.ContactPhone,
        Description = o.Description,
        ScheduledAt = o.ScheduledAt,
        CreatedAt = o.CreatedAt,
        CompletedAt = o.CompletedAt
    };
}