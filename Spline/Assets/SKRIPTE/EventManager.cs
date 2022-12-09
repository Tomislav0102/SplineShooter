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
        LevelDoneWin += EndWin;
        GameReady += GameStart;
        EnemyDestroyed += EnemyRemoved;
    }
    protected virtual void OnDisable()
    {
        LevelDoneWin -= EndWin;
        GameReady -= GameStart;
        EnemyDestroyed -= EnemyRemoved;
    }
    protected virtual void GameStart()
    {
       
    }
    protected virtual void EndWin(string st, int level, bool victory)
    {
       
    }
    protected  virtual void EnemyRemoved(EnemyBehaviour enemyBehaviour)
    {

    }
}
