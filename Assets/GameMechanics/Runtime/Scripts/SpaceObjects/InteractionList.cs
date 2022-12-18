using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionList", menuName = "ScriptableObjects/InteractionList", order = 1)]
public class InteractionList : ScriptableObject
{
    public List<Type> interactiveTypes;
    public string[][] calledFunc;

    public void Init()
    {
        Type type = typeof(IInteractiveSpaceObject);
        IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p))
            .Where(t => t != type);


        interactiveTypes= new List<Type>(types);
        calledFunc = new string[interactiveTypes.Count][];
        for (int i = 0; i < calledFunc.Length; i++)
        {
            string[] reacFunc = new string[calledFunc.Length];
            for (int j = 0; j < reacFunc.Length; j++) reacFunc[j] = "";
            calledFunc[i] = reacFunc;
        }
    }
}
