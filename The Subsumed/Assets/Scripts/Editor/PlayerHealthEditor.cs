using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerHealth))]
public class PlayerHealthEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Take damage"))
        {
            HealthSystem healthSystem = (HealthSystem)target;
            healthSystem.TriggerDamage(1);
        }
        if (GUILayout.Button("Heal player"))
        {
            HealthSystem healthSystem = (HealthSystem)target;
            healthSystem.TriggerHeal(1);
        }
    }
}
