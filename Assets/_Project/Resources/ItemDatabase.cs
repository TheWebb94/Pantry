using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[System.Serializable]
public class Compound
{
    public int id;
    public string name;
}

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Scriptable Objects/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public List<Compound> items = new List<Compound>();

    [Header("JSON Importer")]
    public TextAsset jsonToImport;

    public ItemDatabase()
    {
        JsonUtility.FromJsonOverwrite(jsonToImport.text, this);
    }
    
    [ContextMenu("Hydrate From JSON")]
    public void HydrateFromJson()
    {
        if (string.IsNullOrEmpty(jsonToImport.text)) return;

        JsonUtility.FromJsonOverwrite(jsonToImport.text, this);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
#endif
    }
}