using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BumpyAsteroid : Asteroid
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Action spaceAction = InvertAction(AsteroidActionToSpaceAction(NextAsteroidAction));
        Center = PreviewNextCoordinate(spaceAction).Item1;
        UpdateAsteroidTransform(_currentTerrainCellsize, _asteroidSpeed);
    }

    private Action InvertAction(Action action)
    {
        return action switch
        {
            Action.Front => Action.Back,
            Action.Right => Action.LeftBehind,
            Action.RightBehind => Action.Left,
            Action.Back => Action.Front,
            Action.LeftBehind => Action.Right,
            Action.Left => Action.RightBehind,
            _ => throw new System.NotImplementedException(),
        };
    }

}
