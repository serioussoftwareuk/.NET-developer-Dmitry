using System;
using System.Collections.Generic;
using System.Linq;
using API.Models;

namespace API.Services
{
    public interface IPerformanceCalculator
    {
        CalculationModel Calculate(List<QuoteDTO> quotes);
    }
    
    public class PerformanceCalculator : IPerformanceCalculator
    {
        public CalculationModel Calculate(List<QuoteDTO> quotes)
        {
            var symbol = quotes.FirstOrDefault()?.Symbol;

            if (string.IsNullOrEmpty(symbol))
                throw new ArgumentException("Symbol can not be null.");
            
            var orderedQuotes = quotes
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
    }
}