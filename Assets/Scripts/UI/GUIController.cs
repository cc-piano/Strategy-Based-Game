using System.Collections;
using System.Collections.Generic;
using Builder;
using UnityEngine;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    public GameObject Prefab;
    public GameObject Menu;
    public GameObject CancelDraggingButton;
    public RectTransform Content;
    public static bool IsInMenu;
    public static bool IsMoving;
    public void CreateAndSetMenuItem(string ImageName, string Id, Area a)
    {
        string Name = ImageName + Id;
        GameObject tempItem = Instantiate(Prefab);
        tempItem.transform.SetParent(Content);

        tempItem.transform.GetChild(0).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/" + ImageName);
        tempItem.transform.GetChild(0).GetChild(0).GetComponent<Button>().onClick.AddListener(delegate
        {
            CancelDraggingButton.GetComponent<Button>().onClick.Invoke();
            DisableMenu();
            FindObjectOfType<MyDragDrop>().Spawn(Id, a);
            FindObjectOfType<CameraMovement>().enabled = false;
            CancelDraggingButton.SetActive(true);
            IsMoving = true;
            GameObject.Find("Grid").GetComponent<MyDragDrop>().enabled = true;
            //  StartCoroutine(Wait());
        });
        tempItem.transform.GetChild(1).GetComponent<Text>().text = Name;
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1.5f);
        GameObject.Find("Grid").GetComponent<MyDragDrop>().enabled = true;
    }

    public void EnableMenu()
    {
        IsInMenu = true;
        Menu.SetActive(true);
    }

    public void DisableMenu()
    {
        IsInMenu = false;
        Menu.SetActive(false);
    }
}
