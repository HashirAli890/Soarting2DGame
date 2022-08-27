/* GitHub project: https://github.com/danielcmcg/Unity-UI-Nested-Drag-and-Drop */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler,IPointerUpHandler
{
    Vector3 startPosition;
    Vector3 diffPosition;
    public bool Drag;
    public bool pointerUp;
    public bool triggerEnteredOnce;
    public bool triggerEnteredTwice;
    public bool Moving;

    public void OnDrag(PointerEventData eventData)
    {
        //GameHandler.Instance.Dargging = true;
        transform.position = Input.mousePosition - diffPosition;
        Drag = true;
        Moving = true;
        GameHandler.Instance.DraggedObject = this.gameObject;
        //Debug.Log(Input.mousePosition);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //GameHandler.Instance.Dargging = false;
        Drag = false;
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        GameHandler.Instance.appBeingused = true;
        pointerUp = false;
        startPosition = transform.position;
        diffPosition = Input.mousePosition - startPosition;
        EventSystem.current.SetSelectedGameObject(gameObject);

        GameHandler.Instance.finalAppPosition = transform.localPosition;
       
    }
    public void OnPointerUp(PointerEventData eventData)
    {

        pointerUp = true;
        
        if (GetComponent<TriggerCheck>().InsideFolder && !RectTransformUtility.RectangleContainsScreenPoint(GameHandler.Instance.OpenFolderRef.GetComponent<Image>().rectTransform, Input.mousePosition))
        {
            GameHandler.Instance.InsideFolderApps.Remove(this.gameObject);
            GameHandler.Instance.Apps.Add(this.gameObject);
            transform.SetParent(GameHandler.Instance.AppsMainParent.transform);
            if (GameHandler.Instance.OpenFolderRef.transform.childCount  == 1)
            {
                GameHandler.Instance.InsideFolderApps.Remove(GameHandler.Instance.OpenFolderRef.transform.GetChild(0).gameObject);
                GameHandler.Instance.Apps.Add(GameHandler.Instance.OpenFolderRef.transform.GetChild(0).gameObject);
                GameHandler.Instance.OpenFolderRef.transform.GetChild(0).transform.SetParent(GameHandler.Instance.AppsMainParent.transform);
                GameHandler.Instance.currentFolderGridClosed.transform.parent.gameObject.SetActive(false);
                GameHandler.Instance.currentFolderGridClosed.transform.parent.SetParent(GameHandler.Instance.transform);
             
                DestroyImmediate(GameHandler.Instance.currentFolderGridClosed.transform.parent.gameObject);
            }
            
            int folderChildCount = GameHandler.Instance.OpenFolderRef.transform.childCount;

            for (int i = 0; i < folderChildCount; i++)
            {
                Destroy(GameHandler.Instance.OpenFolderRef.transform.GetChild(0).GetComponent<UIDrag>());
                GameHandler.Instance.OpenFolderRef.transform.GetChild(0).SetParent(GameHandler.Instance.currentFolderGridClosed.transform);
                GameHandler.Instance.DisableTriggers(GameHandler.Instance.currentFolderGridClosed.transform.GetChild(i).gameObject);
            }

            GameHandler.Instance.InsideFolderApps.Clear();
            GameHandler.Instance.FolderScreen.SetActive(false);
            GameHandler.Instance.currentFolderGridClosed = null;
            GameHandler.Instance.UpdateListIndex(false);
            
            foreach (GameObject app in GameHandler.Instance.Apps)
            {
                GameHandler.Instance.ActivateTriggers(app);
            }
            
        }
        
        GameHandler.Instance.appBeingused = false;
       
        StartCoroutine(ResetGrids());
        GameHandler.Instance.CheckScore(GetComponent<TriggerCheck>().InsideFolder);
        if (GetComponent<UIDrag>())
        {
            if (GameHandler.Instance.SwapableObject && !GameHandler.Instance.SwapableObject.GetComponent<TriggerCheck>().Middle)
            {
                if (GameHandler.Instance.SwapableObject.GetComponent<UIDrag>().triggerEnteredOnce && GameHandler.Instance.SwapableObject.GetComponent<UIDrag>().triggerEnteredTwice)
                {
                    Debug.Log("Checking here");
                    this.gameObject.transform.GetComponentInChildren<SideTrigger>().SwitchApps(GameHandler.Instance.SwapableObject);
                }
            }
        }
    }

    IEnumerator ResetGrids()
    {
        yield return new WaitForSeconds(0.05f);
        GameHandler.Instance.OpenFolderRef.GetComponent<GridLayoutGroup>().enabled = false;
        GameHandler.Instance.AppsMainParent.GetComponent<GridLayoutGroup>().enabled = false;
        yield return new WaitForSeconds(0.05f);
        GameHandler.Instance.OpenFolderRef.GetComponent<GridLayoutGroup>().enabled = true;
        GameHandler.Instance.AppsMainParent.GetComponent<GridLayoutGroup>().enabled = true;
        Moving = false;
    }
    

}
