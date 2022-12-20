using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class LevelGoal : SpaceObject, IInteractiveSpaceObject
{
    [SerializeField] InteractionList _interactionList;
    public InteractionList ReferencedList { get => _interactionList; set => _interactionList = value; }
    public static string[] ReactionFunctions { get => new string[] { "Hold", "Destroy", "GetPushed" }; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        (string, string) interaction = SpaceUtilities.Utilities.GetReaction(this, collision.gameObject.GetComponent<IInteractiveSpaceObject>());

        switch (interaction.Item1)
        {
            case "GetPushed":
                throw new System.NotImplementedException();
            case "Hold":
                break;
            case "Destroy":
                base.DestroySpaceObject();
                break;
            default:
                throw new System.NotImplementedException();
        }
    }
}
