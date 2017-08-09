using System.Collections;
using System.Collections.Generic;
using Factory_Method;
using UnityEngine;

public class GridConfig : MonoBehaviour
{
    public int Rows = 0;
    public int Columns = 0;
    public float TileSize = 1;
    public GameObject PlanePrefab;

    public static bool _bIsGridEnabled;
    private List<GridBasement> _gridBasements = new List<GridBasement>();

    public void CreateTile(int i, int j)
    {
        _gridBasements.Add(new SquareCreator().FactoryMethod(i, j, TileSize, "Prefabs/Plane", transform));
    }

    public void UpdateCollider(Vector3 Center, Vector3 Size)
    {
        var Collider = GetComponent<BoxCollider>();
        Collider.center = Center;
        Collider.size = Size;
    }

    public void SetBusyElementAtIndex(int rows, int columns, GameObject GO)
    {
        GameObject child = transform.GetChild(Columns * rows + columns).gameObject;
        child.GetComponent<GridElement>().IsBusy = true;
        if (_bIsGridEnabled)
        {
            child.GetComponent<GridElement>().EnableElement();
        }
        GO.transform.parent = child.transform;
    }

    public void EnableDisableGrid()
    {
        StopAllCoroutines();
        StartCoroutine(EnableDisableGridCour());
    }

    public IEnumerator EnableDisableGridCour()
    {
        _bIsGridEnabled = !_bIsGridEnabled;
        foreach (Transform tr in transform)
        {
            if (_bIsGridEnabled)
            {
                tr.GetComponent<GridElement>().EnableElement();
            }
            else
            {
                tr.GetComponent<GridElement>().DisableElement();
            }
        }
        yield break;
    }
}
