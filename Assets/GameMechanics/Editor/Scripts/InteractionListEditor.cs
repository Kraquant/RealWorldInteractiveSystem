using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Codice.Client.BaseCommands.Import.Commit;

[CustomEditor(typeof(InteractionList))]
public class InteractionListEditor : Editor
{
    private bool isSelecting;
    private Vector2Int selectedIndex;
    private float arrayWidth;
    private float arrayHeight;

    private void OnEnable()
    {
        isSelecting= false;
        selectedIndex = new Vector2Int(-1, -1);

        arrayWidth = 100.0f;
        arrayHeight = 50.0f;
    }

    public override void OnInspectorGUI()
    {
        InteractionList script = (InteractionList)target;

        if (script.interactiveTypes == null)
        {
            if (GUILayout.Button("Init"))
            {
                script.Init();
            }
            return;
        }


        if (!isSelecting)
        {
            arrayWidth = GUILayout.HorizontalScrollbar(arrayWidth, 1.0f, 10.0f, 100.0f);
            arrayHeight = GUILayout.HorizontalScrollbar(arrayHeight, 1.0f, 10.0f, 100.0f);

            Grid.DisplayGrid(GetNamesArray(script), out selectedIndex, arrayWidth, arrayHeight);
            if (selectedIndex.x > 0 && selectedIndex.y > 0)
            {
                isSelecting = true;
                selectedIndex.x--;
                selectedIndex.y--;
            }
        }
        else
        {
            GUILayout.Label("Now editing : " + selectedIndex.x + ";" + selectedIndex.y);
            string currentOption = script.calledFunc[selectedIndex.x][selectedIndex.y];

            List<string> args = new List<string>();
            args.Add("Not implemented");
            IInteractiveSpaceObject inter = (IInteractiveSpaceObject)script.interactiveTypes[selectedIndex.x];
          /*  args.AddRange();

            int selectedOption = EditorGUILayout.Popup(currentOption, options);

            script.calledFunc[selectedIndex.x][selectedIndex.y] = options[selectedOption];*/

            if (GUILayout.Button("Ok"))
            {
                selectedIndex = new Vector2Int(-1, -1);
                isSelecting= false;
            }
        }
    }



    private string[][] GetNamesArray(InteractionList list)
    {
        int arraySize = list.interactiveTypes.Count + 1;

        string[][] res = new string[arraySize][];
        res[0] = new string[arraySize];
        res[0][0] = "Line reaction to column";
        for (int i = 1; i < arraySize; i++) res[0][i] = list.interactiveTypes[i - 1].Name;

        for (int i = 1; i < arraySize; i++)
        {
            string[] line = new string[arraySize];

            line[0] = list.interactiveTypes[i-1].Name;
            for (int j = 1; j < arraySize; j++)
            {
                line[j] = list.calledFunc[i-1][j-1];
            }
            res[i] = line;
        }
        return res;
    }

    }

public static class Grid
{
    public static void DisplayGrid(int row, int col, out Vector2Int selectedIndex)
    {
        selectedIndex = new Vector2Int(-1, -1);
        for (int i = 0; i < row; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < col; j++)
            {
                Rect rect = GUILayoutUtility.GetRect(100.0f, 50.0f);
                if (GUI.Button(rect, "Button " + i + ";" + j))
                {
                    selectedIndex.x = i;
                    selectedIndex.y = j;
                }
            }
            GUILayout.EndHorizontal();
        }
    }

    public static void DisplayGrid(string[][] buttonNames,out Vector2Int selectedIndex, float width = 100.0f, float height = 50.0f)
    {
        selectedIndex = new Vector2Int(-1, -1);
        for (int i = 0; i < buttonNames.Length; i++)
        {
            GUILayout.BeginHorizontal();
            for (int j = 0; j < buttonNames[i].Length; j++)
            {
                Rect rect = GUILayoutUtility.GetRect(width, height);
                if (GUI.Button(rect, buttonNames[i][j]))
                {
                    selectedIndex.x = i;
                    selectedIndex.y = j;
                }
            }
            GUILayout.EndHorizontal();
        }
    }
}

