using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PaymentService.Data;
using PaymentService.DTOs;
using PaymentService.Models;
using PaymentService.Services;

namespace PaymentService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly PaymentDbContext _context;
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly ILogger<PaymentMethodsController> _logger;

        public PaymentMethodsController(
            PaymentDbContext context, 
            IPaymentProcessor paymentProcessor,
            ILogger<PaymentMethodsController> logger)
        {
            _context = context;
            _paymentProcessor = paymentProcessor;
            _logger = logger;
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<List<PaymentMethodResponseDto>>> GetUserPaymentMethods(Guid userId)
        {
            var paymentMethods = await _context.PaymentMethods
                .Where(pm => pm.UserId == userId && pm.IsActive)
                .OrderByDescending(pm => pm.IsDefault)
                .ThenByDescending(pm => pm.CreatedAt)
                .Select(pm => new PaymentMethodResponseDto
                {
                    Id = pm.Id,
                    UserId = pm.UserId,
                    Type = pm.Type,
                    CardLast4 = pm.CardLast4,
                    CardBrand = pm.CardBrand,
                    CardExpMonth = pm.CardExpMonth,
                    CardExpYear = pm.CardExpYear,
                    BankLast4 = pm.BankLast4,
                    BankName = pm.BankName,
                    IsDefault = pm.IsDefault,
                    IsActive = pm.IsActive,
                    CreatedAt = pm.CreatedAt,
                    UpdatedAt = pm.UpdatedAt
                })
                .ToListAsync();

            return Ok(paymentMethods);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentMethodResponseDto>> GetPaymentMethod(Guid id)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);

            if (paymentMethod == null || !paymentMethod.IsActive)
                return NotFound();

            return Ok(new PaymentMethodResponseDto
            {
                Id = paymentMethod.Id,
                UserId = paymentMethod.UserId,
                Type = paymentMethod.Type,
                CardLast4 = paymentMethod.CardLast4,
                CardBrand = paymentMethod.CardBrand,
                CardExpMonth = paymentMethod.CardExpMonth,
                CardExpYear = paymentMethod.CardExpYear,
                BankLast4 = paymentMethod.BankLast4,
                BankName = paymentMethod.BankName,
                IsDefault = paymentMethod.IsDefault,
                IsActive = paymentMethod.IsActive,
                CreatedAt = paymentMethod.CreatedAt,
                UpdatedAt = paymentMethod.UpdatedAt
            });
        }

        [HttpPost]
        public async Task<ActionResult<PaymentMethodResponseDto>> CreatePaymentMethod(CreatePaymentMethodDto createPaymentMethodDto)
        {
            try
            {
                // Create payment method with external processor (e.g., Stripe)
                var externalPaymentMethod = await _paymentProcessor.CreatePaymentMethodAsync(createPaymentMethodDto);

                // If this is set as default, update other payment methods for this user
                if (createPaymentMethodDto.IsDefault)
                {
                    var existingPaymentMethods = await _context.PaymentMethods
                        .Where(pm => pm.UserId == createPaymentMethodDto.UserId && pm.IsActive)
                        .ToListAsync();

                    foreach (var pm in existingPaymentMethods)
                    {
                        pm.IsDefault = false;
                        pm.UpdatedAt = DateTime.UtcNow;
                    }
                }

                var paymentMethod = new PaymentMethod
                {
                    Id = Guid.NewGuid(),
                    UserId = createPaymentMethodDto.UserId,
                    Type = createPaymentMethodDto.Type,
                    StripePaymentMethodId = createPaymentMethodDto.StripePaymentMethodId,
                    CardLast4 = externalPaymentMethod.CardLast4,
                    CardBrand = externalPaymentMethod.CardBrand,
                    CardExpMonth = externalPaymentMethod.CardExpMonth,
                    CardExpYear = externalPaymentMethod.CardExpYear,
                    BankLast4 = externalPaymentMethod.BankLast4,
                    BankName = externalPaymentMethod.BankName,
                    IsDefault = createPaymentMethodDto.IsDefault,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.PaymentMethods.Add(paymentMethod);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created payment method {PaymentMethodId} for user {UserId}", 
                    paymentMethod.Id, paymentMethod.UserId);

                return CreatedAtAction(nameof(GetPaymentMethod), new { id = paymentMethod.Id }, new PaymentMethodResponseDto
                {
                    Id = paymentMethod.Id,
                    UserId = paymentMethod.UserId,
                    Type = paymentMethod.Type,
                    CardLast4 = paymentMethod.CardLast4,
                    CardBrand = paymentMethod.CardBrand,
                    CardExpMonth = paymentMethod.CardExpMonth,
                    CardExpYear = paymentMethod.CardExpYear,
                    BankLast4 = paymentMethod.BankLast4,
                    BankName = paymentMethod.BankName,
                    IsDefault = paymentMethod.IsDefault,
                    IsActive = paymentMethod.IsActive,
                    CreatedAt = paymentMethod.CreatedAt,
                    UpdatedAt = paymentMethod.UpdatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment method for user {UserId}", createPaymentMethodDto.UserId);
                return StatusCode(500, "Failed to create payment method");
            }
        }

        [HttpPut("{id}/set-default")]
        public async Task<IActionResult> SetAsDefault(Guid id)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);
            if (paymentMethod == null || !paymentMethod.IsActive)
                return NotFound();

            // Remove default from other payment methods for this user
            var otherPaymentMethods = await _context.PaymentMethods
                .Where(pm => pm.UserId == paymentMethod.UserId && pm.Id != id && pm.IsActive)
                .ToListAsync();

            foreach (var pm in otherPaymentMethods)
            {
                pm.IsDefault = false;
                pm.UpdatedAt = DateTime.UtcNow;
            }

            // Set this payment method as default
            paymentMethod.IsDefault = true;
            paymentMethod.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Set payment method {PaymentMethodId} as default for user {UserId}", 
                paymentMethod.Id, paymentMethod.UserId);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePaymentMethod(Guid id)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(id);
            if (paymentMethod == null || !paymentMethod.IsActive)
                return NotFound();

            try
            {
                // Delete from external processor if applicable
                if (!string.IsNullOrEmpty(paymentMethod.StripePaymentMethodId))
                {
                    await _paymentProcessor.DeletePaymentMethodAsync(paymentMethod.StripePaymentMethodId);
                }

                // Soft delete - mark as inactive
                paymentMethod.IsActive = false;
                paymentMethod.IsDefault = false;
                paymentMethod.UpdatedAt = DateTime.UtcNow;

                // If this was the default payment method, set another one as default
                if (paymentMethod.IsDefault)
                {
                    var nextDefault = await _context.PaymentMethods
                        .Where(pm => pm.UserId == paymentMethod.UserId && pm.IsActive && pm.Id != id)
                        .OrderByDescending(pm => pm.CreatedAt)
                        .FirstOrDefaultAsync();

                    if (nextDefault != null)
                    {
                        nextDefault.IsDefault = true;
                        nextDefault.UpdatedAt = DateTime.UtcNow;
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation("Deleted payment method {PaymentMethodId} for user {UserId}", 
                    paymentMethod.Id, paymentMethod.UserId);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting payment method {PaymentMethodId}", id);
                return StatusCode(500, "Failed to delete payment method");
            }
        }
    }
}