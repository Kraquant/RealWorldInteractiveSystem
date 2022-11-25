using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexShapeCreator))]
public class HexShapeCreatorEditor : Editor
{
    
    public bool isSelecting;
    private void Awake()
    {
        isSelecting = false;
    }

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
        if (Application.isPlaying) return; // Do not use this script when application is playing

        //Prevent from unselecting the object
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        HexShapeCreator script = (HexShapeCreator)target;
        //Update targeted position
        script.VectorTarget = ProjectMouseOnGround();


        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            isSelecting = true;
            script.addStatus = !script.cellList.Contains(script.HexTarget);
        }

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            isSelecting = false;
            script.AddNewCells();
        }

        if (isSelecting)
        {
            script.addList.Add(script.HexTarget);
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
}
