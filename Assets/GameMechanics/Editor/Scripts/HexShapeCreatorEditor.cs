#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[CustomEditor(typeof(HexShapeCreator))]
public class HexShapeCreatorEditor : Editor
{
    public bool isSelecting;

    private void Awake()
    {
        isSelecting = false;
    }
    private void OnEnable()
    {
        SceneView.beforeSceneGui += OnSceneViewBeforeSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.beforeSceneGui -= OnSceneViewBeforeSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        HexShapeCreator script = (HexShapeCreator)target;

        WriteTitle("----------HEX SHAPE CREATOR----------", 18);
        ToggleButton(ref script.isActive);
        if (GUILayout.Button("Create new shape"))
        {
            script.cellList.Clear();
            isSelecting = false;
            script.AddNewCells();
        }    
        script.cellSize = EditorGUILayout.Slider(new GUIContent("Cell size"), script.CellSize, 0.1f, 100);

        serializedObject.ApplyModifiedProperties();
    }

    

    private void OnSceneViewBeforeSceneGUI(SceneView sceneView)
    {
        if (Application.isPlaying) return; // Do not use this script when application is playing
        HexShapeCreator script = (HexShapeCreator)target;
        if (!script.isActive) return;
        //Prevent from unselecting the object
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        
        //Update targeted position
        script.VectorTarget = ProjectMouseOnGround();


        if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
        {
            isSelecting = true;
            script.addStatus = !script.HexShape.Contains(script.HexTarget);
        }

        if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
        {
            isSelecting = false;
            script.AddNewCells();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
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
    private void ToggleButton(ref bool toggle)
    {
        if (toggle)
        {
            if (GUILayout.Button("Disable"))
            {
                toggle = false;
            }
        }
        else
        {
            if (GUILayout.Button("Enable"))
            {
                toggle = true;
            }
        }
    }

}
#endif