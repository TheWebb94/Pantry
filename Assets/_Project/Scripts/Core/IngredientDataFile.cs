using System;
using UnityEngine;

namespace RecipeGen.Core
{
    //Plain data classes that match the ingredient JSON
    
    [Serializable]
    public class IngredientDataFile
    {
        public string source;
        public int minCompounds;
        public IngredientRecord[] ingredients;
        public CompoundRecord[] compounds;
    }
    
    [Serializable]
    public class IngredientRecord
    {
        public int id;
        public string key;
        public string name;
        public string category;
        public int[] compoundIds;
    }
 
    [Serializable]
    public class CompoundRecord
    {
        public int id;
        public string name;
    }
}
