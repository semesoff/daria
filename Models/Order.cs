using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MenuOrder.Models
{
    public class Order
    {
        private readonly List<MenuItem> _items;
        private string _itemsJson;

        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = "New";
        public decimal TotalPrice { get; set; }

        public string ItemsJson
        {
            get => _itemsJson;
            set
            {
                _itemsJson = value;
                if (!string.IsNullOrEmpty(value))
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var items = JsonSerializer.Deserialize<List<MenuItem>>(value, options);
                    _items.Clear();
                    if (items != null)
                    {
                        _items.AddRange(items);
                    }
                }
            }
        }

        public Order()
        {
            _items = new List<MenuItem>();
            OrderDate = DateTime.Now;
            TotalPrice = 0;
            _itemsJson = "[]";
        }

        public void AddItem(MenuItem item)
        {
            _items.Add(item);
            CalculateTotalPrice();
            UpdateItemsJson();
        }

        public void RemoveItem(MenuItem item)
        {
            _items.Remove(item);
            CalculateTotalPrice();
            UpdateItemsJson();
        }

        public void ClearItems()
        {
            _items.Clear();
            CalculateTotalPrice();
            UpdateItemsJson();
        }

        public IReadOnlyList<MenuItem> GetItems()
        {
            return _items.AsReadOnly();
        }

        private void CalculateTotalPrice()
        {
            TotalPrice = _items.Sum(item => item.CalculatePrice());
        }

        private void UpdateItemsJson()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _itemsJson = JsonSerializer.Serialize(_items, options);
        }
    }
}
