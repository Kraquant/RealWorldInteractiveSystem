using UnityEngine;

[RequireComponent(typeof(Camera))]
public class ResponsiveCamera : MonoBehaviour
{
    private float _lerpFactor;
    private float _adaptTime;

    private float _originSize;
    private float _targetSize;
     
    private Vector3 _originPosition;
    private Vector3 _targetPosition;

    private Camera _camera;

    Vector3 movingDist;
    bool movingUp, movingLeft;
    [SerializeField] float zoomModifierSpeed = 0.0001f;
    [SerializeField] float cameraSpeed = 0.2f;
    private LevelManager _levelManager;

    private void Awake()
    {
        _lerpFactor = 1.0f;
        _camera = GetComponent<Camera>();
        _levelManager = FindObjectOfType<LevelManager>();
        movingLeft = false; movingUp = false;
    }


    public void AdaptCameraToTerrain(Bounds terrainBounds)
    {
        var (center, size) = AdjustOrthoCamera(terrainBounds);
   
        _camera.transform.position = center;
        _camera.orthographicSize = size;
    }

    public void AdaptCameraToTerrain(Bounds terrainBounds, float duration)
    {
        _originPosition = _camera.transform.position;
        _originSize = _camera.orthographicSize;

        var (center, size) = AdjustOrthoCamera(terrainBounds);
        _targetPosition = center;
        _targetSize = size;

        _adaptTime = duration;
        _lerpFactor = 0;
    }
    private (Vector3 center, float size) AdjustOrthoCamera(Bounds bounds)
    {
        Camera cam = GetComponent<Camera>();
        float vertical = bounds.size.y;
        float horizontal = bounds.size.x * cam.pixelHeight/cam.pixelWidth;
        float size = Mathf.Max(horizontal, vertical) * 0.5f;

        Vector3 center = bounds.center + new Vector3(0, 0, -10);
        return (center,size);
    }

    private void Direction()
    {
        Camera cam = GetComponent<Camera>();
        bool oldLeft, oldUp;

        oldLeft = movingLeft;
        oldUp = movingUp;

        if (movingDist.x < 0){
            movingLeft = true;
        }else{
            movingLeft = false;
        }

        if(movingDist.y < 0){
            movingUp = false;
        }else{
            movingUp = true;
        }

        if(oldLeft != movingLeft)
        {
            movingDist.x = 0;
        }
        if(oldUp != movingUp)
        {
            movingDist.y = 0;
        }

    }

    private void Update()
    {
        if (_lerpFactor < 1.0f)
        {
            _camera.transform.position = Vector3.Lerp(_originPosition, _targetPosition, _lerpFactor);
            _camera.orthographicSize = Mathf.Lerp(_originSize, _targetSize, _lerpFactor);

            _lerpFactor += Time.deltaTime / _adaptTime;
        }
    }
}
