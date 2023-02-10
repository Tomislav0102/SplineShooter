using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using FirstCollection;
using System;

public class EnemyAdditions : EventManager, IActivation
{
    GameManager gm;
    SplineContainer _splineContainer;
    public float insertionNorTime;
    const float CONST_SPAWNTIME = 8f;
    float _timerSpawn = Mathf.Infinity;
    bool _isActive;

   // [SerializeField] ShootingMode shootingMode;
    [SerializeField] SpawnedActors enemyToSpawn;

    public bool IsActive { get => _isActive; set => _isActive = value; }

    private void Awake()
    {
        gm = GameManager.gm;
        gm.enemyAdditions = this;
        _splineContainer = GetComponent<SplineContainer>();
    }

    private void Update()
    {
        if (!IsActive) return;

        _timerSpawn += Time.deltaTime;
        if (_timerSpawn > CONST_SPAWNTIME)
        {
            _timerSpawn = 0f;
            if (gm.poolManager.GetActor() == null) return;
            SpawnedActors bufferClass = new SpawnedActors
            {
                sm = enemyToSpawn.sm,
                startPosition = enemyToSpawn.startPosition,
                facingOutward = enemyToSpawn.facingOutward,
                boss = enemyToSpawn.boss,
                maxSpeed= enemyToSpawn.maxSpeed
            };
            gm.levelManager.actors.Add(bufferClass);
            bufferClass.AddEnemyOnMap(_splineContainer);
        }
    }

    protected override void CallEv_GameReady()
    {
        base.CallEv_GameReady();
        IsActive = true;
    }
    protected override void CallEv_LevelDoneWin(string st, bool victory)
    {
        base.CallEv_LevelDoneWin(st, victory);
        IsActive = false;
    }
}
