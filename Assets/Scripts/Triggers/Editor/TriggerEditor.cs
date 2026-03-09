using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(DynamicTrigger))]
public class TriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (target == null)
        {
            base.OnInspectorGUI();
            return; //prevents an annoying error in scenes without a dynamic trigger
        }
        
        var dynamicTrigger = target as DynamicTrigger;

        EditorGUILayout.LabelField("Trigger Effects", EditorStyles.boldLabel);

        //add more as more trigger effects are made
        var effectLabels = new string[]
        {
            "Enable/Disable Objects",
            "Play Sound"
        };

        //also be sure to add more here
        var effectActions = new Action[]
        {
            () => dynamicTrigger.addEffect(new DisableObjectEffect()),
            () => dynamicTrigger.addEffect(new PlayNoiseEffect())
        };

        DrawTripleButtons(effectLabels, effectActions);

        base.OnInspectorGUI();
    }
    private void DrawTripleButtons(string[] labels, Action[] actions)
    {
        if (labels == null || actions == null) return;
        int count = Math.Min(labels.Length, actions.Length);
        float padding = 18f;
        float third = Mathf.Max(60f, (EditorGUIUtility.currentViewWidth - padding) / 3f);

        for (int i = 0; i < count; i += 3)
        {
            EditorGUILayout.BeginHorizontal();
            for (int j = 0; j < 3; j++)
            {
                int idx = i + j;
                if (idx < count)
                {
                    if (GUILayout.Button(labels[idx], GUILayout.Width(third)))
                        actions[idx]?.Invoke();
                }
                else
                {
                    GUILayout.FlexibleSpace();
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
    private object DeepClone(object source)
    {
        if (source == null) return null;
        try
        {
            var type = source.GetType();
            string json = JsonUtility.ToJson(source);
            var clone = Activator.CreateInstance(type);
            JsonUtility.FromJsonOverwrite(json, clone);
            return clone;
        }
        catch (Exception)
        {
            return null;
        }
    }
}