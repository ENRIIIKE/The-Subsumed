using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemSpawner))]
public class ItemSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ItemSpawner itemSpawner = (ItemSpawner)target;

        if (GUILayout.Button("Find new spawn locations", GUILayout.Height(30f)))
        {
            itemSpawner.FindSpawnLocations();
        }
    }
}
