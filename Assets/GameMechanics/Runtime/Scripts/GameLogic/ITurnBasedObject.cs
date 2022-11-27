using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public interface ITurnBasedObject
{
    public int TurnPriority { get; set; }
    Task<bool> PlayTurnAsync(TurnManager manager);
}
