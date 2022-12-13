using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpaceObjectList", menuName = "ScriptableObjects/SpaceManager", order =1)]
public class SpaceObjectList : ScriptableObject
{
    public string listName;
    public List<GameObject> prefabs;
}
