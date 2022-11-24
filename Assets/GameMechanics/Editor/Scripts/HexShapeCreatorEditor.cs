using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(HexShapeCreator))]
public class HexShapeCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void OnEnable()
    {
        SceneView.beforeSceneGui += OnSceneViewBeforeSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.beforeSceneGui -= OnSceneViewBeforeSceneGUI;
    }

    private void OnSceneViewBeforeSceneGUI(SceneView sceneView)
    {
        HexShapeCreator script = (HexShapeCreator)target;

        if (Application.isPlaying) return;

        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Vector3.Dot(ray.direction, Vector3.forward) == 0) script.target = Vector3.zero;
        else
        {
            Vector3 n = Vector3.forward;
            Vector3 D = Vector3.zero;
            float lambda = Vector3.Dot(D - ray.origin, n) / Vector3.Dot(ray.direction, n);

            script.target = ray.origin + lambda * ray.direction;
        }
    }
}
