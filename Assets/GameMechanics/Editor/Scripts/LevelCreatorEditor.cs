using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.UIElements;
using UnityEditor.PackageManager.UI;

[CustomEditor(typeof(LevelCreator))]
public class LevelCreatorEditor : Editor
{
    SerializedProperty e_terrainShape;
    SerializedProperty e_objectsList;
    SerializedProperty e_vizualizeTerrain;
    SerializedProperty e_turnCellMat;
    SerializedProperty e_terrainDisplayMat;
    SerializedProperty e_interactionList;

    private bool _terrainFoldout;
    private bool _spaceObjectsFoldout;
    private bool _otherPropertiesFoldout;

    private void OnEnable()
    {
        SceneView.beforeSceneGui += OnSceneViewBeforeSceneGUI;

        e_terrainShape = serializedObject.FindProperty("_terrainShape");
        e_vizualizeTerrain = serializedObject.FindProperty("_vizualizeTerrain");
        e_objectsList = serializedObject.FindProperty("objectsList");
        e_turnCellMat = serializedObject.FindProperty("turnCellMat");
        e_terrainDisplayMat = serializedObject.FindProperty("terrainDisplayMat");
        e_interactionList = serializedObject.FindProperty("interactionList");

        _terrainFoldout = true;
        _spaceObjectsFoldout = true;
        _otherPropertiesFoldout = true;
    }
    private void OnDisable()
    {
        SceneView.beforeSceneGui -= OnSceneViewBeforeSceneGUI;
    }



    #region SceneGUI
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
    #endregion
    #region InspectorGUI
    public override void OnInspectorGUI()
    {
        LevelCreator script = (LevelCreator)target;

        WriteTitle("----------LEVEL CREATOR----------", 18);

        if (GUILayout.Button("Build level"))
        {
            script.BuildLevel();
        }

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
            GUILayout.Label("• Placed Space Objects", EditorStyles.largeLabel);
            GUILayout.Space(5);
            ShowPlacedObjectsList(ref script);

            GUILayout.Space(20);
            GUILayout.Label("• Placeable Space Objects", EditorStyles.largeLabel);
            GUILayout.Space(5);
            switch (script.placingState)
            {
                case LevelCreator.PlacingState.None:
                    ShowPlacableObjectsList(ref script);
                    break;
                case LevelCreator.PlacingState.Placing:
                    EditorGUILayout.HelpBox("Now placing | Right Click to cancel", MessageType.Info);
                    break;
                case LevelCreator.PlacingState.Orienting:
                    EditorGUILayout.HelpBox("Now orienting | Right Click to cancel", MessageType.Info);
                    break;
                case LevelCreator.PlacingState.Deleting:
                    break;
                default:
                    break;
            }
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Clean deleted elements")) script.CleanDeletedElements();
            if (GUILayout.Button("Check terrain changes")) script.CheckTerrainChanges();
            if (GUILayout.Button("Remove incompatible objects")) script.RemoveIncompatibleObjects();
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
            EditorGUILayout.PropertyField(e_objectsList, new GUIContent("Objects list"));
            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        }

        

        //Other Settings
        CreateFoldout("Other Properties", 18, ref _otherPropertiesFoldout);
        EditorGUILayout.PropertyField(e_turnCellMat, new GUIContent("Turn cell Mat"));
        EditorGUILayout.PropertyField(e_terrainDisplayMat, new GUIContent("Terrain Display Mat"));
        EditorGUILayout.PropertyField(e_interactionList, new GUIContent("Interaction list"));

        serializedObject.ApplyModifiedProperties();

    }
    private void ShowPlacableObjectsList(ref LevelCreator script, bool tab = false)
    {
        if (script.objectsList == null) return;
        bool terrainIsSet = script.CurrentTerrainSize() > 0;
        string buttonText = terrainIsSet ? "Place" : "Define a terrain to place objects";

        foreach (SpaceObjectList objectList in script.objectsList)
        {
            if (objectList == null) continue;
            string tabText = tab ? "\t" : "";
            GUILayout.Label(" - " + objectList.name, EditorStyles.boldLabel);
            foreach (GameObject spaceObject in objectList.prefabs)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(tabText + spaceObject.name);


                if (GUILayout.Button(buttonText))
                {
                    if (terrainIsSet)
                    {
                        script.PlaceObject(spaceObject);
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
    }
    private void ShowPlacedObjectsList(ref LevelCreator script, bool tab = false)
    {
        if (script.placedObjects is null) return;
        string tabText = tab ? "\t" : "";
        foreach ((GameObject,bool) placedGO in script.placedObjects)
        {
            if (placedGO.Item1 == null)
            {
                EditorGUILayout.HelpBox(tabText + "Deleted Game Object", MessageType.Warning);
            }
            else if (!placedGO.Item2)
            {
                EditorGUILayout.HelpBox(tabText + "The object position is no longer compatible with the terrain", MessageType.Error);
            }
            else
            {
                SpaceObject spaceObjScript = placedGO.Item1.GetComponent<SpaceObject>();

                GUILayout.BeginHorizontal();
                GUILayout.Label(
                    tabText +
                    spaceObjScript.name +
                    " - Coord: " + spaceObjScript.Center.ToString() +
                    " - Orient: " + spaceObjScript.ObjectOrientation.ToString());
                if (GUILayout.Button("Destroy"))
                {
                    DestroyImmediate(placedGO.Item1);
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
    #endregion
}
