using System;
using System.Collections.Generic;
using System.Linq;

namespace MenuOrder.Models
{
    public class Order
    {
        // Инкапсуляция - приватные поля
        private List<MenuItem> _items;
        private decimal _totalPrice;

        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }

        public Order()
        {
            _items = new List<MenuItem>();
            OrderDate = DateTime.Now;
            Status = "New";
        }

        // Публичные методы для работы с приватными полями
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

        public decimal GetTotalPrice()
        {
            return _totalPrice;
        }

        private void CalculateTotalPrice()
        {
            _totalPrice = _items.Sum(item => item.CalculatePrice());
        }
    }
}
