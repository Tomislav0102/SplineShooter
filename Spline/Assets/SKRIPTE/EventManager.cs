using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;

public class EventManager : MonoBehaviour
{
    public static System.Action GameReady;
    public static System.Action<string, int, bool> LevelDoneWin;
    public static System.Action<EnemyBehaviour> EnemyDestroyed;

    protected virtual void OnEnable()
    {
        LevelDoneWin += CallEv_LevelDoneWin;
        GameReady += CallEv_GameReady;
        EnemyDestroyed += CallEv_EnemyDestroyed;
    }
    protected virtual void OnDisable()
    {
        LevelDoneWin -= CallEv_LevelDoneWin;
        GameReady -= CallEv_GameReady;
        EnemyDestroyed -= CallEv_EnemyDestroyed;
    }
    protected virtual void CallEv_GameReady()
    {
       
    }
    protected virtual void CallEv_LevelDoneWin(string st, int level, bool victory)
    {
       
    }
    protected  virtual void CallEv_EnemyDestroyed(EnemyBehaviour enemyBehaviour)
    {

    }
}
