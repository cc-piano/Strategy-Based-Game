using UnityEngine;

public class PlacementInfo : MonoBehaviour
{
    public string InfoToDisplay;

    void OnMouseDown()
    {
        Debug.Log(InfoToDisplay);
    }
}
