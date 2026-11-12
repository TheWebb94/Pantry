using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class ConfigTest
{
    // A Test behaves as an ordinary method
    [Test]
    public void ConfigTestSimplePasses()
    {
        // Use the Assert class to test conditions
        Assert.AreEqual(2, 1 + 1);
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator ConfigTestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
