using UnityEngine;
using System.Collections.Generic;

namespace RecipeGen.Core
{
    
    public class Ingredient
    {
        public int ID;
        public string Key;
        public string Name;
        public string Category;
        public IReadOnlyCollection<int> Compounds => compounds; 

        private readonly HashSet<int> compounds; //hashset used for compoound data storage for efficient lookups - O(1)

        //constructor for setting values on creation of ingredient 
        public Ingredient(int id, string key, string name, string category, IEnumerable<int> compoundIds)
        {
            this.ID = id;
            this.Key = key;
            this.Name = name;
            this.Category = category;
            
            compounds = new HashSet<int>(compoundIds);
        }
        
        public bool hasCompound(int compoundId)
        {
            return compounds.Contains(compoundId);
        }
        
        //override of native string method so when debugging I can call 'writeline(ingredient)' for info
        public override string ToString()
        {
            return $"{Name}: ({Category}, {compounds.Count} compounds)";
        }
    }
}