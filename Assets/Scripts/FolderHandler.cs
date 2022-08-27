using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.UI;

public class FolderHandler : MonoBehaviour
{
    private Image folderPanel;
    private GridLayoutGroup _gridLayoutGroup;
    // Start is called before the first frame update
    void Start()
    {
        folderPanel = GetComponent<Image>();
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(folderPanel.rectTransform, Input.mousePosition))
            {
                Debug.Log("Clicked Outside");
                CloseFolder();
            }
        }
    }

    void CloseFolder()
    {
        if (!GameHandler.Instance.appBeingused)
        {
            Debug.Log("CloseFolder");
            GameHandler.Instance.FolderScreen.SetActive(false);
            int folderChildCount = _gridLayoutGroup.transform.childCount;

            for (int i = 0; i < folderChildCount; i++)
            {
                Destroy(_gridLayoutGroup.transform.GetChild(0).GetComponent<UIDrag>());
                _gridLayoutGroup.transform.GetChild(0)
                    .SetParent(GameHandler.Instance.currentFolderGridClosed.transform);
                GameHandler.Instance.DisableTriggers(GameHandler.Instance.currentFolderGridClosed.transform.GetChild(i)
                    .gameObject);
            }

            foreach (GameObject app in GameHandler.Instance.Apps)
            {
                GameHandler.Instance.ActivateTriggers(app);
            }

            GameHandler.Instance.InsideFolderApps.Clear();
            GameHandler.Instance.currentFolderGridClosed = null;
        }
    }
}
