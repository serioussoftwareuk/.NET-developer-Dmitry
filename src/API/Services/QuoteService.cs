using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using API.Repositories;

namespace API.Services
{
    public interface IQuoteService
    {
        Task<List<QuoteDTO>> GetWeeklyQuotesAsync(string symbol, QuoteType type);
    }
    
    public class QuoteService : IQuoteService
    {
        private readonly IAlphaVantageHttpClient _alphaVantageHttpClient;
        private readonly IQuoteRepository _quoteRepository;

        public QuoteService(IAlphaVantageHttpClient alphaVantageHttpClient, IQuoteRepository quoteRepository)
        {
            _alphaVantageHttpClient = alphaVantageHttpClient;
            _quoteRepository = quoteRepository;
        }
        
        public async Task<List<QuoteDTO>> GetWeeklyQuotesAsync(string symbol, QuoteType type)
        {
            var quotes = await _quoteRepository.GetQuotes(symbol, type);

            if (!ValidateDbQuotes(quotes))
            {
                quotes = await FetchQuotes(symbol, type);
                await _quoteRepository.InsertQuotes(quotes);
            }

            return quotes;
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

                if (++day == 7)
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