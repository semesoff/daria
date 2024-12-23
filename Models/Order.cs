using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MenuOrder.Models
{
    public class Order
    {
        private List<MenuItem> _items = new();

        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = "New";
        public decimal TotalPrice { get; set; }
        
        public string? ItemsJson 
        { 
            get => _items.Any() ? JsonSerializer.Serialize(_items) : null;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    _items = JsonSerializer.Deserialize<List<MenuItem>>(value, options) ?? new();
                }
            }
        }

        public Order()
        {
            OrderDate = DateTime.Now;
            TotalPrice = 0;
        }

        public void AddItem(MenuItem item)
        {
            _items.Add(item);
            CalculateTotalPrice();
        }

        public void RemoveItem(MenuItem item)
        {
            _items.Remove(item);
            CalculateTotalPrice();
        }

        public IReadOnlyList<MenuItem> GetItems()
        {
            return _items.AsReadOnly();
        }

        private void CalculateTotalPrice()
        {
            TotalPrice = _items.Sum(item => item.CalculatePrice());
        }
    }
}
