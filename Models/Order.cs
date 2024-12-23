using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

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
                    try
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            Converters = { new JsonStringEnumConverter() }
                        };
                        var items = JsonSerializer.Deserialize<List<MenuItem>>(value, options);
                        _items.Clear();
                        if (items != null)
                        {
                            _items.AddRange(items);
                        }
                    }
                    catch (JsonException ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"JSON Deserialization error: {ex.Message}");
                        _items.Clear();
                        _itemsJson = "[]";
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
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    Converters = { new JsonStringEnumConverter() }
                };
                _itemsJson = JsonSerializer.Serialize(_items, options);
            }
            catch (JsonException ex)
            {
                System.Diagnostics.Debug.WriteLine($"JSON Serialization error: {ex.Message}");
                _itemsJson = "[]";
            }
        }
    }
}
