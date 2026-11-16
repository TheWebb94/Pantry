using System.Collections;
using System.Linq;
using NUnit.Framework;
using RecipeGen.Core;
using UnityEngine;
using UnityEngine.TestTools;

public class IngredientDataTests
{
    
    private IngredientDatabase db;
    
    // Onetimesetup runs this method before anyy other tests, ensuring db setup is complete so tests can evluate results properly
    [OneTimeSetUp]
    public void LoadRealDataTest()
    {
        var file = Resources.Load<TextAsset>("ingredients");
        
        Assert.NotNull(file);
        
        db = IngredientDatabase.FromJson(file.text);
    }

    //test that all curated ingredients make it to the database
    [Test]
    public void LoadsAllIngredientsFromFile()
    {
        Assert.AreEqual(99, db.Count);
    }
    
    //
    [Test]
    public void CompoundDataIsStoredOnIngredients()
    {
        Assert.IsTrue(db.All.All(i => i.Compounds.Count >= 10));
    }
}
