using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;
using StripePaymentGateway.WebAPI.DatabaseContext;
using StripePaymentGateway.WebAPI.Models;

namespace StripePaymentGateway.WebAPI.Controllers
{
    
    [ApiController]
    public class PaymentGatewayController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        public PaymentGatewayController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("api/[action]")]
        public async Task<IActionResult> GetPaymentLink([FromBody]PaymentLinkRequestDTO request)
        {
            var options = CreateSessionOptions(request);
            var service = new SessionService();

            try
            {
                var session = await service.CreateAsync(options);

                if(_dbContext.OrderDetails.Any(o => o.OrderID == request.OrderID))
                {
                    return StatusCode(409, "Duplicate order: An order with this ID has already been created.");
                }

                var orderDetails = new OrderDetails
                {
                    Amount = request.Amount,
                    OrderID = request.OrderID,
                    CustomerName = request.CustomerName,
                    Currency = request.Currency,
                    Email = request.Email,
                    Mobile = request.Mobile,
                    SessionID = session.Id
                };

                _dbContext.OrderDetails.Add(orderDetails);
                await _dbContext.SaveChangesAsync();

                return Ok(new { url = session.Url });
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Consumes("application/json")]
        [Produces("application/json")]
        [Route("api/[action]")]
        public async Task<IActionResult> CheckPaymentStatus([FromBody]CheckPaymentStatusRequestDTO request)
        {
            try
            {
                var orderDetailsFromDb = await _dbContext.OrderDetails.FindAsync(request.OrderID);

                var service = new SessionService();
                var session = await service.GetAsync(orderDetailsFromDb?.SessionID);
                return Ok(new { orderID = request.OrderID, status = session.Status, paymentAmount = (decimal)session.AmountTotal / 100 });
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        private SessionCreateOptions CreateSessionOptions (PaymentLinkRequestDTO paymentLinkRequestDTO)
        {
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",
                CustomerEmail = paymentLinkRequestDTO.Email,
                ClientReferenceId = paymentLinkRequestDTO.OrderID,
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = paymentLinkRequestDTO.Currency.ToLower(),
                            UnitAmount = (long)(paymentLinkRequestDTO.Amount * 100),
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = paymentLinkRequestDTO.OrderID
                            }
                        },
                        Quantity = 1
                    }
                },
                CustomFields = new List<SessionCustomFieldOptions>
                {
                      new SessionCustomFieldOptions
                      {
                          Key = "customerName",
                          Label = new SessionCustomFieldLabelOptions
                          {
                              Type = "custom",
                              Custom = "Customer Name"
                          },
                          Type = "text",
                          Text = new SessionCustomFieldTextOptions
                          {
                              DefaultValue = paymentLinkRequestDTO.CustomerName
                          },
                          Optional = false
                      },
                      new SessionCustomFieldOptions
                      {
                          Key = "mobile",
                          Label = new SessionCustomFieldLabelOptions
                          {
                              Type = "custom",
                              Custom = "Mobile"
                          },
                          Type = "numeric",
                          Numeric = new SessionCustomFieldNumericOptions
                          {
                              DefaultValue = paymentLinkRequestDTO.Mobile,
                              MinimumLength = 10,
                              MaximumLength = 10
                          },
                          Optional = false
                      }
                },
                SuccessUrl = "https://www.example.com",
                PaymentIntentData = new SessionPaymentIntentDataOptions
                {
                    ReceiptEmail = paymentLinkRequestDTO.Email,
                }
            };
            return options;
        }
    }
}
