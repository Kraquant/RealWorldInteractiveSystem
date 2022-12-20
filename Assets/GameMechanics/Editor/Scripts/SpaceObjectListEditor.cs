#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Linq;

[CustomEditor(typeof(SpaceObjectList))]
public class SpaceObjectListEditor : Editor
{
    SerializedProperty e_spaceObjectList;
    SerializedProperty e_listName;

    private void OnEnable()
    {
        e_spaceObjectList = serializedObject.FindProperty("prefabs");
        e_listName = serializedObject.FindProperty("listName");
    }
    public override void OnInspectorGUI()
    {
        SpaceObjectList script = (SpaceObjectList)target;

        EditorGUILayout.PropertyField(e_listName, new GUIContent("List Name"));
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        EditorGUILayout.LabelField("Space Object List ");
        EditorGUILayout.Space();
        EditorGUI.indentLevel += 2;
        for (int i = 0; i < e_spaceObjectList.arraySize; i++)
        {
            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(e_spaceObjectList.GetArrayElementAtIndex(i), new GUIContent(""));
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("Remove"))
            {
                e_spaceObjectList.DeleteArrayElementAtIndex(i);
            }
            GUILayout.EndHorizontal();
        }
        EditorGUI.indentLevel -= 2;

        var rect = GUILayoutUtility.GetRect(new GUIContent("Add"), EditorStyles.toolbarButton);
        if (GUI.Button(rect, new GUIContent("Add"), EditorStyles.toolbarButton))
        {
            var dropdown = new SpaceObjectsDropdown(new AdvancedDropdownState(), script);
            dropdown.Show(rect);
        }


        serializedObject.ApplyModifiedProperties();
    }
    

    class SpaceObjectsDropdown : AdvancedDropdown
    {
        private SpaceObjectList _script;
        private GameObject[] _prefabs;
        private int[] _prefabsID;

        public SpaceObjectsDropdown(AdvancedDropdownState state, SpaceObjectList script) : base(state)
        {
            _script = script;
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new AdvancedDropdownItem("Space Objects");

            _prefabs = FindAllSpaceObjects();
            _prefabsID = new int[_prefabs.Length];


            for (int i = 0; i < _prefabs.Length; i++)
            {
                AdvancedDropdownItem item = new AdvancedDropdownItem(_prefabs[i].name);
                _prefabsID[i] = item.id;
                root.AddChild(item);
            }

            return root;
        }

        private GameObject[] FindAllSpaceObjects()
        {
            var objects = AssetDatabase.FindAssets("t:prefab");
            List<GameObject> prefabs = new List<GameObject>();
            foreach (var item in objects)
            {
                var path = AssetDatabase.GUIDToAssetPath(item);
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab.GetComponent<SpaceObject>() != null) prefabs.Add(prefab);
            }
            return prefabs.ToArray();
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            if (!item.enabled) return;
            int prefabIndex = _prefabsID.ToList().FindIndex(x => x == item.id);
            _script.prefabs.Add(_prefabs[prefabIndex]);
        }
    }
}
#endif