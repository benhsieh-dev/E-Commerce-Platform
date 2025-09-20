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
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentDbContext _context;
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PaymentsController> _logger;

        public PaymentsController(
            PaymentDbContext context, 
            IPaymentProcessor paymentProcessor,
            IHttpClientFactory httpClientFactory,
            ILogger<PaymentsController> logger)
        {
            _context = context;
            _paymentProcessor = paymentProcessor;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<PaymentListResponseDto>> GetPayments(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? bookingId = null,
            [FromQuery] Guid? payerId = null,
            [FromQuery] Guid? payeeId = null,
            [FromQuery] PaymentStatus? status = null,
            [FromQuery] PaymentType? paymentType = null,
            [FromQuery] DateTime? createdFrom = null,
            [FromQuery] DateTime? createdTo = null)
        {
            var query = _context.Payments.Include(p => p.Transactions).AsQueryable();

            if (bookingId.HasValue)
                query = query.Where(p => p.BookingId == bookingId.Value);

            if (payerId.HasValue)
                query = query.Where(p => p.PayerId == payerId.Value);

            if (payeeId.HasValue)
                query = query.Where(p => p.PayeeId == payeeId.Value);

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            if (paymentType.HasValue)
                query = query.Where(p => p.PaymentType == paymentType.Value);

            if (createdFrom.HasValue)
                query = query.Where(p => p.CreatedAt >= createdFrom.Value);

            if (createdTo.HasValue)
                query = query.Where(p => p.CreatedAt <= createdTo.Value);

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var payments = await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new PaymentResponseDto
                {
                    Id = p.Id,
                    BookingId = p.BookingId,
                    PayerId = p.PayerId,
                    PayeeId = p.PayeeId,
                    Amount = p.Amount,
                    Currency = p.Currency,
                    PaymentType = p.PaymentType,
                    Status = p.Status,
                    PaymentMethod = p.PaymentMethod.Type,
                    ExternalTransactionId = p.ExternalTransactionId,
                    Description = p.Description,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    ProcessedAt = p.ProcessedAt,
                    RefundedAt = p.RefundedAt,
                    FailureReason = p.FailureReason,
                    RefundAmount = p.RefundAmount,
                    RefundReason = p.RefundReason,
                    PlatformFee = p.PlatformFee,
                    ProcessingFee = p.ProcessingFee,
                    HostAmount = p.HostAmount,
                    PropertyTitle = p.PropertyTitle,
                    BookingCheckIn = p.BookingCheckIn,
                    BookingCheckOut = p.BookingCheckOut,
                    Transactions = p.Transactions.Select(t => new PaymentTransactionDto
                    {
                        Id = t.Id,
                        Type = t.Type,
                        Amount = t.Amount,
                        Status = t.Status,
                        ExternalTransactionId = t.ExternalTransactionId,
                        Description = t.Description,
                        CreatedAt = t.CreatedAt,
                        ProcessedAt = t.ProcessedAt,
                        FailureReason = t.FailureReason
                    }).OrderBy(t => t.CreatedAt).ToList()
                })
                .ToListAsync();

            return Ok(new PaymentListResponseDto
            {
                Payments = payments,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentResponseDto>> GetPayment(Guid id)
        {
            var payment = await _context.Payments
                .Include(p => p.Transactions)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (payment == null)
                return NotFound();

            return Ok(new PaymentResponseDto
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                PayerId = payment.PayerId,
                PayeeId = payment.PayeeId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                PaymentType = payment.PaymentType,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod.Type,
                ExternalTransactionId = payment.ExternalTransactionId,
                Description = payment.Description,
                CreatedAt = payment.CreatedAt,
                UpdatedAt = payment.UpdatedAt,
                ProcessedAt = payment.ProcessedAt,
                RefundedAt = payment.RefundedAt,
                FailureReason = payment.FailureReason,
                RefundAmount = payment.RefundAmount,
                RefundReason = payment.RefundReason,
                PlatformFee = payment.PlatformFee,
                ProcessingFee = payment.ProcessingFee,
                HostAmount = payment.HostAmount,
                PropertyTitle = payment.PropertyTitle,
                BookingCheckIn = payment.BookingCheckIn,
                BookingCheckOut = payment.BookingCheckOut,
                Transactions = payment.Transactions.Select(t => new PaymentTransactionDto
                {
                    Id = t.Id,
                    Type = t.Type,
                    Amount = t.Amount,
                    Status = t.Status,
                    ExternalTransactionId = t.ExternalTransactionId,
                    Description = t.Description,
                    CreatedAt = t.CreatedAt,
                    ProcessedAt = t.ProcessedAt,
                    FailureReason = t.FailureReason
                }).OrderBy(t => t.CreatedAt).ToList()
            });
        }

        [HttpPost]
        public async Task<ActionResult<PaymentResponseDto>> CreatePayment(CreatePaymentDto createPaymentDto)
        {
            // Get booking details to validate and fetch host info
            var bookingClient = _httpClientFactory.CreateClient();
            var bookingResponse = await bookingClient.GetAsync($"http://localhost:5003/api/bookings/{createPaymentDto.BookingId}");
            
            if (!bookingResponse.IsSuccessStatusCode)
                return BadRequest("Booking not found.");

            // Calculate fees
            var platformFeeRate = 0.1m; // 10% platform fee
            var processingFeeRate = 0.029m; // 2.9% processing fee
            
            var platformFee = createPaymentDto.Amount * platformFeeRate;
            var processingFee = createPaymentDto.Amount * processingFeeRate;
            var hostAmount = createPaymentDto.Amount - platformFee - processingFee;

            var payment = new Payment
            {
                Id = Guid.NewGuid(),
                BookingId = createPaymentDto.BookingId,
                PayerId = createPaymentDto.PayerId,
                PayeeId = Guid.NewGuid(), // Should come from booking service
                Amount = createPaymentDto.Amount,
                Currency = createPaymentDto.Currency,
                PaymentType = createPaymentDto.PaymentType,
                Status = PaymentStatus.Pending,
                PaymentMethod = new PaymentMethod 
                {
                    Id = createPaymentDto.PaymentMethodId,
                    Type = PaymentMethodType.CreditCard, // Should come from payment method lookup
                    UserId = createPaymentDto.PayerId,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                Description = createPaymentDto.Description,
                PlatformFee = platformFee,
                ProcessingFee = processingFee,
                HostAmount = hostAmount,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                PropertyTitle = "Sample Property", // Should come from booking
                BookingCheckIn = DateTime.Today.AddDays(7), // Should come from booking
                BookingCheckOut = DateTime.Today.AddDays(10) // Should come from booking
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPayment), new { id = payment.Id }, new PaymentResponseDto
            {
                Id = payment.Id,
                BookingId = payment.BookingId,
                PayerId = payment.PayerId,
                PayeeId = payment.PayeeId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                PaymentType = payment.PaymentType,
                Status = payment.Status,
                PaymentMethod = payment.PaymentMethod.Type,
                Description = payment.Description,
                CreatedAt = payment.CreatedAt,
                UpdatedAt = payment.UpdatedAt,
                PlatformFee = payment.PlatformFee,
                ProcessingFee = payment.ProcessingFee,
                HostAmount = payment.HostAmount,
                PropertyTitle = payment.PropertyTitle,
                BookingCheckIn = payment.BookingCheckIn,
                BookingCheckOut = payment.BookingCheckOut,
                Transactions = new List<PaymentTransactionDto>()
            });
        }

        [HttpPost("{id}/process")]
        public async Task<IActionResult> ProcessPayment(Guid id, ProcessPaymentDto processPaymentDto)
        {
            var payment = await _context.Payments.Include(p => p.Transactions).FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null)
                return NotFound();

            if (payment.Status != PaymentStatus.Pending)
                return BadRequest("Only pending payments can be processed.");

            try
            {
                payment.Status = PaymentStatus.Processing;
                payment.UpdatedAt = DateTime.UtcNow;

                // Create payment intent if not provided
                string paymentIntentId;
                if (string.IsNullOrEmpty(processPaymentDto.PaymentIntentId))
                {
                    var paymentIntent = await _paymentProcessor.CreatePaymentIntentAsync(
                        payment.Amount, payment.Currency, payment.PayerId);
                    paymentIntentId = paymentIntent.PaymentIntentId;
                    payment.ExternalTransactionId = paymentIntentId;
                }
                else
                {
                    paymentIntentId = processPaymentDto.PaymentIntentId;
                }

                // Process the payment
                var success = await _paymentProcessor.ProcessPaymentAsync(paymentIntentId, payment.PaymentMethodId ?? "");

                if (success)
                {
                    payment.Status = PaymentStatus.Completed;
                    payment.ProcessedAt = DateTime.UtcNow;
                    
                    // Create successful transaction record
                    var transaction = new PaymentTransaction
                    {
                        Id = Guid.NewGuid(),
                        PaymentId = payment.Id,
                        Type = TransactionType.Charge,
                        Amount = payment.Amount,
                        Status = TransactionStatus.Completed,
                        ExternalTransactionId = paymentIntentId,
                        Description = "Payment processed successfully",
                        CreatedAt = DateTime.UtcNow,
                        ProcessedAt = DateTime.UtcNow
                    };
                    
                    _context.PaymentTransactions.Add(transaction);
                    _logger.LogInformation("Payment {PaymentId} processed successfully", payment.Id);
                }
                else
                {
                    payment.Status = PaymentStatus.Failed;
                    payment.FailureReason = "Payment processing failed";
                    
                    // Create failed transaction record
                    var transaction = new PaymentTransaction
                    {
                        Id = Guid.NewGuid(),
                        PaymentId = payment.Id,
                        Type = TransactionType.Charge,
                        Amount = payment.Amount,
                        Status = TransactionStatus.Failed,
                        ExternalTransactionId = paymentIntentId,
                        Description = "Payment processing failed",
                        FailureReason = "Payment declined",
                        CreatedAt = DateTime.UtcNow
                    };
                    
                    _context.PaymentTransactions.Add(transaction);
                    _logger.LogWarning("Payment {PaymentId} processing failed", payment.Id);
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment {PaymentId}", payment.Id);
                
                payment.Status = PaymentStatus.Failed;
                payment.FailureReason = "Internal processing error";
                payment.UpdatedAt = DateTime.UtcNow;
                
                await _context.SaveChangesAsync();
                return StatusCode(500, "Payment processing failed");
            }
        }

        [HttpPost("{id}/refund")]
        public async Task<IActionResult> RefundPayment(Guid id, RefundPaymentDto refundDto)
        {
            var payment = await _context.Payments.Include(p => p.Transactions).FirstOrDefaultAsync(p => p.Id == id);
            if (payment == null)
                return NotFound();

            if (payment.Status != PaymentStatus.Completed)
                return BadRequest("Only completed payments can be refunded.");

            if (refundDto.Amount > payment.Amount)
                return BadRequest("Refund amount cannot exceed the original payment amount.");

            var currentRefundTotal = payment.RefundAmount ?? 0;
            if (currentRefundTotal + refundDto.Amount > payment.Amount)
                return BadRequest("Total refund amount cannot exceed the original payment amount.");

            try
            {
                var success = await _paymentProcessor.RefundPaymentAsync(
                    payment.ExternalTransactionId ?? "", refundDto.Amount, refundDto.Reason);

                if (success)
                {
                    payment.RefundAmount = currentRefundTotal + refundDto.Amount;
                    payment.RefundReason = refundDto.Reason;
                    payment.RefundedAt = DateTime.UtcNow;
                    payment.UpdatedAt = DateTime.UtcNow;

                    // Update status based on refund amount
                    if (payment.RefundAmount >= payment.Amount)
                        payment.Status = PaymentStatus.Refunded;
                    else
                        payment.Status = PaymentStatus.PartiallyRefunded;

                    // Create refund transaction record
                    var transaction = new PaymentTransaction
                    {
                        Id = Guid.NewGuid(),
                        PaymentId = payment.Id,
                        Type = TransactionType.Refund,
                        Amount = refundDto.Amount,
                        Status = TransactionStatus.Completed,
                        Description = $"Refund: {refundDto.Reason}",
                        CreatedAt = DateTime.UtcNow,
                        ProcessedAt = DateTime.UtcNow
                    };

                    _context.PaymentTransactions.Add(transaction);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation("Refund of {Amount} processed for payment {PaymentId}", refundDto.Amount, payment.Id);
                    return NoContent();
                }
                else
                {
                    return BadRequest("Refund processing failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refund for payment {PaymentId}", payment.Id);
                return StatusCode(500, "Refund processing failed");
            }
        }

        [HttpPost("create-intent")]
        public async Task<ActionResult<PaymentIntentDto>> CreatePaymentIntent(CreatePaymentDto createPaymentDto)
        {
            try
            {
                var paymentIntent = await _paymentProcessor.CreatePaymentIntentAsync(
                    createPaymentDto.Amount, createPaymentDto.Currency, createPaymentDto.PayerId);
                
                return Ok(paymentIntent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating payment intent");
                return StatusCode(500, "Failed to create payment intent");
            }
        }

        [HttpGet("summary")]
        public async Task<ActionResult<PaymentSummaryDto>> GetPaymentSummary(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            var start = startDate ?? DateTime.UtcNow.AddDays(-30);
            var end = endDate ?? DateTime.UtcNow;

            var payments = await _context.Payments
                .Where(p => p.CreatedAt >= start && p.CreatedAt <= end)
                .ToListAsync();

            var summary = new PaymentSummaryDto
            {
                TotalRevenue = payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.Amount),
                TotalRefunds = payments.Sum(p => p.RefundAmount ?? 0),
                PlatformFees = payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.PlatformFee ?? 0),
                HostPayouts = payments.Where(p => p.Status == PaymentStatus.Completed).Sum(p => p.HostAmount ?? 0),
                TotalTransactions = payments.Count,
                SuccessfulPayments = payments.Count(p => p.Status == PaymentStatus.Completed),
                FailedPayments = payments.Count(p => p.Status == PaymentStatus.Failed),
                RefundCount = payments.Count(p => p.RefundAmount > 0),
                PeriodStart = start,
                PeriodEnd = end
            };

            return Ok(summary);
        }

        [HttpPost("host-payout")]
        public async Task<IActionResult> ProcessHostPayout(HostPayoutDto hostPayoutDto)
        {
            try
            {
                var success = await _paymentProcessor.TransferToHostAsync(
                    hostPayoutDto.HostId, hostPayoutDto.Amount, hostPayoutDto.Description ?? "Host payout");

                if (success)
                {
                    _logger.LogInformation("Host payout of {Amount} processed for host {HostId}", 
                        hostPayoutDto.Amount, hostPayoutDto.HostId);
                    return NoContent();
                }
                else
                {
                    return BadRequest("Host payout processing failed");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing host payout for host {HostId}", hostPayoutDto.HostId);
                return StatusCode(500, "Host payout processing failed");
            }
        }
    }
}