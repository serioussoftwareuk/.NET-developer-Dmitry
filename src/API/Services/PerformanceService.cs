using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using API.Repositories;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public interface IPerformanceService
    {
        Task<CalculationResult> CalculateAsync(string symbol);
    }
    
    public class PerformanceService : IPerformanceService
    {
        private readonly IAlphaVantageHttpClient _alphaVantageHttpClient;
        private readonly IQuoteRepository _quoteRepository;
        private readonly BaseConfiguration _config;

        public PerformanceService(
            IAlphaVantageHttpClient alphaVantageHttpClient,
            IQuoteRepository quoteRepository,
            IOptions<BaseConfiguration> config)
        {
            _alphaVantageHttpClient = alphaVantageHttpClient;
            _quoteRepository = quoteRepository;
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
            var dbQuotes = await _quoteRepository.GetQuotes(symbol, type);

            if (!ValidateDbQuotes(dbQuotes))
            {
                dbQuotes = await FetchQuotes(symbol, type);
                await _quoteRepository.InsertQuotes(dbQuotes);
            }

            var orderedQuotes = dbQuotes
                .OrderByDescending(x => x.Date)
                .Take(7)
                .OrderBy(x => x.Date)
                .ToList();
            
            var result = new CalculationModel(symbol);
            decimal based = 0;
            for (int i = 0; i < orderedQuotes.Count; i++)
            {
                if (i == 0)
                {
                    result.Items.Add(new CalculationItem(orderedQuotes[0].Date, 0));

                    based = orderedQuotes[0].Close;
                    
                    continue;
                }
                
                result.Items.Add(new CalculationItem(orderedQuotes[i].Date, (orderedQuotes[i].Close - based) / based * 100));

            }

            return result;
        }

        private bool ValidateDbQuotes(List<QuoteDTO> quotes)
        {
            if (quotes.Count < 7)
                return false;
            
            int day = 0;
            foreach (var quote in quotes.OrderByDescending(x => x.Date))
            {
                if (quote.Date != DateTime.Now.AddDays(-day).Date)
                    return false;

                day++;

                if (day == 7)
                    return true;
            }
            
            return true;
        }

        private async Task<List<QuoteDTO>> FetchQuotes(string symbol, QuoteType type)
        {
            var quotes = await _alphaVantageHttpClient.GetDailyQuotes(symbol);
            
            return quotes.TimeSeries
                .Select(x => x.Value.ToDto(symbol, type, DateTime.Parse(x.Key)))
                .ToList();
        }
    }
}