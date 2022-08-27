using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public enum ScoreState
{
    None,
    Name,
    Color,
    Category,
    Creator
}

[System.Serializable]
public class AppsAttributes 
{
    public Sprite AppImage;
    public string AppName;
    public string CompanyName;
    public string AppColor;
    public string Catagory;
   
}


[System.Serializable]
public class Levels 
{
    public int LevelNo;
    public string Description;
    public AppsAttributes[] Apps;
    //public bool MakeFolder;
    public int col;
    public ScoreState[] scoreSequence;
    public int ScoreToComplete;
    public string ObjectiveText;
}
public class LevelHandler : MonoBehaviour
{
   
    public Levels[] _levels;
    public List<string> filePath;
    public static LevelHandler Instance;

    public void Awake()
    {
        Instance = this;
    }

    [ContextMenu("FillLevelData")]
    public void FillLevelData()
    {

        StreamReader source;
        for (int j = 0; j < filePath.Count; j++)
        {
           
      
            source = new StreamReader(Application.dataPath + "/" + filePath[j] + ".txt");
            string fileContents = source.ReadToEnd();
            source.Close();
            string[] lines = fileContents.Split("\n"[0]);
            int appsCount = 0;
            for (int i = 0; i < lines.Length; i++)
            {
                Debug.Log(lines[i]);
                _levels[j].Apps[appsCount].AppName = lines[i];
                i++;
                _levels[j].Apps[appsCount].AppColor = lines[i];
                i++;
                _levels[j].Apps[appsCount].Catagory = lines[i];
                i++;
                _levels[j].Apps[appsCount].CompanyName = lines[i];
                appsCount++;
            }
        }
    }
}
