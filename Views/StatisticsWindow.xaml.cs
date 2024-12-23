using System.Linq;
using System.Windows;
using MenuOrder.Data;
using System.Collections.Generic;

namespace MenuOrder.Views
{
    public partial class StatisticsWindow : Window
    {
        private readonly ApplicationDbContext _context;

        public StatisticsWindow(ApplicationDbContext context)
        {
            InitializeComponent();
            _context = context;
            LoadStatistics();
        }

        private void LoadStatistics()
        {
            // Загружаем все заказы
            var orders = _context.Orders.ToList();

            // Общее количество заказов
            OrderCountText.Text = orders.Count.ToString();

            // Общая выручка
            decimal totalRevenue = orders.Sum(o => o.GetTotalPrice());
            TotalRevenueText.Text = totalRevenue.ToString("C");

            // Средний чек
            decimal averageOrder = orders.Any() ? totalRevenue / orders.Count : 0;
            AverageOrderText.Text = averageOrder.ToString("C");

            // Популярные позиции
            var popularItems = new Dictionary<string, int>();
            foreach (var order in orders)
            {
                foreach (var item in order.GetItems())
                {
                    if (!popularItems.ContainsKey(item.Name))
                        popularItems[item.Name] = 0;
                    popularItems[item.Name]++;
                }
            }

            var topItems = popularItems
                .Select(kvp => new { Name = kvp.Key, OrderCount = kvp.Value })
                .OrderByDescending(x => x.OrderCount)
                .Take(10)
                .ToList();

            PopularItemsGrid.ItemsSource = topItems;
        }
    }
}
