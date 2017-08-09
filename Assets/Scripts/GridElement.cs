using UIObserver;
using UnityEngine;

public class GridElement : MonoBehaviour, IObserver
{
    public Material BusyMaterial;
    public Material FreeMaterial;

    public Material DefaultMaterial;

    public bool IsBusy;
    public int Index;

    //implementing observer method
    public void AddToListOfObservers()
    {
        Singleton.getInstance().Observer.observers.Add(this);
    }

    public void ParsingFinished()
    {
        HandleFinishCreatingDefaults();
    }

    void HandleFinishCreatingDefaults()
    {
        if (transform.childCount > 0)
        {
            IsBusy = true;
        }
    }

    void OnEnable()
    {
        AddToListOfObservers();
    }

    public void EnableElement()
    {
        if (IsBusy)
        {
            GetComponent<MeshRenderer>().material = BusyMaterial;
        }
        else
        {
            GetComponent<MeshRenderer>().material = FreeMaterial;
        }
    }

    public void DisableElement()
    {
        GetComponent<MeshRenderer>().material = DefaultMaterial;
    }
}
