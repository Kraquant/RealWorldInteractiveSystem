using UnityEngine;


public class DisplaceTextButton : MonoBehaviour
{
    public int offsetX = 0, offsetY = 20;
    [SerializeField] RectTransform textRect;
    Vector3 pos;
    
    void Start()
    {
        pos = textRect.localPosition;
    }

    public void Down()
    {
        textRect.localPosition = new Vector3(pos.x + (float)offsetX, pos.y + (float)offsetY, pos.z);
    }

    public void Up()
    {
        textRect.localPosition = new Vector3(pos.x, pos.y, pos.z);
    }
}