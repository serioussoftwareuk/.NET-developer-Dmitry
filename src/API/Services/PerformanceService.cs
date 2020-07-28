using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public interface IPerformanceService
    {
        Task<CalculationResult> CalculateAsync(string[] symbols);
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
        
        public async Task<CalculationResult> CalculateAsync(string[] symbols)
        {
            var calculateBaseTask = CalculateAsync(QuoteType.Base, symbols);
            var calculateSPTask = CalculateAsync(QuoteType.SP, _config.SP);

            await Task.WhenAll(calculateBaseTask, calculateSPTask);

            return new CalculationResult
            {
                Base = await calculateBaseTask,
                SP = await calculateSPTask
            };
        }

        private async Task<CalculationModel> CalculateAsync(QuoteType type, params string[] symbols)
        {
            var calculationsTasks = symbols
                .Select(async x =>
                {
                    var quotes = await _quoteService.GetWeeklyHourQuotesAsync(x, type);
                    return _performanceCalculator.Calculate(quotes);
                })
                .ToList();

            await Task.WhenAll(calculationsTasks);

            var calculations = calculationsTasks
                .Select(x => x.Result)
                .ToList();

            var first = calculations.First();
            calculations.RemoveAt(0);
            
            var result = new List<CalculationItem>();
            foreach (var calculation in first.Items)
            {
                var itemsToAggregate = new List<CalculationItem>
                {
                    calculation
                };
                
                foreach (var list in calculations)
                {
                    var item = list.Items.FirstOrDefault(x => x.Date.Equals(calculation.Date));
                    
                    if(item != null)
                        itemsToAggregate.Add(item);
                }
                
                result.Add(new CalculationItem(calculation.Date, itemsToAggregate.Average(x => x.Value)));
            }
            
            return new CalculationModel(string.Join("\\", symbols))
            {
                Items = result
            };
        }
    }
}