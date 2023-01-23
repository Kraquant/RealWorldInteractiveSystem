using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using SpaceUtilities;

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
    [SerializeField] List<GameObject> _objectsGO; //Can be serialized
    private ITurnBasedObject[] _objects; //Can't be serialized
    private GameObject _visualizerGO;
    private HexVisualizer _visualizerScript;

    [Range(0, 2000)] public int turnCellTime;
    public Material turnCellMat;
    public ITurnBasedObject[] Objects { get => _objects;}
    
    public bool IsPlayingTurn { get; private set; }
    public SpaceTerrain Terrain { get => _terrain; set => _terrain = value; }

    private void Start()
    {
        CreateInterfaceList();
        //Debug.Log("Number of objects: " + _objects.Length);

        _visualizerGO = new GameObject();
        _visualizerGO.name = "Turn Manager Visualizer";
        _visualizerGO.transform.parent = this.transform;
        _visualizerScript = _visualizerGO.AddComponent<HexVisualizer>();
        _visualizerScript.displayMat = turnCellMat;

    }

    public async Task<bool> PlayTurnAsync()
    {
        if (IsPlayingTurn)
        {
            Debug.LogError("Already playing a turn");
            return false;
        }
        IsPlayingTurn = true;

        ITurnBasedObject[][] itemsByPriority = SortItemsByPriority();
        foreach (ITurnBasedObject[] itemsList in itemsByPriority)
        {
            var itemsListC = itemsList.Where(c => c != null).ToArray();// Cleaned items list

            await ShowSpacePositionAsync(itemsListC, turnCellTime);
            IEnumerable<Task> tasks = itemsListC.Select(async item =>
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
            DestroyOutOfTerrainObjects();
        }
        DestroyOutOfTerrainObjects();
        IsPlayingTurn = false;
        Debug.Log("Turn is over");
        return true;
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
    
    private void DestroyOutOfTerrainObjects()
    {
        foreach (var so in _objects)
        {
            SpaceObject SOScript = (SpaceObject)so;
            if (SOScript != null)
            {
                if (!Terrain.TerrainShape.Contains(SOScript.Center)) SOScript.DestroySpaceObject();
            }
        }
        _objects = _objects.Where(c => c != null).ToArray(); // Clean List
    }

    private async Task<bool> ShowSpacePositionAsync(ITurnBasedObject[] items, int time)
    {
        if (time == 0) return true;

        HashSet<HexCoordinates> coords = new HashSet<HexCoordinates>();
        foreach (ITurnBasedObject tbo in items)
        {
            SpaceObject tboCast = (SpaceObject)tbo;    
            if (tboCast != null)
            {
                coords.Add(tboCast.Center);
            }
        }

        _visualizerScript.CreateShape(coords, Terrain.CellSize);
        await Task.Delay(time);
        _visualizerScript.ClearShape();
        return true;
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

    #region Private utility methods
    private ITurnBasedObject[][] SortItemsByPriority()
    {
        _objects = _objects.Where(c => c != null).ToArray(); //Clean list

        List<List<ITurnBasedObject>> itemsByPriority = new List<List<ITurnBasedObject>>();
        itemsByPriority.Add(new List<ITurnBasedObject>());

        IOrderedEnumerable<ITurnBasedObject> sortedObjects = _objects.OrderBy(i => i.TurnPriority);

        int currentOrder = sortedObjects.First().TurnPriority;
        foreach (ITurnBasedObject item in sortedObjects)
        {
            if (item.TurnPriority != currentOrder)
            {
                itemsByPriority.Add(new List<ITurnBasedObject>());
                currentOrder= item.TurnPriority;
            }
            itemsByPriority.Last().Add(item);
        }
        return Utilities.ToArrayArray(itemsByPriority);
    } 
    #endregion
}