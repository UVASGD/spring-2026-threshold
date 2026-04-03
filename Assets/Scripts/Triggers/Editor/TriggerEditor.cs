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

        var effectLabels = new string[]
        {
            "Enable/Disable Objects",
            "Play Sound",
            "Enable/Disable Player Control",
            "Look at an Object",
            "Wait for set time",
            "Apply Camera Shake",
            "Display text onscreen",
            "Fade screen in/out",
            "Teleport Player",
            "Close a door",
            "Drop Current Physics Object",
            "Lerp Objects",
            "Apply Impulse"
        };

        Action[] effectActions;
        if (dynamicTrigger != null)
        {
            effectActions = new Action[]
            {
                () => dynamicTrigger.addEffect(new DisableObjectEffect()),
                () => dynamicTrigger.addEffect(new PlayNoiseEffect()),
                () => dynamicTrigger.addEffect(new RemovePlayerMovementEffect()),
                () => dynamicTrigger.addEffect(new LookAtEffect()),
                () => dynamicTrigger.addEffect(new WaitEffect()),
                () => dynamicTrigger.addEffect(new ShakeCameraEffect()),
                () => dynamicTrigger.addEffect(new DisplayMessageEffect()),
                () => dynamicTrigger.addEffect(new FadeOutEffect()),
                () => dynamicTrigger.addEffect(new TeleportPlayerEffect()),
                () => dynamicTrigger.addEffect(new CloseDoorEffect()),
                () => dynamicTrigger.addEffect(new DropPhysicsEffect()),
                () => dynamicTrigger.addEffect(new LerpObjectEffect()),
                () => dynamicTrigger.addEffect(new ApplyForceEffect())
            };
        }
        else
        {
            effectActions = new Action[effectLabels.Length]; // Empty actions
        }

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