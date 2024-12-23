using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MenuOrder.Data;
using MenuOrder.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MenuOrder.Views
{
    public partial class OrderDialog : Window
    {
        private readonly ApplicationDbContext _context;
        private readonly List<MenuItem> _orderItems;
        private readonly Order? _existingOrder;
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

            if (existingOrder != null && !string.IsNullOrEmpty(existingOrder.ItemsJson))
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        Converters = { new JsonStringEnumConverter() }
                    };

                    // Выводим JSON для отладки
                    System.Diagnostics.Debug.WriteLine($"Loading order items from JSON: {existingOrder.ItemsJson}");

                    var items = JsonSerializer.Deserialize<List<MenuItem>>(existingOrder.ItemsJson, options);

                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            System.Diagnostics.Debug.WriteLine($"Processing item: Type={item.GetType().Name}, Id={item.Id}, Name={item.Name}");

                            // Находим актуальную версию элемента из базы данных
                            MenuItem? dbItem = null;
                            if (item is Dish dish)
                            {
                                dbItem = menuItems.OfType<Dish>().FirstOrDefault(d => d.Id == dish.Id);
                                System.Diagnostics.Debug.WriteLine($"Looking for Dish with Id={dish.Id}, Found={(dbItem != null)}");
                            }
                            else if (item is Beverage beverage)
                            {
                                dbItem = menuItems.OfType<Beverage>().FirstOrDefault(b => b.Id == beverage.Id);
                                System.Diagnostics.Debug.WriteLine($"Looking for Beverage with Id={beverage.Id}, Found={(dbItem != null)}");
                            }

                            if (dbItem != null)
                            {
                                _orderItems.Add(dbItem);
                                System.Diagnostics.Debug.WriteLine($"Added item to order: {dbItem.Name}");
                            }
                        }
                    }
                }
                catch (JsonException ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error deserializing order items: {ex.Message}");
                    MessageBox.Show($"Ошибка при загрузке элементов заказа: {ex.Message}", "Ошибка");
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
