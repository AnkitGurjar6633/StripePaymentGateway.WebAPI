using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StripePaymentGateway.WebAPI.Models;

namespace StripePaymentGateway.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentGatewayController : ControllerBase
    {
        [HttpGet(Name = "PaymentLink")]
        public IActionResult Get()
        {
            return Ok();
        }

        [HttpPost(Name = "PaymentLinkPost")]
        [Consumes("application/json")]
        public IActionResult Post([FromBody]OrderDetails orderDetails)
        {
            return new JsonResult(new { data = "hello" });
        }
    }
}
