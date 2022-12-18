using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractiveSpaceObject
{
    public Type GetType();
    public static string[] ReactionFunctions { get;}

    InteractionList ReferencedList { get; set; }


    public (string, string) GetReaction(IInteractiveSpaceObject other)
    {
        if (other.ReferencedList != ReferencedList) throw new System.NotImplementedException();

        int sourceIndex = ReferencedList.InteractiveTypes.IndexOf(GetType());
        int targetIndex = ReferencedList.InteractiveTypes.IndexOf(other.GetType());

        return (this.ReferencedList.CalledFunc[sourceIndex][targetIndex], this.ReferencedList.CallOrder[sourceIndex][targetIndex]);
    }
}
