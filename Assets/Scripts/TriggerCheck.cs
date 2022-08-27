using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TriggerCheck : MonoBehaviour
{
    public bool InsideFolder;
    public bool Folder;
    public GridLayoutGroup LayoutGroup;
    public bool once;
    public bool Middle;
    private void OnTriggerStay2D(Collider2D other)
    {
        Middle = true;
        if (other.GetComponent<UIDrag>() && GetComponent<UIDrag>())
        {

            if (GetComponent<UIDrag>().Drag == false && other.gameObject.GetComponent<UIDrag>().Drag == false)
            {
                Debug.Log("folder here");
                if (GetComponent<UIDrag>().pointerUp == true && !InsideFolder)
                {
                    if (!Folder)
                    {
                        //Create folder
                        Debug.Log("Folder Creation");
                        if (other.GetComponent<TriggerCheck>().Folder)
                        {
                            GameHandler.Instance.AddInFolder(gameObject, other.gameObject);
                        }
                        else
                        {
                            if (!other.GetComponent<UIDrag>().Moving)
                            {
                                Debug.Log(transform.parent.name + " Parent Name " + gameObject.name + " My name");
                                GameHandler.Instance.CreateFolder(this.gameObject, other.gameObject, this);
                                other.GetComponent<UIDrag>().Moving = false;
                            }
                        }
                    }
                }
            }
        }
        
    }

    public void OnPointerUp() 
    {
        if (Folder && GetComponent<UIDrag>().Drag == false) 
        {
            if (LayoutGroup) 
            {
                GameHandler.Instance.FolderScreen.SetActive(true);
                int folderChildCount = LayoutGroup.transform.childCount;
                for (int i=0;i<folderChildCount;i++)
                {
                    if (LayoutGroup.transform.GetChild(i).GetComponent<TriggerCheck>()) 
                    {
                        LayoutGroup.transform.GetChild(i).GetComponent<TriggerCheck>().Middle = false;
                    }
                    LayoutGroup.transform.GetChild(0).SetParent(GameHandler.Instance.OpenFolderRef.transform);
                }
                foreach (GameObject obj in GameHandler.Instance.Apps)
                {
                    GameHandler.Instance.DisableTriggers(obj);
                }
                GameHandler.Instance.InsideFolderApps.Clear();
                for (int i = 0; i < GameHandler.Instance.OpenFolderRef.transform.childCount; i++)
                {
                    GameHandler.Instance.ActivateTriggers(GameHandler.Instance.OpenFolderRef.transform.GetChild(i).gameObject);
                    GameHandler.Instance.OpenFolderRef.transform.GetChild(i).GetComponent<TriggerCheck>().InsideFolder = true;
                    GameHandler.Instance.InsideFolderApps.Add(GameHandler.Instance.OpenFolderRef.transform.GetChild(i).gameObject);
                    GameHandler.Instance.OpenFolderRef.transform.GetChild(i).gameObject.AddComponent<UIDrag>();
                }

                GameHandler.Instance.currentFolderGridClosed = LayoutGroup.gameObject;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        Middle = false;
    }

}
