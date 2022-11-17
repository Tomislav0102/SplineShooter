using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;

public class EventManager : MonoBehaviour
{
    public static System.Action GameReady;
    public static System.Action<string, int, bool> LevelDoneWin;
    public static System.Action PathGenerated;
    public static System.Action<EnemyBehaviour> EnemyDestroyed;

    private void OnEnable()
    {
        LevelDoneWin += EndWin;
        GameReady += GameStart;
        PathGenerated += CanGenerateMesh;
        EnemyDestroyed += EnemyRemoved;
    }
    private void OnDisable()
    {
        LevelDoneWin -= EndWin;
        GameReady -= GameStart;
        PathGenerated -= CanGenerateMesh;
        EnemyDestroyed -= EnemyRemoved;
    }
    protected virtual void GameStart()
    {
        
    }
    protected virtual void EndWin(string st, int level, bool victory)
    {
       
    }
    protected virtual void CanGenerateMesh()
    {

    }
    protected  virtual void EnemyRemoved(EnemyBehaviour enemyBehaviour)
    {

    }
}
