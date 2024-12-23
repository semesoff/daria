using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MenuOrder.Data;
using MenuOrder.Models;
using Microsoft.EntityFrameworkCore;

namespace MenuOrder.Views
{
    public partial class OrderDialog : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly List<MenuItem> _orderItems;
        private Order? _existingOrder;
        public Order? ResultOrder { get; private set; }

        public OrderDialog(ApplicationDbContext context, Order? existingOrder = null)
        {
            InitializeComponent();
            _context = context;
            _orderItems = new List<MenuItem>();
            _existingOrder = existingOrder;

            // Загружаем все доступные элементы меню
            var menuItems = _context.Dishes.AsNoTracking().ToList<MenuItem>()
                .Concat(_context.Beverages.AsNoTracking().ToList<MenuItem>())
                .ToList();
            MenuItemsGrid.ItemsSource = menuItems;

            if (existingOrder != null)
            {
                foreach (var item in existingOrder.GetItems())
                {
                    // Находим актуальную версию элемента из базы данных
                    MenuItem? dbItem = null;
                    if (item is Dish dish)
                    {
                        dbItem = _context.Dishes.FirstOrDefault(d => d.Id == dish.Id);
                    }
                    else if (item is Beverage beverage)
                    {
                        dbItem = _context.Beverages.FirstOrDefault(b => b.Id == beverage.Id);
                    }

                    if (dbItem != null)
                    {
                        _orderItems.Add(dbItem);
                    }
                }
            }

            UpdateOrderDisplay();
        }

        private void UpdateOrderDisplay()
        {
            OrderItemsGrid.ItemsSource = null;
            OrderItemsGrid.ItemsSource = _orderItems;

            decimal total = _orderItems.Sum(item => item.CalculatePrice());
            TotalPriceRun.Text = $"{total:N2} ₽";
        }

        private void AddToOrder_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = MenuItemsGrid.SelectedItem as MenuItem;
            if (selectedItem != null)
            {
                _orderItems.Add(selectedItem);
                UpdateOrderDisplay();
            }
        }

        private void RemoveFromOrder_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = OrderItemsGrid.SelectedItem as MenuItem;
            if (selectedItem != null)
            {
                _orderItems.Remove(selectedItem);
                UpdateOrderDisplay();
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_orderItems.Any())
            {
                MessageBox.Show("Добавьте хотя бы один элемент в заказ", "Предупреждение");
                return;
            }

            if (_existingOrder != null)
            {
                ResultOrder = _existingOrder;
            }
            else
            {
                ResultOrder = new Order();
            }

            ResultOrder.ClearItems();
            foreach (var item in _orderItems)
            {
                ResultOrder.AddItem(item);
            }

            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
