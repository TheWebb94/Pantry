using System.Linq;
using RecipeGen.Core;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class DataLoader : MonoBehaviour
{
    
    [SerializeField] private string resourceName = "ingredients";
    [SerializeField] private bool logSummary = true;
    
    public IngredientDatabase Database { get; set; }
    public PairScorer Scorer { get; set; }

    private void Awake()
    {
        var file = Resources.Load<TextAsset>(resourceName);

        if (file == null) //if file fails to load, return early
        {
            Debug.LogError($"Could not find ingredients file: {resourceName}");
            return;
        }
        
        Database = IngredientDatabase.FromJson(file.text);
        Scorer = new PairScorer(Database);

        if (logSummary) LogSummary();
    }

    private void LogSummary()
    {
        //validate database has been correctly instantiated with output log
        Debug.Log($"Loaded {Database.Count} ingredients");
        Debug.Log($"Loaded {Database.Categories.Count} categories");
        
        /*
         * logical test on a claim from research - section 4.2.1
         * 'a combination would be successful when volatiles are shared’. An example given was that chocolate and blue cheese would be a good match because they share 73 flavour compounds'
         * 
         */
        if (Database.TryGetByKey("cocoa", out var cocoa) && Database.TryGetByKey("blue_cheese", out var blue))
        {
            Debug.Log($"Cocoa + Blue cheese share {PairScorer.CountShared(cocoa, blue)} compounds, " +
                      $"score {Scorer.Score(cocoa, blue):0.000}");
        }
        
        
        /*
         * logical test to verify that compounds are being stored against ingredients
         */
        if (!Database.TryGetByKey("shallot", out var shallot)) //find ingredient by key
        {
            Debug.LogWarning($"No ingredient with key 'shallot' in the database."); //failsafe
            return;
        }

        var compoundNames = shallot.Compounds //gets the index (int) of all compounds logged against the ingredient
            .Select(id => Database.CompoundName(id)) //feed the ID into the compound name mapper in ingredient databse - returns name as text
            .OrderBy(name => name); 

        Debug.Log($"{shallot.Name} ({shallot.Category}) has {shallot.Compounds.Count} compounds:\n- " +
                  string.Join("\n- ", compoundNames));
    }

}
