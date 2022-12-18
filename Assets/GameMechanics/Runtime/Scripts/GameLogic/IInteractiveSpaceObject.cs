using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractiveSpaceObject
{
    static string[] ReactionFunctions { get;}

    InteractionList referencedList { get; set; }

}
