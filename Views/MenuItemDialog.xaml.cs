using System;
using System.Windows;
using MenuOrder.Models;

namespace MenuOrder.Views
{
    public partial class MenuItemDialog : Window
    {
        public MenuItem ResultItem { get; private set; }
        private MenuItem _editingItem;

        public MenuItemDialog(MenuItem item = null)
        {
            InitializeComponent();
            _editingItem = item;
            
            if (item != null)
            {
                // Заполняем поля для редактирования
                NameTextBox.Text = item.Name;
                DescriptionTextBox.Text = item.Description;
                PriceTextBox.Text = item.Price.ToString();

                if (item is Dish dish)
                {
                    TypeComboBox.SelectedIndex = 0;
                    CookingTimeTextBox.Text = dish.CookingTime.ToString();
                    CategoryTextBox.Text = dish.Category;
                }
                else if (item is Beverage beverage)
                {
                    TypeComboBox.SelectedIndex = 1;
                    VolumeTextBox.Text = beverage.Volume.ToString();
                    IsAlcoholicCheckBox.IsChecked = beverage.IsAlcoholic;
                }
            }
            else
            {
                TypeComboBox.SelectedIndex = 0;
            }
        }

        private void TypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (TypeComboBox.SelectedIndex == 0)
            {
                DishPanel.Visibility = Visibility.Visible;
                BeveragePanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                DishPanel.Visibility = Visibility.Collapsed;
                BeveragePanel.Visibility = Visibility.Visible;
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(NameTextBox.Text))
                {
                    MessageBox.Show("Пожалуйста, введите название", "Ошибка");
                    return;
                }

                if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
                {
                    MessageBox.Show("Пожалуйста, введите корректную цену", "Ошибка");
                    return;
                }

                if (TypeComboBox.SelectedIndex == 0) // Dish
                {
                    if (!int.TryParse(CookingTimeTextBox.Text, out int cookingTime))
                    {
                        MessageBox.Show("Пожалуйста, введите корректное время приготовления", "Ошибка");
                        return;
                    }

                    ResultItem = _editingItem as Dish ?? new Dish();
                    var dish = (Dish)ResultItem;
                    dish.Name = NameTextBox.Text;
                    dish.Description = DescriptionTextBox.Text;
                    dish.Price = price;
                    dish.CookingTime = cookingTime;
                    dish.Category = CategoryTextBox.Text;
                }
                else // Beverage
                {
                    if (!double.TryParse(VolumeTextBox.Text, out double volume))
                    {
                        MessageBox.Show("Пожалуйста, введите корректный объем", "Ошибка");
                        return;
                    }

                    ResultItem = _editingItem as Beverage ?? new Beverage();
                    var beverage = (Beverage)ResultItem;
                    beverage.Name = NameTextBox.Text;
                    beverage.Description = DescriptionTextBox.Text;
                    beverage.Price = price;
                    beverage.Volume = volume;
                    beverage.IsAlcoholic = IsAlcoholicCheckBox.IsChecked ?? false;
                }

                DialogResult = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении: {ex.Message}", "Ошибка");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
