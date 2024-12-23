using System;
using System.Windows;
using MenuOrder.Models;

namespace MenuOrder.Views
{
    public partial class MenuItemDialog : Window
    {
        public MenuItem? ResultItem { get; private set; }
        private readonly MenuItem? _existingItem;

        public MenuItemDialog(MenuItem? item = null)
        {
            InitializeComponent();
            _existingItem = item;

            if (item != null)
            {
                NameTextBox.Text = item.Name;
                DescriptionTextBox.Text = item.Description;
                PriceTextBox.Text = item.Price.ToString();
                CategoryTextBox.Text = item.Category;

                if (item is Dish dish)
                {
                    DishRadioButton.IsChecked = true;
                    CookingTimeTextBox.Text = dish.CookingTime.ToString();
                }
                else if (item is Beverage beverage)
                {
                    BeverageRadioButton.IsChecked = true;
                    VolumeTextBox.Text = beverage.Volume.ToString();
                    IsAlcoholicCheckBox.IsChecked = beverage.IsAlcoholic;
                }
            }
            else
            {
                DishRadioButton.IsChecked = true;
            }

            UpdateVisibility();
        }

        private void TypeRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            bool isDish = DishRadioButton.IsChecked == true;
            DishPanel.Visibility = isDish ? Visibility.Visible : Visibility.Collapsed;
            BeveragePanel.Visibility = isDish ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                string.IsNullOrWhiteSpace(PriceTextBox.Text) ||
                string.IsNullOrWhiteSpace(CategoryTextBox.Text))
            {
                MessageBox.Show("Пожалуйста, заполните все поля", "Ошибка");
                return;
            }

            if (!decimal.TryParse(PriceTextBox.Text, out decimal price))
            {
                MessageBox.Show("Некорректная цена", "Ошибка");
                return;
            }

            if (DishRadioButton.IsChecked == true)
            {
                if (!int.TryParse(CookingTimeTextBox.Text, out int cookingTime))
                {
                    MessageBox.Show("Некорректное время приготовления", "Ошибка");
                    return;
                }

                if (_existingItem is Dish existingDish)
                {
                    existingDish.Name = NameTextBox.Text;
                    existingDish.Description = DescriptionTextBox.Text;
                    existingDish.Category = CategoryTextBox.Text;
                    existingDish.Price = price;
                    existingDish.CookingTime = cookingTime;
                    ResultItem = existingDish;
                }
                else
                {
                    ResultItem = new Dish
                    {
                        Name = NameTextBox.Text,
                        Description = DescriptionTextBox.Text,
                        Category = CategoryTextBox.Text,
                        Price = price,
                        CookingTime = cookingTime
                    };
                }
            }
            else
            {
                if (!int.TryParse(VolumeTextBox.Text, out int volume))
                {
                    MessageBox.Show("Некорректный объем", "Ошибка");
                    return;
                }

                if (_existingItem is Beverage existingBeverage)
                {
                    existingBeverage.Name = NameTextBox.Text;
                    existingBeverage.Description = DescriptionTextBox.Text;
                    existingBeverage.Category = CategoryTextBox.Text;
                    existingBeverage.Price = price;
                    existingBeverage.Volume = volume;
                    existingBeverage.IsAlcoholic = IsAlcoholicCheckBox.IsChecked ?? false;
                    ResultItem = existingBeverage;
                }
                else
                {
                    ResultItem = new Beverage
                    {
                        Name = NameTextBox.Text,
                        Description = DescriptionTextBox.Text,
                        Category = CategoryTextBox.Text,
                        Price = price,
                        Volume = volume,
                        IsAlcoholic = IsAlcoholicCheckBox.IsChecked ?? false
                    };
                }
            }

            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
