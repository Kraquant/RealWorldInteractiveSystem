using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Asteroid : SpaceObject, ITurnBasedObject
{
    public enum AsteroidAction
    {
        OO,
        O1,
        O2,
        O3,
        O4,
        O5,
        O6
    };

    #region Protected Attributes
    
    protected AsteroidAction _nextAsteroidAction = AsteroidAction.O1;

    protected bool _asteroidMoving;
    protected float _targetPosSpeed;
    protected Vector3 _targetPos;

    protected float _currentTerrainCellsize;
    protected float _asteroidSpeed = 0.5f; //Time that it takes for the asteroid to move 
    #endregion

    public int TurnPriority { get => 1; set => throw new System.NotImplementedException(); }

    public AsteroidAction NextAsteroidAction
    {
        get => _nextAsteroidAction;
        set => _nextAsteroidAction = value;
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

    #region ActionType Conversion
    protected Action AsteroidActionToSpaceAction(int asteroidAction)
    {
        return asteroidAction switch
        {
            0 => Action.Hold,
            1 => Action.Front,
            2 => Action.Right,
            3 => Action.RightBehind,
            4 => Action.Back,
            5 => Action.LeftBehind,
            6 => Action.Left,
            _ => throw new System.NotImplementedException(),
        };
    }

    protected Action AsteroidActionToSpaceAction(AsteroidAction asteroidAction)
    {
        return AsteroidActionToSpaceAction((int)asteroidAction + 1);
    } 
    #endregion

}
