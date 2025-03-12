using Microsoft.AspNetCore.Mvc;
using TestMcSonae.DTOs;
using TestMcSonae.Extensions;
using TestMcSonae.Mapping;
using TestMcSonae.Models;
using TestMcSonae.Services;
using TestMcSonae.Validation.Validators;

namespace TestMcSonae.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly OrderMapper _orderMapper;
        private readonly CreateOrderValidator _createOrderValidator;
        
        public OrdersController(
            IOrderService orderService, 
            OrderMapper orderMapper,
            CreateOrderValidator createOrderValidator)
        {
            _orderService = orderService;
            _orderMapper = orderMapper;
            _createOrderValidator = createOrderValidator;
        }
        
        [HttpGet]
        public ActionResult<ApiResponse<IEnumerable<OrderDTO>>> GetAll()
        {
            var orders = _orderService.GetAllOrders();
            var orderDTOs = orders.Select(_orderMapper.MapToDTO).ToList();
            return this.ApiResponse(ApiResponse<IEnumerable<OrderDTO>>.Create(true, orderDTOs));
        }
        
        [HttpGet("{id}")]
        public ActionResult<ApiResponse<OrderDTO>> GetById(Guid id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
            {
                return this.ApiResponse(ApiResponse<OrderDTO>.Create(false, default, $"Order with ID {id} not found", 404));
            }
            
            return this.ApiResponse(ApiResponse<OrderDTO>.Create(true, _orderMapper.MapToDTO(order)));
        }
        
        [HttpPut("{id}/confirm")]
        public ActionResult<ApiResponse<OrderDTO>> ConfirmOrder(Guid id)
        {
            string errorMessage;
            var order = _orderService.ConfirmOrder(id, out errorMessage);
            
            if (order != null)
            {
                return this.ApiResponse(ApiResponse<OrderDTO>.Create(true, _orderMapper.MapToDTO(order), $"Order {id} confirmed successfully"));
            }
            
            int statusCode = 400;
            if (errorMessage.Contains("not found"))
                statusCode = 404;
            else if (errorMessage.Contains("cannot be confirmed"))
                statusCode = 409;
                
            return this.ApiResponse(ApiResponse<OrderDTO>.Create(false, default, errorMessage, statusCode));
        }
        
        [HttpPut("{id}/cancel")]
        public ActionResult<ApiResponse<OrderDTO>> CancelOrder(Guid id)
        {
            string errorMessage;
            var order = _orderService.CancelOrder(id, out errorMessage);
            
            if (order != null)
            {
                return this.ApiResponse(ApiResponse<OrderDTO>.Create(true, _orderMapper.MapToDTO(order), $"Order {id} cancelled successfully"));
            }
            
            int statusCode = 400;
            if (errorMessage.Contains("not found"))
                statusCode = 404;
            else if (errorMessage.Contains("cannot be cancelled"))
                statusCode = 409;
                
            return this.ApiResponse(ApiResponse<OrderDTO>.Create(false, default, errorMessage, statusCode));
        }
        
        [HttpPost]
        public ActionResult<ApiResponse<OrderDTO>> Create([FromBody] CreateOrderDTO createOrderDTO)
        {
            var validationResult = _createOrderValidator.Validate(createOrderDTO);
            if (!validationResult.IsValid)
            {
                return this.ApiResponse(ApiResponse<OrderDTO>.Create(false, default, validationResult.Errors.First(), 400));
            }
            
            var orderItems = createOrderDTO.Items.Select(item => new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = item.ProductId,
                Quantity = item.Quantity
            }).ToList();
            
            string errorMessage;
            var order = _orderService.CreateOrder(orderItems, out errorMessage);
            
            if (order != null)
            {
                var orderDto = _orderMapper.MapToDTO(order);
                return this.ApiResponse(ApiResponse<OrderDTO>.Create(true, orderDto, "Order created successfully", 201));
            }
            
            return this.ApiResponse(ApiResponse<OrderDTO>.Create(false, default, errorMessage, 400));
        }
    }
}