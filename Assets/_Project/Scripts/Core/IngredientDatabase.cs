using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RecipeGen.Core
{
    // Holds all supported ingredients from the curated ingredient/compound list
    public sealed class IngredientDatabase
    {
        public IReadOnlyList<Ingredient> All => ingredients; //returns all ingredients Readonly
        public IReadOnlyList<string> Categories { get; } //returns all categories
        public int Count => ingredients.Count; //retrns total count of ingredients

        //ingredient and compounds stored here for the database of supported ingreds:compounds - indexByid and bykey are mapping contexts so we can search lists more efficiently
        private readonly List<Ingredient> ingredients;
        private readonly Dictionary<int, int> indexById = new Dictionary<int, int>();
        private readonly Dictionary<string, Ingredient> byKey = new Dictionary<string, Ingredient>();
        private readonly Dictionary<int, string> compoundNames;

        public IngredientDatabase(IEnumerable<Ingredient> items, IDictionary<int, string> compoundNames = null)
        {
            //handles invalid evocation of emthod
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            ingredients = items.ToList(); //copies the given ingredients into database
            
            this.compoundNames = compoundNames != null
                ? new Dictionary<int, string>(compoundNames)
                : new Dictionary<int, string>();

            //indexes all ingredient ID and key to be mapped to a value of 0 - List length (i)
            for (int i = 0; i < ingredients.Count; i++)
            {
                var ing = ingredients[i];
                
                //Argument exceptions will handle invalid entries in the database - this likely wqont trigger with curated list of ingredients, but could be an issue with raw dataset / futureproofing
                if (indexById.ContainsKey(ing.ID) || byKey.ContainsKey(ing.Key)) throw new ArgumentException($"Duplicate ingredient {ing.ID}."); //checks if ingredient is already in database
                indexById[ing.ID] = i;
                byKey[ing.Key] = ing;
            }

            
            /*
             * GENERATE CATEGORIES
             * Select extracts all categories including duplicates
             * Distinct removes duplicates
             * Orderby then orders in alphabetical order
             * ToList stores in list
             */
            Categories = ingredients.Select(x => x.Category).Distinct().OrderBy(c => c).ToList(); 
        }

        // Builds the database from the contents of ingredients.json.
        public static IngredientDatabase FromJson(string json)
        {
            var file = JsonUtility.FromJson<IngredientDataFile>(json);
            
            if (file?.ingredients == null || file.ingredients.Length == 0)
                throw new FormatException("Ingredient JSON contains no ingredients.");
           
            //extract id, key, name, category, compounds from json
            var items = file.ingredients.Select(r => new Ingredient(r.id, r.key, r.name, r.category, r.compoundIds));
            //converts compunds into dictionary with cID as keys and compounds as values
            var names = (file.compounds ?? Array.Empty<CompoundRecord>()).ToDictionary(c => c.id, c => c.name); 
            return new IngredientDatabase(items, names);
        }

        // Returns the index pof the ingredient given
        public int IndexOf(Ingredient ingredient)
        {
            if (indexById.TryGetValue(ingredient.ID, out int index)) return index;
            throw new KeyNotFoundException($"'{ingredient.Key}' is not in the ingredient database.");
        }

        //Return the ingredient with the given iD
        public Ingredient GetById(int id) => ingredients[indexById[id]];

        //return the ingrdeient by key
        public bool TryGetByKey(string key, out Ingredient ingredient) => byKey.TryGetValue(key, out ingredient);

        //returns a list of all ingredients in the given category
        public IEnumerable<Ingredient> InCategory(string category) => ingredients.Where(i => i.Category == category);

        //returns the name of the compound at given index - with section 5.4.3 of research in mind  (giving scoring information to user)
        public string CompoundName(int compoundId) =>
            compoundNames.TryGetValue(compoundId, out var name) ? name : $"compound #{compoundId}"; //if no name is found, return the id as its name instead
    }
}