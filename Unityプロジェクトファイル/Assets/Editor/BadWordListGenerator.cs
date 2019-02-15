using UnityEditor;
using UnityEngine;

public class BadWordListGenerator : ScriptableObject
{
    [MenuItem("Assets/Create/CreateBadWordList")]
    private static void CreateBadWordList()
    {
        var obj = Selection.activeObject;

        if (obj.GetType().ToString() != "UnityEngine.TextAsset")
        {
            Debug.LogWarning(obj.name + " is not TextAsset." + obj.GetType().ToString());
            return;
        }

        TextAsset textAsset = (TextAsset)obj;

        BadWordList badWordList = CreateInstance<BadWordList>();

        badWordList.language = textAsset.name;
        badWordList.words = textAsset.text.Split("\n"[0]);

        AssetDatabase.CreateAsset(badWordList, "Assets/Resources/BadWordList_" + textAsset.name + ".asset");
        AssetDatabase.Refresh();
    }
}