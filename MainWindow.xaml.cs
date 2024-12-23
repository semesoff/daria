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
            var menuColumns = MainDataGrid.Columns;
            // Показываем только колонки меню
            menuColumns[0].Visibility = Visibility.Visible; // Название
            menuColumns[1].Visibility = Visibility.Visible; // Описание
            menuColumns[2].Visibility = Visibility.Visible; // Цена
            menuColumns[3].Visibility = Visibility.Visible; // Категория
            menuColumns[4].Visibility = Visibility.Collapsed; // ID заказа
            menuColumns[5].Visibility = Visibility.Collapsed; // Дата заказа

            var items = _context.Dishes.ToList<MenuItem>()
                .Concat(_context.Beverages.ToList<MenuItem>());
            MainDataGrid.ItemsSource = items;
        }

        private void LoadOrders()
        {
            var menuColumns = MainDataGrid.Columns;
            // Показываем только колонки заказов
            menuColumns[0].Visibility = Visibility.Collapsed; // Название
            menuColumns[1].Visibility = Visibility.Collapsed; // Описание
            menuColumns[2].Visibility = Visibility.Collapsed; // Цена
            menuColumns[3].Visibility = Visibility.Collapsed; // Категория
            menuColumns[4].Visibility = Visibility.Visible; // ID заказа
            menuColumns[5].Visibility = Visibility.Visible; // Дата заказа

            // Загружаем заказы с отслеживанием изменений
            var orders = _context.Orders.AsNoTracking().ToList();
            MainDataGrid.ItemsSource = orders;
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
                if (MainDataGrid.SelectedItem is MenuItem selectedItem)
                {
                    var dialog = new MenuItemDialog(selectedItem);
                    dialog.Owner = this;
                    if (dialog.ShowDialog() == true && dialog.ResultItem != null)
                    {
                        _context.SaveChanges();
                        LoadMenuItems();
                    }
                }
            }
            else if (_currentView == "Orders")
            {
                if (MainDataGrid.SelectedItem is Order selectedOrder)
                {
                    // Загружаем заказ с его элементами
                    var order = _context.Orders
                        .AsNoTracking()
                        .FirstOrDefault(o => o.Id == selectedOrder.Id);

                    if (order != null)
                    {
                        var dialog = new OrderDialog(_context, order);
                        dialog.Owner = this;
                        if (dialog.ShowDialog() == true && dialog.ResultOrder != null)
                        {
                            // Обновляем существующий заказ
                            selectedOrder.ItemsJson = dialog.ResultOrder.ItemsJson;
                            selectedOrder.TotalPrice = dialog.ResultOrder.TotalPrice;
                            _context.SaveChanges();
                            LoadOrders();
                        }
                    }
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
                    if (MainDataGrid.SelectedItem is MenuItem selectedItem)
                    {
                        if (selectedItem is Dish dish)
                            _context.Dishes.Remove(dish);
                        else if (selectedItem is Beverage beverage)
                            _context.Beverages.Remove(beverage);
                    }
                }
                else if (_currentView == "Orders")
                {
                    if (MainDataGrid.SelectedItem is Order selectedOrder)
                    {
                        _context.Orders.Remove(selectedOrder);
                    }
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
