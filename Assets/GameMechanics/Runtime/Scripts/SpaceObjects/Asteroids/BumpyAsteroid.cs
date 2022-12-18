using UnityEngine;

public class BumpyAsteroid : Asteroid, IInteractiveSpaceObject
{
    [SerializeField] InteractionList _interactionList;
    public InteractionList ReferencedList { get => _interactionList; set => _interactionList = value; }
    public static string[] ReactionFunctions { get => new string[] { "Bump", "Hold", "Destroy" }; }

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
        ((IInteractiveSpaceObject)this).GetReaction(collision.gameObject.GetComponent<IInteractiveSpaceObject>());
        Bump();
    }

    private void Bump()
    {
        Action spaceAction = InvertAction(AsteroidActionToSpaceAction(NextAsteroidAction));
        Center = PreviewNextCoordinate(spaceAction).Item1;
        UpdateAsteroidTransform(_currentTerrainCellsize, _asteroidSpeed);
    }

}
