#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpaceTerrain))]
public class SpaceTerrainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SpaceTerrain script = (SpaceTerrain)target;
        GUILayout.TextField("Terrain size: " + script.Size.ToString());
        GUILayout.TextField("Cell size: " + script.CellSize.ToString());
        serializedObject.Update();
    }
}
#endif