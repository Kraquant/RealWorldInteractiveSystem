using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Spaceship : SpaceObject, ITurnBasedObject, IPlayer, IInteractiveSpaceObject
{
    public event IPlayer.playerStateEvent OnPlayerDeath;
    public event IPlayer.playerStateEvent OnPlayerWin;

    #region Public Properties
    public int TurnPriority { get => 10; set => throw new System.NotImplementedException(); }
    public SpaceObject.Action NextAction { get; set; }
    public bool IsAlive => _isAlive;
    public bool HasWon => _hasWon;

    public static string[] ReactionFunctions { get => new string[] { "Destroy" };}
    public InteractionList ReferencedList { get => _interactionList; set => _interactionList = value; }
    #endregion

    #region Private Attributes
    //State attributes
    [SerializeField] InteractionList _interactionList;

    private bool _isAlive;
    private bool _hasWon;

    //Spaceship movement
    private bool _spaceshipMoving;
    private float _targetPosSpeed;
    private float _targetRotSpeed;
    private Vector3 _targetPos;
    private Quaternion _targetRot;
    #endregion


    private void Awake()
    {
        _hasWon = false;
        _isAlive = true;
    }
    #region Play Turn
    public async Task<bool> PlayTurnAsync(TurnManager turnManager)
    {
        //Check for possible movement
        turnManager.CheckCollision(PreviewNextCoordinate(NextAction).Item1, out TurnManager.CollisionType collision, out SpaceObject collided);
        if (collision == TurnManager.CollisionType.Object && collided == this) collision = TurnManager.CollisionType.None;

        switch (collision)
        {
            case TurnManager.CollisionType.None:
            case TurnManager.CollisionType.Object:
                MoveCoordinate(NextAction);
                await UpdateSpaceObjectTransformAsync(turnManager.Terrain.CellSize, .5f); // CellSizeToDefine;
                break;
            case TurnManager.CollisionType.Terrain: //The space ship does not move
                break;

            default:
                break;
        }
        return true;
    }

    public async Task<bool> UpdateSpaceObjectTransformAsync(float cellSize, float moveTime)
    {
        _targetPos = 2 * cellSize * Center.GetVector3Position();
        _targetRot = Quaternion.FromToRotation(Vector3.right, HexCoordinates.direction_vectors[(int)ObjectOrientation].GetVector3Position());

        _targetPosSpeed = Vector3.Distance(this.transform.position, _targetPos) / moveTime;
        _targetRotSpeed = Quaternion.Angle(this.transform.rotation, _targetRot) / moveTime;


        CancellationTokenSource cts = new CancellationTokenSource();
        _spaceshipMoving = true;

        await SpaceUtilities.Utilities.WaitUntilAsync(() => _spaceshipMoving == false, 100, cts.Token);

        return true;
    }

    private void Update()
    {
        if (_spaceshipMoving)
        {
            if (transform.position != _targetPos || transform.rotation != _targetRot)
            {
                transform.SetPositionAndRotation(
                    Vector3.MoveTowards(transform.position, _targetPos, _targetPosSpeed * Time.deltaTime),
                    Quaternion.RotateTowards(transform.rotation, _targetRot, _targetRotSpeed * Time.deltaTime));
            }
            else _spaceshipMoving = false;
        }
    }
    #endregion

    #region Collisions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        (string, string) interaction = SpaceUtilities.Utilities.GetReaction(this, collision.gameObject.GetComponent<IInteractiveSpaceObject>());

        switch (interaction.Item1)
        {
            case "Destroy":
                Destroy();
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    private void Destroy()
    {
        OnPlayerDeath?.Invoke();
        Destroy(gameObject);
    }
    #endregion
}
