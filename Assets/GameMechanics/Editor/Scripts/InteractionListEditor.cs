using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractionList))]
public class InteractionListEditor : Editor
{
    public int selGridInt = 10;
    public string[] selStrings = new string[] { "Grid 1", "Grid 2", "Lol", "Grid 4" };

    public override void OnInspectorGUI()
    {
        // use 2 elements in the horizontal direction
        selGridInt = GUI.SelectionGrid(new Rect(25, 25, 200, 100), selGridInt, selStrings, 3);

        serializedObject.ApplyModifiedProperties();
    }
}
