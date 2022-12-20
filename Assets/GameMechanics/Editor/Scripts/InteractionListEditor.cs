using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(InteractionList))]
public class InteractionListEditor : Editor
{
    private bool _isSelectingReacFunc;
    private Vector2Int _reacFuncSelectedIndex;
    private string currentReacFuncOption;

    private bool _isSelectingReacOrder = false;
    private Vector2Int _reacOrderSelectedIndex;
    private string _currentReacOrderOption;

    private float _arrayWidth;
    private float _arrayHeight;

    

    private void OnEnable()
    {
        _isSelectingReacFunc = false;
        _isSelectingReacOrder = false;
        _reacFuncSelectedIndex = new Vector2Int(-1, -1);
        _reacOrderSelectedIndex = new Vector2Int(-1, -1);

        _arrayWidth = 100.0f;
        _arrayHeight = 50.0f;
    }

    public override void OnInspectorGUI()
    {
        InteractionList script = (InteractionList)target;

        if (script.InteractiveTypes == null || script.InteractiveTypes.Count == 0)
        {
            if (GUILayout.Button("Init"))
            {
                script.Init();
            }
            return;
        }

        if (GUILayout.Button("Refresh")) script.RefreshForNewInteractiveObjects();

        _arrayWidth = GUILayout.HorizontalScrollbar(_arrayWidth, 1.0f, 10.0f, 100.0f);
        _arrayHeight = GUILayout.HorizontalScrollbar(_arrayHeight, 1.0f, 10.0f, 100.0f);
        SetFuncReac(script);

        GUILayout.Space(100);
        SetFuncOrder(script);

        EditorUtility.SetDirty(script);
    }

    private void SetFuncReac(InteractionList script)
    {
        if (!_isSelectingReacFunc)
        {


            Grid.DisplayGrid(GetReacFuncNamesArray(script), out _reacFuncSelectedIndex, _arrayWidth, _arrayHeight);
            if (_reacFuncSelectedIndex.x > 0 && _reacFuncSelectedIndex.y > 0)
            {
                _isSelectingReacFunc = true;
                _reacFuncSelectedIndex.x--;
                _reacFuncSelectedIndex.y--;

                currentReacFuncOption = script.CalledFunc[_reacFuncSelectedIndex.x, _reacFuncSelectedIndex.y];
            }
        }
        else
        {
            GUILayout.Label("Now editing : " + _reacFuncSelectedIndex.x + ";" + _reacFuncSelectedIndex.y);

            //Get the options list
            string[] reacFuncs = script.GetKnownTypeReactionFunctions(script.InteractiveTypes[_reacFuncSelectedIndex.x]);
            List<string> options = new List<string>();
            options.AddRange(reacFuncs);
            options.Add("Custom");

            //Get the current option index


            GUILayout.BeginHorizontal();
            currentReacFuncOption = GUILayout.TextField(currentReacFuncOption);
            int popUpIndex = Array.FindIndex(reacFuncs, t => t == currentReacFuncOption);
            if (popUpIndex < 0) popUpIndex = options.Count - 1;
            popUpIndex = EditorGUILayout.Popup(popUpIndex, options.ToArray());
            if (popUpIndex != options.Count - 1) currentReacFuncOption = options[popUpIndex];

            GUILayout.EndHorizontal();

            if (GUILayout.Button("Ok"))
            {

                script.CalledFunc[_reacFuncSelectedIndex.x, _reacFuncSelectedIndex.y] = currentReacFuncOption;

                _reacFuncSelectedIndex = new Vector2Int(-1, -1);
                _isSelectingReacFunc = false;
            }
        }
    }
    private void SetFuncOrder(InteractionList script)
    {
        if (!_isSelectingReacOrder)
        {

            Grid.DisplayGrid(GetReacOrderArray(script), out _reacOrderSelectedIndex, _arrayWidth, _arrayHeight);
            if (_reacOrderSelectedIndex.x > 0 && _reacOrderSelectedIndex.y > 0)
            {
                _isSelectingReacOrder = true;
                _reacOrderSelectedIndex.x--;
                _reacOrderSelectedIndex.y--;

                _currentReacOrderOption = script.CallOrder[_reacOrderSelectedIndex.x, _reacOrderSelectedIndex.y];
            }
        }
        else
        {
            GUILayout.Label("Now editing : " + _reacOrderSelectedIndex.x + ";" + _reacOrderSelectedIndex.y);
            string[] options = new string[] { "First", "Last", "Same" };
            int popUpIndex = Array.FindIndex(options, t => t == _currentReacOrderOption);
            if (popUpIndex < 0) throw new System.NotImplementedException();

            popUpIndex = EditorGUILayout.Popup(popUpIndex, options.ToArray());
            _currentReacOrderOption = options[popUpIndex];

            if (GUILayout.Button("Ok"))
            {

                script.ChangeCallOrder(_reacOrderSelectedIndex, options[popUpIndex]);

                _reacOrderSelectedIndex = new Vector2Int(-1, -1);
                _isSelectingReacOrder = false;
            }
        }
    }

    private string[][] GetReacFuncNamesArray(InteractionList list)
    {
        int arraySize = list.InteractiveTypes.Count + 1;

        string[][] res = new string[arraySize][];
        res[0] = new string[arraySize];
        res[0][0] = "Line reaction to column";
        for (int i = 1; i < arraySize; i++) res[0][i] = list.InteractiveTypes[i - 1];

        for (int i = 1; i < arraySize; i++)
        {
            string[] line = new string[arraySize];

            line[0] = list.InteractiveTypes[i - 1];
            for (int j = 1; j < arraySize; j++)
            {
                line[j] = list.CalledFunc[i - 1, j - 1];
            }
            res[i] = line;
        }
        return res;
    }

    private string[][] GetReacOrderArray(InteractionList list)
    {
        int arraySize = list.InteractiveTypes.Count + 1;

        string[][] res = new string[arraySize][];
        res[0] = new string[arraySize];
        res[0][0] = "Line reaction to column";
        for (int i = 1; i < arraySize; i++) res[0][i] = list.InteractiveTypes[i - 1];

        for (int i = 1; i < arraySize; i++)
        {
            string[] line = new string[arraySize];

            line[0] = list.InteractiveTypes[i - 1];
            for (int j = 1; j < arraySize; j++)
            {
                line[j] = list.CallOrder[i - 1, j - 1];
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

    public static void DisplayGrid(string[][] buttonNames, out Vector2Int selectedIndex, float width = 100.0f, float height = 50.0f)
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

