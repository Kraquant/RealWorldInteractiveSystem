using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionList", menuName = "ScriptableObjects/InteractionList", order = 1)]
public class InteractionList : ScriptableObject
{
    public Func<bool> test;
    public bool lol;
    public delegate void InteractionFunction();
    public InteractionFunction function;
    public Action unityAction;

}
