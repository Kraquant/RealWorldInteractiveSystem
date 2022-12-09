using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayer
{
    public delegate void playerStateEvent();
    public event playerStateEvent OnPlayerDeath;
    public event playerStateEvent OnPlayerWin;
    public bool IsAlive { get;}
    public bool HasWon { get;}
}
