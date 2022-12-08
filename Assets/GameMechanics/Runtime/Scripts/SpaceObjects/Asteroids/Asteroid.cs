using Codice.CM.Triggers;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Asteroid : SpaceObject, ITurnBasedObject
{
    private int _nextAsteroidAction = 1;

    protected bool _asteroidMoving;
    protected float _targetPosSpeed;
    protected Vector3 _targetPos;

    protected float _currentTerrainCellsize;
    protected float _asteroidSpeed = 0.5f; //Time that it takes for the asteroid to move

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
        _currentTerrainCellsize = turnManager.Terrain.CellSize;

        Action spaceAction = AsteroidActionToSpaceAction(NextAsteroidAction);
        turnManager.CheckCollision(PreviewNextCoordinate(spaceAction).Item1, out TurnManager.CollisionType collision, out SpaceObject collided);

        if (collision == TurnManager.CollisionType.Terrain)
        {
            return true;
        }

        Center = PreviewNextCoordinate(spaceAction).Item1;
        CancellationTokenSource cts = new CancellationTokenSource();
        UpdateAsteroidTransform(_currentTerrainCellsize, _asteroidSpeed);
        await SpaceUtilities.WaitUntilAsync(() => _asteroidMoving == false, 100, cts.Token);

        return true;
    }

    protected void UpdateAsteroidTransform(float cellSize, float moveTime)
    {
        _targetPos = 2 * cellSize * Center.GetVector3Position();
        _targetPosSpeed = Vector3.Distance(this.transform.position, _targetPos) / moveTime;
        _asteroidMoving = true;
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
    protected Action AsteroidActionToSpaceAction(int asteroidAction)
    {
        return asteroidAction switch
        {
            1 => Action.Front,
            2 => Action.Right,
            3 => Action.RightBehind,
            4 => Action.Back,
            5 => Action.LeftBehind,
            6 => Action.Left,
            _ => throw new System.NotImplementedException(),
        };
    }
}
