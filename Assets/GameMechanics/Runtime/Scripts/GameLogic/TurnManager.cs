using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TurnManager : MonoBehaviour
{
    //TBO : Turn Based Object
    public enum CollisionType
    {
        None,
        Terrain,
        Object
    }

    [SerializeField] SpaceTerrain _terrain;
    [SerializeField] List<GameObject> _objectsGO;
    private ITurnBasedObject[] _objects; //Can't be serialized
    public ITurnBasedObject[] Objects { get => _objects;}
    
    public bool IsPlayingTurn { get; private set; }
    public SpaceTerrain Terrain { get => _terrain; set => _terrain = value; }

    private void Start()
    {
        //Creating the interface list
        CreateInterfaceList();
        Debug.Log("Number of objects: " + _objects.Length);
    }

    public async void PlayTurnAsync()
    {
        if (IsPlayingTurn)
        {
            Debug.LogError("Already playing a turn");
            return;
        }
        IsPlayingTurn = true;

        List<List<ITurnBasedObject>> itemsByPriority = SortItemsByPriority();

        foreach (List<ITurnBasedObject> itemsList in itemsByPriority)
        {
            IEnumerable<Task> tasks = itemsList.Select(async item =>
            {
                try
                {
                    await item.PlayTurnAsync(this);
                }
                catch (System.Exception e)
                {
                    Debug.LogError(e.Message);
                }
            });
            await Task.WhenAll(tasks);
        }

        IsPlayingTurn = false;
        Debug.Log("Turn is over");
    }

    private List<List<ITurnBasedObject>> SortItemsByPriority()
    {
        List<List<ITurnBasedObject>> itemsByPriority = new List<List<ITurnBasedObject>>();
        itemsByPriority.Add(new List<ITurnBasedObject>());

        IOrderedEnumerable<ITurnBasedObject> sortedObjects = _objects.OrderBy(i => i.TurnPriority);

        int currentOrder = sortedObjects.First().TurnPriority;
        foreach (ITurnBasedObject item in sortedObjects)
        {
            if (item.TurnPriority != currentOrder) itemsByPriority.Add(new List<ITurnBasedObject>());
            itemsByPriority.Last().Add(item);
        }

        return itemsByPriority;
    }

    public void CheckCollision(HexCoordinates coords, out CollisionType collision, out SpaceObject collidedObject)
    {
        //Check collision with terrain
        if (!Terrain.TerrainShape.Contains(coords))
        {
            collision = CollisionType.Terrain;
            collidedObject = null;
            return;
        }

        //Check collision with terrain objects
        foreach (ITurnBasedObject turnObject in _objects)
        {
            SpaceObject spaceObject = turnObject as SpaceObject;

            if (spaceObject != null)
            {
                foreach (HexCoordinates shapeCoord in spaceObject.Shape)
                {
                    if (coords == shapeCoord + spaceObject.Center)
                    {
                        collision = CollisionType.Object;
                        collidedObject = spaceObject;
                        return;
                    }
                }
            }
        }

        //No collision found
        collision = CollisionType.None;
        collidedObject=null;
    }

    #region Serialization Deserialization
    public void SetTurnBasedObjects(ITurnBasedObject[] objects)
    {
        _objectsGO = new List<GameObject>();
        if (Application.isPlaying) throw new System.Exception("Can't edit objects at runtime");
        foreach (ITurnBasedObject objInterface in objects)
        {
            if (objInterface != null) _objectsGO.Add(objInterface.gameObject);
        }
    }
    public void CreateInterfaceList()
    {
        List<ITurnBasedObject> TBOInterfaces = new List<ITurnBasedObject>();
        foreach (GameObject TBOGO in _objectsGO)
        {
            ITurnBasedObject TBOInterface = TBOGO.GetComponent<ITurnBasedObject>();
            if (TBOInterface != null) TBOInterfaces.Add(TBOInterface);
        }
        _objects = TBOInterfaces.ToArray();
    }
    #endregion
}