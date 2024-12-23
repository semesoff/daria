using System;
using System.Text.Json.Serialization;

namespace MenuOrder.Models
{
    // Базовый класс для демонстрации наследования
    [JsonDerivedType(typeof(Dish), typeDiscriminator: "Dish")]
    [JsonDerivedType(typeof(Beverage), typeDiscriminator: "Beverage")]
    public abstract class MenuItem
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public decimal Price { get; set; }
        public required string Category { get; set; }

        // Виртуальный метод для демонстрации полиморфизма
        public abstract decimal CalculatePrice();
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
            return IsAlcoholic ? Price * 1.2m : Price;
        }
    }
}
