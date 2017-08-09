using System.Linq;
using Builder;
using UnityEngine;

public class MyDragDrop : MonoBehaviour
{
    public string SpawnEffectPath = "Effects/";
    private GameObject Prefab;


    private int XScale = 1;
    private int ZScale = 1;
    private Vector2[] vv;

    private int sizeX;
    private int sizeZ;
    private bool CanBeDropped;
    private bool _inCour;
    // Use this for initialization
    void Start ()
	{
	    Input.simulateMouseWithTouches = true;
	    GameObject gg = FindObjectOfType<GridConfig>().gameObject;
	    for (int i = 0; i < gg.transform.childCount; i++)
	    {
	        gg.transform.GetChild(i).GetComponent<GridElement>().Index = i;
	    }

	    vv = new Vector2[gameObject.transform.childCount];
	    for (int i = 0; i < gameObject.transform.childCount; i++)
	    {
	        vv[i] = new Vector2(gameObject.transform.GetChild(i).transform.localPosition.x, gameObject.transform.GetChild(i).transform.localPosition.z);
	    }
    }
    public void Spawn(string id, Area a)
    {
        XScale = a.w;
        ZScale = a.h;
        Prefab = FindObjectOfType<DecorationDirector>().MakeNewItemById(id);
        if (!Prefab)
        {
            Prefab = null;
            CanBeDropped = false;
            GUIController.IsMoving = false;
            StopAllCoroutines();
            GetComponent<MyDragDrop>().enabled = false;
            Debug.LogError("Limit of this item is finished!");
            return;
        }
        Material[] matArr = Prefab.GetComponent<Renderer>().materials;
        for (int i = 0; i < matArr.Length; i++)
        {
            ChangeMaterialMode.ChangeRenderMode(matArr[i], ChangeMaterialMode.BlendMode.Fade);
            matArr[i].SetColor("_Color", new Color(1, 1, 1, 0.5f));
        }
        Prefab.transform.localScale = new Vector3(XScale, XScale, ZScale);

    }

    //IEnumerator Wait()
    //{
    //    _inCour = true;
    //    GUIController GUIController = FindObjectOfType<GUIController>();
    //    yield return new WaitUntil(() => !GUIController.Menu.gameObject.activeSelf);//wait for menu to disable
    //    yield return new WaitForSeconds(1.0f);
    //    yield return new WaitForSeconds(1.5f);//wait for menu to disable
    //    CanBeDropped = true;
    //    _inCour = false;
    //}

  
    // Update is called once per frame
	void Update () {
        Debug.Log(Prefab);
	    if (!Prefab)
	    {
            return;
	    }
     //   if (!_inCour){
	    //    StartCoroutine(Wait());
	    //}
	    //Debug.Log(CanBeDropped);
	    //if (!CanBeDropped)
	    //{
     //       return;
	    //}
        Debug.Log(Time.deltaTime);
	    Vector3 point = Vector3.zero;
	    if (getTargetLocation(out point))
	    {
	        int k = CompareSet(vv, new Vector2(point.x, point.z));
            if (transform.GetChild(k).GetComponent<GridElement>().IsBusy)
	        {
	            return;
	        }

            Vector3 prevpos = Prefab.transform.position;
            Prefab.transform.position = new Vector3(transform.GetChild(k).transform.position.x,
                transform.GetChild(k).transform.position.y, transform.GetChild(k).transform.position.z);

            bool ReturnToPreviousPos = transform.Cast<Transform>()
                .Any(a => a.GetComponent<Collider>().bounds.Intersects(Prefab.GetComponent<Collider>().bounds)
                            && a.GetComponent<GridElement>().IsBusy);
            if (ReturnToPreviousPos)
            {
                Prefab.transform.position = prevpos;
            }
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        if ((Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled) &&
	        Prefab != null)
	    {
        SpawnParticle(point);
             Clicked();
        }
#else
        if (Input.GetMouseButtonDown(0))
        {
            SpawnParticle(point);
            Clicked();
        }
#endif
	}

    void Clicked()
    {
        Material[] matArr = Prefab.GetComponent<Renderer>().materials;
        for (int i = 0; i < matArr.Length; i++)
        {
            ChangeMaterialMode.ChangeRenderMode(matArr[i], ChangeMaterialMode.BlendMode.Opaque);
            matArr[i].SetColor("_Color", new Color(1, 1, 1, 1));
        }
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<Collider>().bounds.Intersects(Prefab.GetComponent<Collider>().bounds))
            {
                transform.GetChild(i).GetComponent<GridElement>().IsBusy = true;
                if(GridConfig._bIsGridEnabled)
                transform.GetChild(i).GetComponent<GridElement>().EnableElement();
            }
        }
        Prefab = null;
        CanBeDropped = false;
        FindObjectOfType<GUIController>().CancelDraggingButton.SetActive(false);
        GUIController.IsMoving = false;
        FindObjectOfType<CameraMovement>().enabled = true;
        StopAllCoroutines();
        GetComponent<MyDragDrop>().enabled = false;
        Vibrate(500);
    }

    public void CancelDragging()
    {
        Destroy(Prefab);
        Prefab = null;
        CanBeDropped = false;
        GUIController.IsMoving = false;
    }

    void SpawnParticle(Vector3 point)
    {
        GameObject temp = Instantiate(Resources.Load(SpawnEffectPath))as GameObject;
        temp.transform.position = point;
        Destroy(temp, 1.0f);
    }


    int CompareSet(Vector2[] myArray, Vector2 pos)
    {
        int closestIndex = 0;
        float smallestDistance = 0.0f;
        for (int u = 0; u < myArray.Length; u++)
        {

            if (u != 0)
            {

                float thisDistance = (pos - myArray[u]).sqrMagnitude;

                closestIndex = (thisDistance < smallestDistance) ? u : closestIndex;
                smallestDistance = (thisDistance < smallestDistance) ? thisDistance : smallestDistance;

            }
            else smallestDistance = (pos - myArray[u]).sqrMagnitude;

        }
        return closestIndex;
    }

    bool getTargetLocation(out Vector3 point)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
         Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
#else
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
#endif

        RaycastHit hitInfo = new RaycastHit();
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("Grid")))
        {
            if (hitInfo.collider == GetComponent<Collider>())
            {
                point = hitInfo.point;
                return true;
            }
        }
        point = Vector3.zero;
        return false;
    }



    private void Vibrate(long milliseconds = 0) // the source of this plugin will be in plugins/android folder
    {
//#if UNITY_ANDROID && !UNITY_EDITOR
//        return;
//#endif
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaClass javavClass = new AndroidJavaClass("com.cc.vibration.Vibration");

        if (milliseconds == 0)
        {
            javavClass.CallStatic("Vibrate", currentActivity);
        }
        else
        {
            javavClass.CallStatic("VibrateForSeconds", currentActivity, milliseconds);
        }
    }
}
