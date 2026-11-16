using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RecipeGen.Core
{
    public class PairScorer
    {
        private readonly IngredientDatabase database;
        private readonly float[,] scores; 

        public PairScorer(IngredientDatabase database)
        {
            this.database = database;
            int n = database.Count;
            scores = new float[n, n]; //here we create a matrix for all ingredients so every ingredient can be scored against eachother. 100 ingredients = 100x100 matrix

            for (int i = 0; i < n; i++)
            {
                scores[i, i] = 1f; //when column and row number are same indicates the ingredients is scoring against itself = score set to 1 (self is removed from top-scorers when checking the best candidates)

                for (int j = i + 1; j < n; j++)
                {
                    float s = Jaccard(database.All[i], database.All[j]); //performs the scoring across all elements of the matrix
                    
                    //where i = j, also j = i. ingredient pair mirror selves
                    scores[i, j] = s;
                    scores[j, i] = s; 
                }
            }
        }

        public static float Jaccard(Ingredient a, Ingredient b)
        {
        /*
         * TODO: create test on Jaccard - assert Jaccard(i1, i2) = 1 when
         * ingredient1 w compounds (0, 1, 2, 3, 4)
         * ingredient2 w compounds (0, 1, 2, 3, 4)
         *
         * TODO: assert zero overlap set  Jaccard(i1, i2) = 0 when
         * ingredient1 w compounds (1, 3 ,5 ,7, 9)
         * ingredient2 w compounds (0, 2, 4, 6, 8)
         *
         * TODO: write full testing framework for pair scorer class
        */ 
            
            int shared = CountShared(a, b); //overlap of compounds of both ingredients  ex (onion 58, garlic 21) shared = 16
            int union = a.Compounds.Count + b.Compounds.Count - shared; //              ex 58 + 21 - 16 = 63
            
            //cast to float and return the calculated value
            if (union == 0)
            {
                return 0f;
            } 
            return (float)shared / union;                           // shared/union ex 16 / 63 = 0.253... = Jaccard similarity score
            
        }

        public static int CountShared(Ingredient a, Ingredient b)
        {
            //create local variables to store ingredients - we only need to loop through the smaller set. 
            Ingredient smallerSet;
            Ingredient greaterSet;

            //determine smaller compound set and set local variable 
            if (a.Compounds.Count <= b.Compounds.Count)
            {
                smallerSet = a;
                greaterSet = b;
            }
            else
            {
                greaterSet = a;
                smallerSet = b;
            }
            
            int shared = 0;
            
            foreach (int c in smallerSet.Compounds)
            {
                if (greaterSet.HasCompound(c)) shared++;
            }
            
            return shared;
        }
        
        //lookup of score for two ingredients against the pre-calculated index
        public float Score(Ingredient a, Ingredient b) => scores[database.IndexOf(a),  database.IndexOf(b)];

        //returns list of compounds present in both given ingredients
        public IEnumerable<int> SharedCompounds(Ingredient a, Ingredient b) =>
            a.Compounds.Where(b.HasCompound).OrderBy(c => c);

        
        /// <summary>
        /// Returns a list of the top scoring partners of the reference ingredient
        /// </summary>
        /// <param name="ingredient">Reference/starting ingredient</param>
        /// <param name="count">how many ingredient partners to return</param>
        /// <param name="candidates">list of ingredients to include in scoring</param>
        /// <returns></returns>
        public IEnumerable<(Ingredient ingredient, float score)> TopScoringPartners(
            Ingredient ingredient, int count, IEnumerable<Ingredient> candidates = null)
        {
            if (candidates == null)
            {
                candidates = database.All;
            }
            
            /*
             * return an ordered list of top scorers
             * where statement removes self from list so ingredient doesn't match to self
             * select turns each entry into a tuple containing the ingredient, score
             * Ordeerbydescending organising the list in order of best scoring
             * Take picks the x amount of top scoring ingredients
             */
            return candidates.Where(c => c.ID != ingredient.ID)
                             .Select(c => (c, Score(ingredient, c)))
                             .OrderByDescending(p => p.Item2)
                             .Take(count);
        }
    }
}
