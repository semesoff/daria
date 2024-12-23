using System;
using System.Text.Json.Serialization;

namespace MenuOrder.Models
{
    // Базовый класс для демонстрации наследования
    [JsonDerivedType(typeof(Dish), typeDiscriminator: "dish")]
    [JsonDerivedType(typeof(Beverage), typeDiscriminator: "beverage")]
    public abstract class MenuItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Category { get; set; }

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

        public override decimal CalculatePrice()
        {
            return Price;
        }
    }

    // Пример наследования 2
    public class Beverage : MenuItem
    {
        public int Volume { get; set; }
        public bool IsAlcoholic { get; set; }

        public override decimal CalculatePrice()
        {
            return Price;
        }
    }
}
