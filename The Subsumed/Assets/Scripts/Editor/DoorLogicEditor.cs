using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DoorLogic))]
public class DoorLogicEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DoorLogic doorLogic = (DoorLogic)target;

        if (GUILayout.Button("Apply default settings", GUILayout.Height(30f)))
        {
            Debug.LogWarning("Applying default settings to the object", target);
            doorLogic.DefaultSettings();
        }
    }
}
