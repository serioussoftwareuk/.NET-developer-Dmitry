using System.Threading.Tasks;
using API.Models;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public interface IPerformanceService
    {
        Task<CalculationResult> CalculateAsync(string symbol);
    }
    
    public class PerformanceService : IPerformanceService
    {
        private readonly IQuoteService _quoteService;
        private readonly IPerformanceCalculator _performanceCalculator;
        private readonly BaseConfiguration _config;

        public PerformanceService(
            IQuoteService quoteService,
            IPerformanceCalculator performanceCalculator,
            IOptions<BaseConfiguration> config)
        {
            _quoteService = quoteService;
            _performanceCalculator = performanceCalculator;
            _config = config?.Value;
        }
        
        public async Task<CalculationResult> CalculateAsync(string symbol)
        {
            var calculateBaseTask = CalculateAsync(symbol, QuoteType.Base);
            var calculateSPTask = CalculateAsync(_config.SP, QuoteType.SP);

            await Task.WhenAll(calculateBaseTask, calculateSPTask);

            return new CalculationResult
            {
                Base = await calculateBaseTask,
                SP = await calculateSPTask
            };
        }

        private async Task<CalculationModel> CalculateAsync(string symbol, QuoteType type)
        {
            var quotes = await _quoteService.GetWeeklyQuotesAsync(symbol, type);
            
            return _performanceCalculator.Calculate(quotes);
        }
    }
}