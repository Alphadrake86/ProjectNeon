﻿#if UNITY_EDITOR

using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CardActionsData))]
public class CardActionsDataEditor : Editor
{
    private CardActionsData _target;

    public void OnEnable()
    {
        _target = (CardActionsData) target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.indentLevel = 0;
        var actions = _target.Actions.ToArray();
        PresentLabelsWithControls("Effect Sequence", menu => menu.AddItem(new GUIContent("Insert New"), false, () =>
        {
            _target.Actions = new CardActionV2[] { new CardActionV2() }.Concat(actions).ToArray();
            EditorUtility.SetDirty(target);
        }));
        EditorGUI.indentLevel++;
        for (var i = 0; i < actions.Length; i++)
        {
            var refBrokeni = i;
            var action = actions[refBrokeni];
            PresentLabelsWithControls($"Effect {refBrokeni}", menu =>
            {
                menu.AddItem(new GUIContent("Add"), false, () =>
                {
                    _target.Actions = actions.Take(Array.IndexOf(actions, action) + 1)
                        .Concat(new CardActionV2[] {new CardActionV2()})
                        .Concat(actions.Skip(Array.IndexOf(actions, action) + 1))
                        .ToArray();
                    EditorUtility.SetDirty(target);
                });
                menu.AddItem(new GUIContent("Delete"), false, () =>
                {
                    _target.Actions = actions.Where(x => x != action).ToArray();
                    EditorUtility.SetDirty(target);
                });
            });
            EditorGUI.indentLevel++;
            PresentUnchanged(serializedObject.FindProperty($"Actions.Array.data[{refBrokeni}].type"));
            if (action.Type == CardBattleActionType.AnimateCharacter)
                PresentUnchanged(serializedObject.FindProperty($"Actions.Array.data[{refBrokeni}].characterAnimation"));
            if (action.Type == CardBattleActionType.AnimateAtTarget)
                PresentUnchanged(serializedObject.FindProperty($"Actions.Array.data[{refBrokeni}].atTargetAnimation"));
            if (action.Type == CardBattleActionType.Battle)
                PresentUnchanged(serializedObject.FindProperty($"Actions.Array.data[{refBrokeni}].battleEffect"));
            if (action.Type == CardBattleActionType.Condition)
                PresentUnchanged(serializedObject.FindProperty($"Actions.Array.data[{refBrokeni}].conditionData"));
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;
    }
    
    private void PresentUnchanged(SerializedProperty serializedProperty)
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedProperty, includeChildren: true);
        serializedObject.ApplyModifiedProperties();
    }
    
    private void PresentLabelsWithControls(string label, Action<GenericMenu> addToGenericMenu)
    {
        EditorGUILayout.LabelField(label);
        var clickArea =  GUILayoutUtility.GetLastRect();
        var current = Event.current;
        if (clickArea.Contains(current.mousePosition) && current.type == EventType.ContextClick)
        {
            GenericMenu menu = new GenericMenu();
            addToGenericMenu(menu);
            menu.ShowAsContext();
            current.Use(); 
        }
    }
}

#endif
