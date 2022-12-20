using SpaceUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "InteractionList", menuName = "ScriptableObjects/InteractionList", order = 1)]
public class InteractionList : ScriptableObject
{
    [SerializeField] List<string> _interactiveTypes; // Contains Types
    [SerializeField] Matrix<string> _calledFunc;
    [SerializeField] Matrix<string> _callOrder;


    public void Init()
    {
        Type type = typeof(IInteractiveSpaceObject);
        IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p))
            .Where(t => t != type);


        InteractiveTypes = new List<string>(types.Select(t => t.FullName));
        int ITCount = InteractiveTypes.Count;

        CalledFunc = new Matrix<string>(ITCount, "");
        CallOrder = new Matrix<string>(ITCount, "Same");
    }

    public void RefreshForNewInteractiveObjects()
    {
        Type type = typeof(IInteractiveSpaceObject);
        IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p))
            .Where(t => t != type);

        List<string> newInteractiveTypes = new List<string>(
            types.Select(t => t.FullName)
            .ToList()
            .RemoveAll(x => !_interactiveTypes.Any(y => y == x)));

        if (newInteractiveTypes.Count == 0) return;
        
        _interactiveTypes.AddRange(newInteractiveTypes);

        CalledFunc.Expand(newInteractiveTypes.Count, "");
        CallOrder.Expand(newInteractiveTypes.Count, "Same");
    }

    public readonly string[] knownTypes = new string[]
    {
        typeof(BumpyAsteroid).FullName,
        typeof(Spaceship).FullName,
        typeof(GhostAsteroid).FullName,
    };

    public readonly string[][] knownTypesReactionFunctions = new string[][]
    {
        BumpyAsteroid.ReactionFunctions,
        Spaceship.ReactionFunctions,
        GhostAsteroid.ReactionFunctions,
    };

    public List<string> InteractiveTypes { get => _interactiveTypes; set => _interactiveTypes = value; }
    public Matrix<string> CalledFunc { get => _calledFunc; set => _calledFunc = value; }
    public Matrix<string> CallOrder { get => _callOrder; set => _callOrder = value; }

    public string[] GetKnownTypeReactionFunctions(string type)
    {
        int index = Array.FindIndex(knownTypes, t => t == type);
        if (index == -1) return new string[0];
        return knownTypesReactionFunctions[index];
    }

    public void ChangeCallOrder(Vector2Int index, string order)
    {
        if (index.x >= CallOrder.Rows || index.y >= CallOrder.Rows)
            throw new System.NotImplementedException();

        if (index.x == index.y) return;

        switch (order)
        {
            case "First":
                CallOrder[index.x, index.y] = "First";
                CallOrder[index.y, index.x] = "Last";
                break;
            case "Last":
                CallOrder[index.x, index.y] = "Last";
                CallOrder[index.y, index.x] = "First";
                break;
            case "Same":
                CallOrder[index.x, index.y] = "Same";
                CallOrder[index.y, index.x] = "Same";
                break;
            default:
                throw new System.NotImplementedException();
        }
    }
}
