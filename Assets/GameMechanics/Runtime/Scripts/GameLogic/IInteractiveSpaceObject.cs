using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractiveSpaceObject
{
    public Type GetType();
    public static string[] ReactionFunctions { get;}

    InteractionList ReferencedList { get; set; }
}
