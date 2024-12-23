using System;
using System.Windows;
using MenuOrder.Data;
using MenuOrder.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MenuOrder.Views;

namespace MenuOrder
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationDbContext _context;
        private string _currentView = "Menu";

        public MainWindow()
        {
            InitializeComponent();
            _context = new ApplicationDbContext();
            _context.Database.EnsureCreated();
            LoadMenuItems();
        }

        private void LoadMenuItems()
        {
            var items = _context.Dishes.ToList<MenuItem>()
                .Concat(_context.Beverages.ToList<MenuItem>());
            MainDataGrid.ItemsSource = items;
        }

        private void LoadOrders()
        {
            MainDataGrid.ItemsSource = _context.Orders.ToList();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            _currentView = "Menu";
            LoadMenuItems();
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            _currentView = "Orders";
            LoadOrders();
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            var statisticsWindow = new StatisticsWindow(_context);
            statisticsWindow.Owner = this;
            statisticsWindow.ShowDialog();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentView == "Menu")
            {
                var dialog = new MenuItemDialog();
                dialog.Owner = this;
                if (dialog.ShowDialog() == true)
                {
                    if (dialog.ResultItem is Dish dish)
                        _context.Dishes.Add(dish);
                    else if (dialog.ResultItem is Beverage beverage)
                        _context.Beverages.Add(beverage);

                    _context.SaveChanges();
                    LoadMenuItems();
                }
            }
            else if (_currentView == "Orders")
            {
                var dialog = new OrderDialog(_context);
                dialog.Owner = this;
                if (dialog.ShowDialog() == true)
                {
                    _context.Orders.Add(dialog.ResultOrder);
                    _context.SaveChanges();
                    LoadOrders();
                }
            }
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите элемент для редактирования", "Предупреждение");
                return;
            }

            if (_currentView == "Menu")
            {
                var selectedItem = MainDataGrid.SelectedItem as MenuItem;
                var dialog = new MenuItemDialog(selectedItem);
                dialog.Owner = this;
                if (dialog.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    LoadMenuItems();
                }
            }
            else if (_currentView == "Orders")
            {
                var selectedOrder = MainDataGrid.SelectedItem as Order;
                var dialog = new OrderDialog(_context, selectedOrder);
                dialog.Owner = this;
                if (dialog.ShowDialog() == true)
                {
                    _context.SaveChanges();
                    LoadOrders();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainDataGrid.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите элемент для удаления", "Предупреждение");
                return;
            }

            var result = MessageBox.Show("Вы уверены, что хотите удалить выбранный элемент?", 
                "Подтверждение", MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                if (_currentView == "Menu")
                {
                    var selectedItem = MainDataGrid.SelectedItem as MenuItem;
                    if (selectedItem is Dish dish)
                        _context.Dishes.Remove(dish);
                    else if (selectedItem is Beverage beverage)
                        _context.Beverages.Remove(beverage);
                }
                else if (_currentView == "Orders")
                {
                    var selectedOrder = MainDataGrid.SelectedItem as Order;
                    _context.Orders.Remove(selectedOrder);
                }

                _context.SaveChanges();
                
                if (_currentView == "Menu")
                    LoadMenuItems();
                else
                    LoadOrders();
            }
        }
    }
}
