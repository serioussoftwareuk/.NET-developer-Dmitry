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
        Task<object> CalculateAsync(string symbol);
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
        
        public async Task<object> CalculateAsync(string symbol)
        {
            var calculateBaseTask = CalculateAsync(symbol, QuoteType.Base);
            var calculateSPTask = CalculateAsync(_config.SP, QuoteType.SP);

            await Task.WhenAll(calculateBaseTask, calculateSPTask);

            return new
            {
                Base = await calculateBaseTask,
                SP = await calculateSPTask
            };
        }

        private async Task<List<object>> CalculateAsync(string symbol, QuoteType type)
        {
            var dbQuotes = await _quoteRepository.GetQuotes(symbol, type);

            if (!ValidateDbQuotes(dbQuotes))
            {
                dbQuotes = await FetchQuotes(symbol);
            }

            var orderedQuotes = dbQuotes
                .OrderByDescending(x => x.Date)
                .Take(7)
                .OrderBy(x => x.Date)
                .ToList();
            
            var result = new List<object>();
            decimal based = 0;
            for (int i = 0; i < orderedQuotes.Count; i++)
            {
                if (i == 0)
                {
                    result.Add(new
                    {
                        orderedQuotes[0].Date,
                        Value = 0
                    });

                    based = orderedQuotes[0].Close;
                    
                    continue;
                }
                
                result.Add(new
                {
                    orderedQuotes[i].Date,
                    Value = based / (orderedQuotes[i].Close - based)
                });
            }
        }

        private bool ValidateDbQuotes(List<QuoteDTO> quotes)
        {
            if (quotes.Count < 7)
                return false;
            
            int day = 0;
            foreach (var quote in quotes.OrderByDescending(x => x.Date))
            {
                if (quote.Date != DateTime.Now.AddDays(-day++).Date)
                    return false;

                if (day == 7)
                    return true;
            }
            
            return true;
        }

        private async Task<Quote> FetchQuotes(string symbol)
        {
            var quotes = await _alphaVantageHttpClient.GetDailyQuotes(symbol);
        }
    }
}