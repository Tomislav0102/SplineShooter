using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;
using UnityEngine.Splines;

public class LevelManager : EventManager
{
    GameManager gm;
    [SerializeField] SplineAnimate player, enemy;
    [SerializeField] SplineContainer container;
    [Header("LEVEL SPECIFIC")]
    [SerializeField] bool facingOutward;
    [SerializeField] int enemyCount;
    [HideInInspector] public int enemyWithCannonsCount;
    [SerializeField] ShootingMode[] shootingMode = new ShootingMode[2];
    [SerializeField, Tooltip("If 0 fuel is infinte")] int fuel = 1;
    [SerializeField, Tooltip("if 0 ammo is infinte")] int ammo = 10;
    [SerializeField, Tooltip("Prvi je player, ostali su sve enemy")] float[] startPositions;
    [SerializeField] Transform puParent;
    [SerializeField] Pickup puPrefab;
    Pickup _currentPU;
    Vector2[] pusSpawnPoints;


    private void Awake()
    {
        gm = GameManager.gm;
        gm.levelManager = this;
    }
    private void Start()
    {
        pusSpawnPoints = new Vector2[puParent.childCount];
        for (int i = 0; i < pusSpawnPoints.Length; i++)
        {
            pusSpawnPoints[i] = puParent.GetChild(i).position;
        }
        enemyWithCannonsCount = enemyCount;
        if (shootingMode[1] == ShootingMode.NoBullets) enemyWithCannonsCount--;
    }

    protected override void GameStart()
    {
        base.GameStart();
        container = GameObject.FindGameObjectWithTag("Path").GetComponent<SplineContainer>();
        gm.IniConnect(container, facingOutward, player, fuel, ammo, (enemyCount > 0 ? enemy : null), shootingMode, startPositions);
        for (int i = 0; i < 3; i++)
        {
            if (i == 1 && fuel <= 0) continue;
            if (i == 2 && ammo <= 0) continue;
            _currentPU = Instantiate(puPrefab, pusSpawnPoints[i], Quaternion.identity);
            _currentPU.pUtype = (PUtype)i;
            _currentPU.gameObject.SetActive(true);
        }
    }

    protected override void EnemyRemoved(EnemyBehaviour enemyBehaviour)
    {
        base.EnemyRemoved(enemyBehaviour);
        enemyCount--;
        enemyWithCannonsCount--;
    }






}

