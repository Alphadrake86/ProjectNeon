﻿using UnityEditor;
using UnityEngine;

class BattleAssetMenu
{

    [MenuItem("Assets/Create/Battle/Card Play Zone")]
    static void CardPlayZone() => Create<CardPlayZone>();
    [MenuItem("Assets/Create/Battle/Card Play Zones")]
    static void CardPlayZones() => Create<CardPlayZones>();
    [MenuItem("Assets/Create/Battle/Enemy Area")]
    static void EnemyArea() => Create<EnemyArea>();

    private static void Create<T>() where T : ScriptableObject
    {
        var asset = ScriptableObject.CreateInstance<T>();
        var path = AssetDatabase.GetAssetPath(Selection.activeObject);
        path += $"/New{typeof(T).Name}.asset";
        ProjectWindowUtil.CreateAsset(asset, path);
    }
}