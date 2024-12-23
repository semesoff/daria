using System;

namespace MenuOrder.Models
{
    // Базовый класс для демонстрации наследования
    public abstract class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        
        // Виртуальный метод для демонстрации полиморфизма
        public virtual decimal CalculatePrice()
        {
            return Price;
        }
    }

    // Пример наследования 1
    public class Dish : MenuItem
    {
        public int CookingTime { get; set; }
        public string Category { get; set; }

        public override decimal CalculatePrice()
        {
            // Для блюд с длительным временем приготовления цена выше
            return CookingTime > 30 ? Price * 1.1m : Price;
        }
    }

    // Пример наследования 2
    public class Beverage : MenuItem
    {
        public double Volume { get; set; }
        public bool IsAlcoholic { get; set; }

        public override decimal CalculatePrice()
        {
            // Для алкогольных напитков дополнительная наценка
            return IsAlcoholic ? Price * 1.2m : Price;
        }
    }
}
