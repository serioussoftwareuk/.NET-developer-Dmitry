using System;
using System.Collections.Generic;

namespace API.Models
{
    public class CalculationResult
    {
        public CalculationModel Base { get; set; }
        public CalculationModel SP { get; set; }
    }
    
    public class CalculationModel
    {
        public string Symbol { get; set; }

        public List<CalculationItem> Items { get; set; }

        public CalculationModel(string symbol)
        {
            Symbol = symbol;
            Items = new List<CalculationItem>();
        }
    }

    public class CalculationItem
    {
        public DateTime Date { get; set; }
        public decimal Value { get; set; }

        public CalculationItem(DateTime date, decimal value)
        {
            Date = date;
            Value = value;
        }
    }
}