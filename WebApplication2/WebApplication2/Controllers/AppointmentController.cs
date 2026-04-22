using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var appointments = ServiceCollection.GetAllAppointments();
            return Ok();
        }
    }
}
