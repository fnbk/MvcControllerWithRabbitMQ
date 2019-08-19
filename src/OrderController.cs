using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MvcControllerWithRabbitMQ
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        // GET: api/order
        [HttpGet]
        public async Task<ActionResult> AcceptOrder([FromServices] RabbitProducer client)
        {
            string correlationId = System.Guid.NewGuid().ToString();

            var message = new
            {
                correlationId,
                florian = "hello world"
            };
            client.PushMessage( message);
            
            return new OkObjectResult(new
            {
                correlationId
            });
        }
    }
}
