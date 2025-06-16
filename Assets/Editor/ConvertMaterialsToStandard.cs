using UnityEngine;
using UnityEditor;

public class ConvertMaterialsToStandard : MonoBehaviour
{
    [MenuItem("Tools/Convert Materials to Standard")]
    static void ConvertAll()
    {
        string[] guids = AssetDatabase.FindAssets("t:Material");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat.shader.name.Contains("Universal Render Pipeline"))
            {
                Debug.Log("Converting: " + path);
                mat.shader = Shader.Find("Standard");
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Conversion complete.");
    }
}
