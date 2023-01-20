using UnityEngine;

public class BumpyAsteroid : Asteroid, IInteractiveSpaceObject
{
    [SerializeField] InteractionList _interactionList;
    public InteractionList ReferencedList { get => _interactionList; set => _interactionList = value; }
    public static string[] ReactionFunctions { get => new string[] { "Bump", "Hold", "Destroy", "GetPushed" }; }
    public override int TurnPriority { get => 2; set => throw new System.NotImplementedException(); }

    private Action InvertAction(Action action)
    {
        return action switch
        {
            Action.Hold => Action.Hold,
            Action.Front => Action.Back,
            Action.Right => Action.LeftBehind,
            Action.RightBehind => Action.Left,
            Action.Back => Action.Front,
            Action.LeftBehind => Action.Right,
            Action.Left => Action.RightBehind,
            _ => throw new System.NotImplementedException(),
        };
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        (string, string) interaction = SpaceUtilities.Utilities.GetReaction(this, collision.gameObject.GetComponent<IInteractiveSpaceObject>());

        switch (interaction.Item1)
        {
            case "Bump":
                Bump();
                break;
            case "Hold":
                break;
            case "Destroy":
                base.DestroySpaceObject();
                break;
            case "GetPushed":
                Center = collision.gameObject.GetComponent<HeavyAsteroid>().GetPushed();
                if (-_currentTerrainCellsize == 0.0f) _currentTerrainCellsize = FindObjectOfType<SpaceTerrain>().CellSize;
                UpdateAsteroidTransform(_currentTerrainCellsize, _asteroidSpeed);
                break;
            default:
                throw new System.NotImplementedException();
        }
        
    }

    private void Bump()
    {
        if (!_asteroidMoving) return;
        Action spaceAction = InvertAction(AsteroidActionToSpaceAction(NextAsteroidAction));
        Center = PreviewNextCoordinate(spaceAction).Item1;
        UpdateAsteroidTransform(_currentTerrainCellsize, _asteroidSpeed);
    }

}
