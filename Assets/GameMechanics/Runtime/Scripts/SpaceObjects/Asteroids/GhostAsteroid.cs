using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAsteroid : Asteroid, IInteractiveSpaceObject
{
    [SerializeField] InteractionList _interactionList;
    public InteractionList ReferencedList { get => _interactionList; set => _interactionList = value; }
    public static string[] ReactionFunctions { get => new string[] { "DashThrough", "Hold", "Destroy", "GetPushed" }; }

    public override int TurnPriority { get => 3; set => throw new System.NotImplementedException(); }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        (string, string) interaction = SpaceUtilities.Utilities.GetReaction(this, collision.gameObject.GetComponent<IInteractiveSpaceObject>());

        switch (interaction.Item1)
        {
            case "DashThrough":
                DashThrough();
                break;
            case "Hold":
                break;
            case "Destroy":
                base.DestroyAsteroid();
                break;
            case "Get Pushed":
                Center = collision.gameObject.GetComponent<HeavyAsteroid>().GetPushed();
                UpdateAsteroidTransform(_currentTerrainCellsize, _asteroidSpeed);
                break;
            default:
                throw new System.NotImplementedException();
        }

    }

    private void DashThrough()
    {
        if (!_asteroidMoving) return;
        Center = PreviewNextCoordinate(AsteroidActionToSpaceAction(NextAsteroidAction)).Item1;
        UpdateAsteroidTransform(_currentTerrainCellsize, _asteroidSpeed);
    }
}
