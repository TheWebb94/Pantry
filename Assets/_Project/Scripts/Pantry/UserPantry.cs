using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RecipeGen.Core;
using UnityEngine;

namespace RecipeGen.Core
{
    //define a pantryy item that stores a quantity against the ingredient
    public class PantryItem
    {
        public Ingredient Ingredient { get; }
        public float Quantity { get; set; }
        public Unit Unit { get; }
    
        public PantryItem(Ingredient ingredient, float quantity,  Unit unit)
        {
            Ingredient = ingredient;
            Quantity = quantity;
            Unit = unit;
        }
    }
    
    public enum Unit
    {
        Gram = 0,
        Millilitre = 1,
        Portion = 2
    }
    public sealed class UserPantry : MonoBehaviour
    {
        
        private readonly Dictionary<int, PantryItem> items = new Dictionary<int, PantryItem>();
        public event Action Changed;
        public IReadOnlyCollection<PantryItem> Items => items.Values;
        public IEnumerable<Ingredient> Ingredients => items.Values.Select(i => i.Ingredient);
        public int Count => items.Count;
        
        public bool Contains(Ingredient ingredient) => items.ContainsKey(ingredient.ID);
    
        public void AddIngredient(Ingredient ingredient, float quantity, Unit unit)
        {
            if (ingredient == null)  return;
            if (quantity <= 0) return;
            
            if (items.TryGetValue(ingredient.ID, out var existing))
            {
                // Adding quantity in g to an amount stored in portions wouldn't make sense.
                if (existing.Unit != unit)
                    throw new InvalidOperationException(
                        $"{ingredient.Name} is stored in {existing.Unit}, not {unit}.");

                existing.Quantity += quantity;
            }
            else
            {
                items[ingredient.ID] = new PantryItem(ingredient, quantity, unit);
            }
            
            
            Changed?.Invoke();
        }
    
        private bool RemoveIngredient(Ingredient ingredient)
        {
            if (ingredient == null || items.Remove(ingredient.ID)) return false;
            
            Changed?.Invoke();
            return true;
        }
    }
}