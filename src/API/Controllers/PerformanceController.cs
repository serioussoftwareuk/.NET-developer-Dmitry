using System.Threading.Tasks;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PerformanceController : ControllerBase
    {
        private readonly IPerformanceService _service;

        public PerformanceController(IPerformanceService service)
        {
            _service = service;
        }
        
        [HttpGet("calculate")]
        public Task<CalculationResult> CalculatePerformanceAsync([FromQuery]string[] symbols)
        {
            return _service.CalculateAsync(symbols);
        }
    }
}
