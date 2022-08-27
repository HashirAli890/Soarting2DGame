using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager 
{
    private static GameManager _Instance;
    public int LevelNo;
    public float score;
    public bool Restart = false;
    public bool Next = false;
    public static GameManager Instance 
    {
        get
        {
            if (_Instance ==null) 
            {
                _Instance = new GameManager();
            }
            return _Instance;
        }
    }
    
}
