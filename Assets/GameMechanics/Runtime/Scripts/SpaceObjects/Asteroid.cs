using Codice.CM.Triggers;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Asteroid : SpaceObject, ITurnBasedObject
{
    private int _nextAsteroidAction = 1;

    private bool _asteroidMoving;
    private float _targetPosSpeed;
    private Vector3 _targetPos;


    public int TurnPriority { get => 1; set => throw new System.NotImplementedException(); }
    public int NextAsteroidAction
    {
        get => _nextAsteroidAction;
        set
        {
            if (value > 6 || value < 1) throw new System.Exception("Next value must be between 1 and 6");
            _nextAsteroidAction = value;
        }
    }

    public async Task<bool> PlayTurnAsync(TurnManager turnManager)
    {
        Action spaceAction = AsteroidActionToSpaceAction(NextAsteroidAction);
        turnManager.CheckCollision(PreviewNextCoordinate(spaceAction).Item1, out TurnManager.CollisionType collision, out SpaceObject collided);

        if (collision != TurnManager.CollisionType.Terrain)
        {
            Center = PreviewNextCoordinate(spaceAction).Item1;
            await UpdateSpaceObjectTransformAsync(turnManager.Terrain.CellSize, .5f);
        }
        return true;
    }

    public async Task<bool> UpdateSpaceObjectTransformAsync(float cellSize, float moveTime)
    {

        _targetPos = 2 * cellSize * Center.GetVector3Position();
       
        _targetPosSpeed = Vector3.Distance(this.transform.position, _targetPos) / moveTime;
        

        CancellationTokenSource cts = new CancellationTokenSource();
        _asteroidMoving = true;

        await SpaceUtilities.WaitUntilAsync(() => _asteroidMoving == false, 100, cts.Token);

        return true;
    }
    private void Update()
    {
        if (_asteroidMoving)
        {
            if (transform.position != _targetPos)
            {
                transform.position =
                    Vector3.MoveTowards(transform.position, _targetPos, _targetPosSpeed * Time.deltaTime);
            }
            else _asteroidMoving = false;
        }
    }

    private Action AsteroidActionToSpaceAction(int asteroidAction)
    {
        return asteroidAction switch
        {
            1 => Action.Front,
            2 => Action.Left,
            3 => Action.LeftBehind,
            4 => Action.Turn,
            5 => Action.RightBehind,
            6 => Action.Right,
            _ => throw new System.NotImplementedException(),
        };
    }
}
