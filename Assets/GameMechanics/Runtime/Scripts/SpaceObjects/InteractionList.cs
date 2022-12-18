using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionList", menuName = "ScriptableObjects/InteractionList", order = 1)]
public class InteractionList : ScriptableObject
{
    [SerializeField] List<Type> _interactiveTypes;
    [SerializeField] string[][] _calledFunc;
    [SerializeField] string[][] _callOrder;


    public void Init()
    {
        Type type = typeof(IInteractiveSpaceObject);
        IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p))
            .Where(t => t != type);


        InteractiveTypes= new List<Type>(types);
        int ITCount = InteractiveTypes.Count;

        CalledFunc = new string[ITCount][];
        CallOrder = new string[ITCount][];

        for (int i = 0; i < ITCount; i++)
        {
            //For reaction Function
            string[] reacFunc = new string[ITCount];
            for (int j = 0; j < ITCount; j++) reacFunc[j] = "";
            CalledFunc[i] = reacFunc;

            //For reaction Order
            string[] orderFunc= new string[ITCount];
            for (int j = 0; j < ITCount; j++) orderFunc[j] = "Same";
            CallOrder[i] = orderFunc;

        }
    }

    public readonly Type[] knownTypes = new Type[]
    {
        typeof(BumpyAsteroid),
        typeof(Spaceship),
    };

    public readonly string[][] knownTypesReactionFunctions = new string[][]
    {
        BumpyAsteroid.ReactionFunctions,
        Spaceship.ReactionFunctions,
    };

    public List<Type> InteractiveTypes { get => _interactiveTypes; set => _interactiveTypes = value; }
    public string[][] CalledFunc { get => _calledFunc; set => _calledFunc = value; }
    public string[][] CallOrder { get => _callOrder; set => _callOrder = value; }

    public string[] GetKnownTypeReactionFunctions(Type type)
    {
        int index = Array.FindIndex(knownTypes, t => t==type);
        if (index == -1) return new string[0];
        return knownTypesReactionFunctions[index];
    }

    public void ChangeCallOrder(Vector2Int index, string order)
    {
        if (index.x >= CallOrder.Length || index.y >= CallOrder.Length)
            throw new System.NotImplementedException();

        if (index.x == index.y) return;

        switch (order)
        {
            case "First":
                CallOrder[index.x][index.y] = "First";
                CallOrder[index.y][index.x] = "Last";
                break;
            case "Last":
                CallOrder[index.x][index.y] = "Last";
                CallOrder[index.y][index.x] = "First";
                break;
            case "Same":
                CallOrder[index.x][index.y] = "Same";
                CallOrder[index.y][index.x] = "Same";
                break;
            default:
                throw new System.NotImplementedException();
        }
    }
}
