using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class JsonTest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
         var pantry = new Dictionary<string, int> { { "onion", 3 }, { "garlic", 5 } };
                Debug.Log(JsonConvert.SerializeObject(pantry));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
