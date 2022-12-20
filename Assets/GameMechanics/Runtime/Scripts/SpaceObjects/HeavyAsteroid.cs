using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyAsteroid : Asteroid, IInteractiveSpaceObject
{
    [SerializeField] InteractionList _interactionList;
    public InteractionList ReferencedList { get => _interactionList; set => _interactionList = value; }
    public static string[] ReactionFunctions { get => new string[] { "Hold", "Destroy" }; }

    public override int TurnPriority { get => 1; set => throw new System.NotImplementedException(); }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        (string, string) interaction = SpaceUtilities.Utilities.GetReaction(this, collision.gameObject.GetComponent<IInteractiveSpaceObject>());

        switch (interaction.Item1)
        {
            case "Hold":
                break;
            case "Destroy":
                base.DestroyAsteroid();
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    public HexCoordinates GetPushed()
    {
        return PreviewNextCoordinate(AsteroidActionToSpaceAction(NextAsteroidAction)).Item1;
    }
}

