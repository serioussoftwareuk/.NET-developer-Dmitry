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
        
        [HttpGet("calculate/{symbol}")]
        public Task<CalculationResult> CalculatePerformanceAsync(string symbol)
        {
            return _service.CalculateAsync(symbol);
        }
    }
}
