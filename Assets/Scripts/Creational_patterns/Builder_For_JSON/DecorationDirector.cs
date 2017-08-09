using System.Linq;
using Builder;
using UnityEngine;

public class DecorationDirector : MonoBehaviour
{
    private DecorationParser _dp;
    public GridConfig GridConfig;
    public GUIController GUIController;

    void Start() //Create default map items
    {
        if (!GridConfig)
        {
            GridConfig = FindObjectOfType<GridConfig>();
        }
        if (!GUIController)
        {
            GUIController = FindObjectOfType<GUIController>();
        }
        _dp = new DecorationParser();
        if (!_dp.LoadDataFromScriptableObj())
        {
            Debug.LogWarning("Troubles with loading ScriptableObject!");
            Debug.LogWarning("Trying to parse JSON manually");
            Debug.LogWarning("Next time parse JSON before launch!");
            _dp.ParseJson("game");
        }
        else
        {
            //Fill menu with items
            for (int i = 0; i < _dp.DecorationDescriptionList.Items.Count; i++)
            {
                GUIController.CreateAndSetMenuItem(_dp.DecorationDescriptionList.Items[i].name, 
                    _dp.DecorationDescriptionList.Items[i].id, _dp.DecorationDescriptionList.Items[i].area);
            }
           
        }
        RandomSpawnObjects();

        Singleton.getInstance().Observer.NotifyObservers();
       
    }

    public void RandomSpawnObjects()
    {
        int columns = GridConfig.Columns;
        int rows = GridConfig.Rows;

        if (rows < 3 || columns < 3)
        {
            Debug.LogWarning("Can not create objects at the edges of the map\nROWS and COLUMNS should be bigger or equal to 3!");
            return;
        }

        for (int i = 0; i < rows - 2; i++)
        {
            SpawnItem(i * columns + columns);
            SpawnItem(i * columns + columns + columns - 1);
        }

        for (int i = 0; i < rows * columns; i++)
        {
            if (i < columns || i >= columns * rows - columns)
            {
                SpawnItem(i);
            }
        }
    }

    public GameObject MakeNewItemById(string id)
    {
        return _dp.MakeOneItem(_dp.DecorationDescriptionList.Items.FirstOrDefault(a => a.id.Equals(id)));
    }

    private void SpawnItem(int index)
    {
        int randomedItem = Random.Range(0, _dp.DecorationDescriptionList.Items.Count);

        GameObject temp = MakeOneItemWithNoLimitations(_dp.DecorationDescriptionList.Items[randomedItem]);

        temp.transform.parent = GridConfig.transform.GetChild(index);
        temp.transform.localPosition = Vector3.zero;
        temp.transform.LookAt(GridConfig.transform.GetChild(GridConfig.transform.childCount / 2));// Make everything face the center of grid
    }

    public GameObject MakeOneItemWithNoLimitations(DecorationJson j)
    {
        return GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/" + j.type + "/" + j.name));
    }
}
