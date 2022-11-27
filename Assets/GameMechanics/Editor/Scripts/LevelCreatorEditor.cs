using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomEditor(typeof(LevelCreator))]
public class LevelCreatorEditor : Editor
{
    SerializedProperty e_terrainShape;
    SerializedProperty e_objectsList;
    SerializedProperty e_vizualizeTerrain;

    private bool _terrainFoldout;
    private bool _spaceObjectsFoldout;

    private void OnEnable()
    {
        SceneView.beforeSceneGui += OnSceneViewBeforeSceneGUI;

        e_terrainShape = serializedObject.FindProperty("_terrainShape");
        e_vizualizeTerrain = serializedObject.FindProperty("_vizualizeTerrain");
        e_objectsList = serializedObject.FindProperty("objectsList");

        _terrainFoldout = true;
        _spaceObjectsFoldout = true;
    }
    private void OnDisable()
    {
        SceneView.beforeSceneGui -= OnSceneViewBeforeSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        LevelCreator script = (LevelCreator)target;

        WriteTitle("----------LEVEL CREATOR----------", 18);

        //Terrain properties
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        CreateFoldout("Terrain", 18, ref _terrainFoldout);
        if (_terrainFoldout)
        {
            GUILayout.Space(20);
            if (GUILayout.Button("Get terrain from creator")) script.GetTerrainFromCreator();
            EditorGUILayout.PropertyField(e_vizualizeTerrain, new GUIContent("Vizualize terrain"));
            GUILayout.TextField("Terrain size: " + script.CurrentTerrainSize().ToString());
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        //Objects List
        CreateFoldout("Space Objects", 18, ref _spaceObjectsFoldout);
        
        if (_spaceObjectsFoldout)
        {
            GUILayout.Space(20);
            switch (script.placingState)
            {
                case LevelCreator.PlacingState.None:
                    DisplayObjects(ref script);
                    break;
                case LevelCreator.PlacingState.Placing:
                    GUILayout.Label("Now placing: " + "ObjectName");
                    GUILayout.Label("Right Click to cancel");
                    break;
                case LevelCreator.PlacingState.Orienting:
                    GUILayout.Label("Now orienting: " + "ObjectName");
                    GUILayout.Label("Right Click to cancel");
                    break;
                case LevelCreator.PlacingState.Deleting:
                    break;
                default:
                    break;
            }
            EditorGUILayout.PropertyField(e_objectsList, new GUIContent("Objects list"));
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneViewBeforeSceneGUI(SceneView sceneView)
    {
        if (Application.isPlaying) return; // Do not use this script when application is playing
        LevelCreator script = (LevelCreator)target;
        //Prevent from unselecting the object
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));


        //Update targeted position
        script.VectorTarget = ProjectMouseOnGround();

        //Manage mouse clicks
        switch (script.placingState)
        {
            case LevelCreator.PlacingState.None:
                break;
            case LevelCreator.PlacingState.Placing:

                //Monitor cancel
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    script.CancelPlacement();
                }
                //Monitor validation
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    script.ConfirmPlacement();
                }

                break;
            case LevelCreator.PlacingState.Orienting:
                //Monitor cancel
                if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
                {
                    script.CancelPlacement();
                }
                //Monitor validation
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    script.ConfirmOrientation();
                }

                break;
            case LevelCreator.PlacingState.Deleting:
                break;
            default:
                break;
        }
    }

    private void DisplayObjects(ref LevelCreator script)
    {
        if (script.objectsList == null) return;
        bool terrainIsSet = script.CurrentTerrainSize() > 0;
        string buttonText = terrainIsSet ? "Place" : "Define a terrain to place objects";

        foreach (SpaceObjectList objectList in script.objectsList)
        {
            if (objectList == null) continue;   
            GUILayout.Label(objectList.name);
            foreach (GameObject spaceObject in objectList.prefabs)
            {
                GUILayout.BeginHorizontal();
                GUILayout.TextField(spaceObject.name);

                
                if (GUILayout.Button(buttonText))
                {
                    if (terrainIsSet)
                    {
                        script.PlaceObject();
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
    private void WriteTitle(string title, int size)
    {
        GUIStyle labelStyle = GUI.skin.GetStyle("Label");
        var currentAlign = labelStyle.alignment;
        var currentFontSize = labelStyle.fontSize;

        labelStyle.alignment = TextAnchor.UpperCenter;
        labelStyle.fontSize = size;
        GUILayout.Label(new GUIContent(title));
        labelStyle.alignment = currentAlign;
        labelStyle.fontSize = currentFontSize;
    }
    private void CreateFoldout(string title, int fontsize, ref bool toggleRef)
    {
        GUIStyle labelStyle = GUI.skin.GetStyle("Foldout");
        int currentFontSize = labelStyle.fontSize;
        labelStyle.fontSize = fontsize;
        toggleRef = EditorGUILayout.Foldout(toggleRef, new GUIContent(title));
        labelStyle.fontSize = currentFontSize;
    }
    private Vector3 ProjectMouseOnGround()
    {
        //Casting mouse ray
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        if (Vector3.Dot(ray.direction, Vector3.forward) == 0) return Vector3.zero;
        Vector3 n = Vector3.forward;
        Vector3 D = Vector3.zero;
        float lambda = Vector3.Dot(D - ray.origin, n) / Vector3.Dot(ray.direction, n);

        return ray.origin + lambda * ray.direction;

    }
}
