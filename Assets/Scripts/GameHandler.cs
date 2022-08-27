using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;
    [Header ("Apps List")]
    public List<GameObject> Apps;
    [Header("Phone Object")]
    public GameObject AppsMainParent;
    [HideInInspector]
    public Vector3 finalAppPosition;
    [Header("Score")]
    public float addScore = 1f;
    public float subScore = 0.5f;
    public Text scoreText;

    [Header("Prefab")]
    public GameObject folderPrefab;


    [Header("Grid Layout Multiplier")]
    public float GridMultiplier;

    public GameObject OpenFolderRef;
    public GameObject FolderScreen;

    [Header("Inside Folder List")]
    public List<GameObject> InsideFolderApps;
    public GameObject currentFolderGridClosed;

    public GameObject appPrefab;

    public bool appBeingused;
    private GameObject app;

    [Header("Screens")]
    public GameObject LevelCompleteScreen;
    public GameObject ProfileScreen;
    public GameObject MainScreen;
    [Header("Text")]
    public Text ObjectiveText;

    //public List<GameObject> Folders;

    public Image ProgressBar;

    int DrageObjectIndex;
    int ColliedObjectindex;
    
    public GameObject SwapableObject;
    public GameObject DraggedObject;
    private void Awake()
    {
        Instance = this;
    }
  
    

    private void Start()
    {
        if (GameManager.Instance.Next)
        {
            MainScreen.SetActive(false);
            ProfileScreen.SetActive(true);
        }
        if (GameManager.Instance.Restart)
        {
            MainScreen.SetActive(false);
            ProfileScreen.SetActive(false);
            Initiate();
        }
        GameManager.Instance.Next = false;
        GameManager.Instance.Restart = false;
    }
    public void Initiate()
    {
        ObjectiveText.text = LevelHandler.Instance._levels[GameManager.Instance.LevelNo].ObjectiveText + " " +LevelHandler.Instance._levels[GameManager.Instance.LevelNo].ScoreToComplete;
        for (int i = 0; i < LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps.Length; i++)
        {
            app = Instantiate(appPrefab);
            app.transform.SetParent(AppsMainParent.transform);
            app.GetComponent<AppAttriutes>().AppName = LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps[i].AppName;
            app.GetComponent<AppAttriutes>().ColorName = LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps[i].AppColor;
            app.GetComponent<AppAttriutes>().AppCatagory = LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps[i].Catagory;
            //            app.GetComponent<Image>().sprite = LevelHandler.Instance._levels[GameManager.Instance.LevelNo].Apps[i].AppImage;
            app.GetComponent<AppAttriutes>().ShowText();

            app.transform.localScale = Vector3.one;
            Apps.Add(app);
        }
        GameManager.Instance.score = 0;
    }

    public void DisableTriggers(GameObject clickedApp, GameObject collidedApp)
    {
        foreach (GameObject app in Apps)
        {
            if (app != clickedApp || app != collidedApp)
            {
                app.GetComponent<BoxCollider2D>().enabled = false;
                app.GetComponentInChildren<BoxCollider2D>().enabled = false;
            }
        }
    }
    public void EnableTriggers()
    {
        foreach (GameObject app in Apps)
        {
            app.GetComponent<BoxCollider2D>().enabled = true;
            app.GetComponentInChildren<BoxCollider2D>().enabled = true;
        }
    }

    public void UpdateChildList(GameObject DrageObject, GameObject ColliedObject) 
    {
        if (ColliedObject.GetComponent<AppAttriutes>())
        {
            Debug.Log(ColliedObject.GetComponent<AppAttriutes>().AppName);
        }
        if (ColliedObject.GetComponent<TriggerCheck>().InsideFolder)
        {
            ColliedObjectindex = InsideFolderApps.IndexOf(ColliedObject);
            DrageObjectIndex = InsideFolderApps.IndexOf(DrageObject);
        }
        else
        {
            ColliedObjectindex = Apps.IndexOf(ColliedObject);
            DrageObjectIndex = Apps.IndexOf(DrageObject);
        }
       
    
        ColliedObject.transform.SetSiblingIndex(DrageObjectIndex);
      //  DrageObject.transform.SetSiblingIndex(ColliedObjectindex);
       


        UpdateListIndex(ColliedObject.GetComponent<TriggerCheck>().InsideFolder);
    }
    public void UpdateListIndex(bool InsideFolder)
    {
        if (InsideFolder)
        {
            InsideFolderApps.Clear();
            for (int i = 0; i < OpenFolderRef.transform.childCount; i++)
            {

                InsideFolderApps.Add(OpenFolderRef.transform.GetChild(i).gameObject);
            }
        }
        else
        {
            Apps.Clear();
            for (int i = 0; i < AppsMainParent.transform.childCount; i++)
            {
                Apps.Add(AppsMainParent.transform.GetChild(i).gameObject);
            }
        }
       // CheckScore(InsideFolder);
    }

    public void CreateFolder(GameObject collidedApp,GameObject draggedApp, TriggerCheck tCheck)
    {
        GameObject folder = Instantiate(folderPrefab, AppsMainParent.transform);
        folder.transform.SetSiblingIndex(Apps.IndexOf(collidedApp));
        collidedApp.SetActive(false);
        draggedApp.SetActive(false);
        Apps[Apps.IndexOf(collidedApp)] = folder;
        Apps.Remove(draggedApp);

        DisableTriggers(collidedApp);
        DisableTriggers(draggedApp);
        collidedApp.transform.SetParent(folder.GetComponentInChildren<GridLayoutGroup>().transform);
        Destroy(collidedApp.GetComponent<UIDrag>());
        draggedApp.transform.SetParent(folder.GetComponentInChildren<GridLayoutGroup>().transform);
        Destroy(draggedApp.GetComponent<UIDrag>());

        collidedApp.SetActive(true);
        draggedApp.SetActive(true);
        AssignGridSize(folder.GetComponentInChildren<GridLayoutGroup>(), true);
        tCheck.once = false;
      
    }

    public void AddInFolder(GameObject app,GameObject folder)
    {
        Apps.Remove(app);
        app.transform.SetParent(folder.GetComponent<TriggerCheck>().LayoutGroup.transform);
        Destroy(app.GetComponent<UIDrag>());
        DisableTriggers(app);
        UpdateListIndex(true);
    }

    #region ScoreLogic
    
    private void AddScore()
    {
        GameManager.Instance.score += addScore;
    }
    private void DeductScore()
    {
//        GameManager.Instance.score -= subScore;
    }

    public void CheckScore(bool insideFolder)
    {
        GameManager.Instance.score = 0;
        List<GameObject> appList=new List<GameObject>();
        if (insideFolder)
        {
            appList = InsideFolderApps;
        }
        else
        {
            appList = Apps;
        }
        
        for (int i = 0; i < appList.Count; i++)
        {
            if (!appList[i].GetComponent<TriggerCheck>().Folder)
            {
                foreach (ScoreState scoreState in LevelHandler.Instance._levels[GameManager.Instance.LevelNo]
                    .scoreSequence)
                {
                    switch (scoreState)
                    {
                        case ScoreState.Name:
                            ScoreByName(i, appList);
                            break;
                        case ScoreState.Color:
                            ScoreByColor(i, appList);
                            break;
                        case ScoreState.Category:
                            ScoreByCategory(i, appList);
                            break;
                        case ScoreState.Creator:
                            ScoreByCreator(i, appList);
                            break;
                    }
                }
            }
        }

        scoreText.text = GameManager.Instance.score.ToString();
        ProgressBar.fillAmount = (GameManager.Instance.score / LevelHandler.Instance._levels[GameManager.Instance.LevelNo].ScoreToComplete);
        StartCoroutine(Wait());
    }
    IEnumerator Wait() 
    {
        yield return new WaitForSeconds(2.5f);
        if (GameManager.Instance.score >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].ScoreToComplete)
        {
            LevelCompleteScreen.SetActive(true);
        }
    }

    public void ScoreByName(int index,List<GameObject> appList)
    {
        string currentColor = appList[index].GetComponent<AppAttriutes>().AppName;
        char[] currentTemp = currentColor.ToCharArray();
      
        
        
        //Checking Previous 
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != 0)
        {
            if (appList[index - 1].GetComponent<AppAttriutes>())
            {
                string previousName = appList[index - 1].GetComponent<AppAttriutes>().AppName;
                char[] nextTemp = previousName.ToCharArray();
              

                for (int i = 0; i < currentTemp.Length; i++)
                {
                    //Debug.Log("-----CurrentFirst :" + currentTemp[i] + "-----NextFirst" + nextTemp[i]);
                    if (currentTemp[i] > nextTemp[i])
                    {
                        //Okay boss
                        AddScore();
                      
                        break;
                    }
                    else if (currentTemp[i] < nextTemp[i])
                    {
                        //Score boss 
                        DeductScore();
                       
                        break;
                    }
                }
            }
        }
        
        // Checking Next
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col-1)
        {
            if ((index + 1 < appList.Count) && appList[index + 1].GetComponent<AppAttriutes>())
            {
                string nextName = appList[index + 1].GetComponent<AppAttriutes>().AppName;
                char[] nextTemp = nextName.ToCharArray();
               

                for (int i = 0; i < currentTemp.Length; i++)
                {
                    if (currentTemp[i] > nextTemp[i])
                    {
                        //score deducted
                        DeductScore();
                       
                        break;
                    }
                    else if (currentTemp[i] < nextTemp[i])
                    {
                        //score added
                        AddScore();
                      
                        break;
                    }
                }
            }
        }

        //Checking Top
        if (index >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)
        {
            if (appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>())
            {
                string topName = appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().AppName;
                char[] nextTemp = topName.ToCharArray();
               

                for (int i = 0; i < currentTemp.Length; i++)
                {
                    if (currentTemp[i] > nextTemp[i])
                    {
                        //score added
                        AddScore();
                      
                        break;
                    }
                    else if (currentTemp[i] < nextTemp[i])
                    {
                        //add deducted
                        DeductScore();
                      
                        break;
                    }
                }
            }
        }
        
        //Checking Bottom
        if (index < (appList.Count-LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col))
        {
            if (appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                .GetComponent<AppAttriutes>())
            {
                string bottomName = appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().AppName;
                char[] nextTemp = bottomName.ToCharArray();
          

                for (int i = 0; i < currentTemp.Length; i++)
                {
                    if (currentTemp[i] > nextTemp[i])
                    {
                        //score deducted
                        DeductScore();
                     
                        break;
                    }
                    else if (currentTemp[i] < nextTemp[i])
                    {
                        //score added
                        AddScore();
                      
                        break;
                    }
                }
            }
        }
    }

    public void ScoreByColor(int index,List<GameObject> appList)
    {
        string currentColor = appList[index].GetComponent<AppAttriutes>().ColorName;
        
        //Checking Previous 
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != 0)
        {
            if (appList[index - 1].GetComponent<AppAttriutes>())
            {
                string nextColor = appList[index - 1].GetComponent<AppAttriutes>().ColorName;
             

                if (currentColor == nextColor)
                {
                    //Okay boss
                    AddScore();
               
                }
                else
                {
                    //Score boss
                    DeductScore();
                    
                }
            }
        }
        
        // Checking Next
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col-1)
        {
            if (appList[index + 1].GetComponent<AppAttriutes>())
            {
                string nextColor = appList[index + 1].GetComponent<AppAttriutes>().ColorName;
                char[] nextTemp = nextColor.ToCharArray();
            

                if (currentColor == nextColor)
                {
                    //Okay boss
                    AddScore();
                  
                }
                else
                {
                    //Score boss 
                    DeductScore();
                
                }
            }
        }

        //Checking Top
        if (index >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)
        {
            if (appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                .GetComponent<AppAttriutes>())
            {
                string nextColor = appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().ColorName;
              

                if (currentColor == nextColor)
                {
                    //Okay boss
                    AddScore();
                 
                }
                else
                {
                    //Score boss 
                    DeductScore();
                   
                }
            }
        }
        
        //Checking Bottom
        if (index < (appList.Count - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col))
        {
            if (appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                .GetComponent<AppAttriutes>())
            {
                string nextColor = appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().ColorName;
              

                if (currentColor == nextColor)
                {
                    //Okay boss
                    AddScore();
                    
                }
                else
                {
                    //Score boss 
                    DeductScore();
               
                }
            }
        }
    }
    
    public void ScoreByCreator(int index,List<GameObject> appList)
    {
        string currentCreator = appList[index].GetComponent<AppAttriutes>().Creator;
       
        
        //Checking Previous 
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != 0)
        {
            if (appList[index - 1].GetComponent<AppAttriutes>())
            {
                string nextCreator = appList[index - 1].GetComponent<AppAttriutes>().Creator;
               

                if (currentCreator == nextCreator)
                {
                    //Okay boss
                    AddScore();
                   
                }
                else
                {
                    //Score boss 
                    DeductScore();
                   
                }
            }
        }
        
        // Checking Next
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col-1)
        {
            if (appList[index + 1].GetComponent<AppAttriutes>())
            {
                string nextCreator = appList[index + 1].GetComponent<AppAttriutes>().Creator;
             

                if (currentCreator == nextCreator)
                {
                    //Okay boss
                    AddScore();
                  
                }
                else
                {
                    //Score boss 
                    DeductScore();
                  
                }
            }
        }

        //Checking Top
        if (index >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)
        {
            if (appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>())
            {
                string nextCreator = appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().Creator;
            

                if (currentCreator == nextCreator)
                {
                    //Okay boss
                    AddScore();
                 
                }
                else
                {
                    //Score boss 
                    DeductScore();
                    
                }
            }
        }
        
        //Checking Bottom
        if (index < (appList.Count-LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col))
        {
            if (appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                .GetComponent<AppAttriutes>())
            {
                string nextCreator = appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().Creator;
              

                if (currentCreator == nextCreator)
                {
                    //Okay boss
                    AddScore();
                  
                }
                else
                {
                    //Score boss 
                    DeductScore();
                    
                }
            }
        }
    }
    
    public void ScoreByCategory(int index,List<GameObject> appList)
    {
        string currentCategory = appList[index].GetComponent<AppAttriutes>().AppCatagory;
      
        
        //Checking Previous 
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != 0)
        {
            if (appList[index - 1].GetComponent<AppAttriutes>())
            {
                string nextCategory = appList[index - 1].GetComponent<AppAttriutes>().AppCatagory;
              

                if (currentCategory == nextCategory)
                {
                    //Okay boss
                    AddScore();
                   
                }
                else
                {
                    //Score boss 
                    DeductScore();
                    
                }
            }
        }
        
        // Checking Next
        if ((index % (LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)) != LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col-1)
        {
            if (appList[index + 1].GetComponent<AppAttriutes>())
            {
                string nextCategory = appList[index + 1].GetComponent<AppAttriutes>().AppCatagory;
                char[] nextTemp = nextCategory.ToCharArray();
             

                if (currentCategory == nextCategory)
                {
                    //Okay boss
                    AddScore();
                  
                }
                else
                {
                    //Score boss 
                    DeductScore();
                 
                }
            }
        }

        //Checking Top
        if (index >= LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col)
        {
            if (appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col].GetComponent<AppAttriutes>())
            {
                string nextCategory = appList[index - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().AppCatagory;
              

                if (currentCategory == nextCategory)
                {
                    //Okay boss
                    AddScore();
                  
                }
                else
                {
                    //Score boss 
                    DeductScore();
                  
                }
            }
        }
        
        //Checking Bottom
        if (index < (appList.Count - LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col))
        {
            if (appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                .GetComponent<AppAttriutes>())
            {
                string nextCategory = appList[index + LevelHandler.Instance._levels[GameManager.Instance.LevelNo].col]
                    .GetComponent<AppAttriutes>().AppCatagory;
               

                if (currentCategory == nextCategory)
                {
                    //Okay boss
                    AddScore();
                  
                }
                else
                {
                    //Score boss 
                    DeductScore();
                   
                }
            }
        }
    }
    
    #endregion
    public void AssignGridSize(GridLayoutGroup Grid,bool folder) 
    {
        if (folder)
        {
            Grid.cellSize = new Vector2(Grid.cellSize.x / GridMultiplier, Grid.cellSize.y / GridMultiplier);
            Grid.padding.left = ( Grid.padding.left / (int)GridMultiplier);
            Grid.padding.top = (Grid.padding.top / (int)GridMultiplier);
            Grid.spacing = new Vector2(Grid.spacing.x / GridMultiplier, Grid.spacing.y / GridMultiplier);
        }
        else 
        {
            Grid.cellSize = new Vector2(Grid.cellSize.x * GridMultiplier, Grid.cellSize.y * GridMultiplier);
            Grid.padding.left = (Grid.padding.left * (int)GridMultiplier);
            Grid.padding.top = (Grid.padding.top * (int)GridMultiplier);
            Grid.spacing = new Vector2(Grid.spacing.x * GridMultiplier, Grid.spacing.y * GridMultiplier);
        }
    }
    public void ActivateTriggers(GameObject Obj) 
    {
        Obj.GetComponent<Collider2D>().enabled = true;
        Collider2D[] Col = Obj.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D c in Col) 
        {
            c.enabled = true;
        }
    }

    public void DisableTriggers(GameObject Obj)
    {
        Obj.GetComponent<Collider2D>().enabled = false;
        BoxCollider2D[] Col = Obj.GetComponentsInChildren<BoxCollider2D>();
        foreach (BoxCollider2D c in Col)
        {
            c.enabled = false;
        }
    }
    public void OnPlay()
    {
        MainScreen.SetActive(false);
        ProfileScreen.SetActive(true);
    }
    public void OnSelectProfile(int LeveLNo) 
    {
        GameManager.Instance.LevelNo = LeveLNo;
        ProfileScreen.SetActive(false);
        Initiate();
    }
    public void OnRestart()
    {
        GameManager.Instance.Restart = true;
        SceneManager.LoadScene("Gameplay");
    }
    public void OnNext()
    {
        GameManager.Instance.Next = true;
        SceneManager.LoadScene("Gameplay");
    }
}
