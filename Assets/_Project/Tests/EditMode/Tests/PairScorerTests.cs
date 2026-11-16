using System.Collections;
using System.Linq;
using NUnit.Framework;
using RecipeGen.Core;
using UnityEngine;
using UnityEngine.TestTools;

//class for a set of unit tests to determine proper return values from the pair scoring algorithms
public class PairScorerTests
{
    
    private static Ingredient Make(int id, params int[] compounds) =>
        new Ingredient(id, "ing" + id, "Ingredient " + id, "test", compounds);
    
    // Test to make sure identical compound sets resolve to 1
    // testing is 1,2,3 = ,1,2,3
    [Test]
    public void JaccardIdenticalSetsEvaluateToOne()
    {
        Assert.AreEqual(1f, PairScorer.Jaccard(Make(1,1,2,3), Make(2,1,2,3)));
    }

    //Test for evaluating that two zeero overlap sets resolve to 0
    //testing is 1,3,5 != 0,2,4
    [Test]
    public void JaccardZeroOverlapSetsEvaluateToZero()
    {
        Assert.AreEqual(0f, PairScorer.Jaccard(Make(1,1,3,5), Make(2,0,2,4)));
    }
    
    //Test to check two sets that share half the compounds resolves to 0.5
    //Testing 2 out of 4 total compounds should resolve to 0.5
    // 1,2,3 : 2,3,4 = 2 & 3 are sharede out of 4 total compounds
    [Test]
    public void JaccardHalfOverlapSetsEvaluateToHalf()
    {
        Assert.AreEqual(0.5f, PairScorer.Jaccard(Make(1,1,2,3), Make(2,2,3,4)));
    }
    
    //Test zero compound data on ingredients resolves to zero
    [Test]
    public void JacccardNoCompoundDataEvaluatesToZero()
    {
        Assert.AreEqual(0f, PairScorer.Jaccard(Make(1), Make(2)));
    }
    
    //Test that top scoring partners is calculating properly
    [Test]
    public void TopPartnersExcludesSelfAndIsOrderedBestFirst()
    {
        var a = Make(1, 1, 2, 3);
        var close = Make(2, 1, 2, 3, 4);
        var far = Make(3, 3, 7, 8, 9);
        var scorer = new PairScorer(new IngredientDatabase(new[] { a, far, close }));
 
        var top = scorer.TopScoringPartners(a, 5).Select(p => p.ingredient).ToList();
 
        CollectionAssert.DoesNotContain(top, a);
        Assert.AreEqual(close, top[0]);
        Assert.AreEqual(far, top[1]);
    }
}
