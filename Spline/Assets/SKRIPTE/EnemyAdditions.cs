using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;
using FirstCollection;
using System;

public class EnemyAdditions : EventManager
{
    GameManager gm;
    SplineContainer _splineContainer;
    public float insertionNorTime;
    const float CONST_SPAWNTIME = 6f;
    float _timerSpawn = Mathf.Infinity;
    bool _isActive;

    [SerializeField] ShootingMode shootingMode;

    private void Awake()
    {
        gm = GameManager.gm;
        gm.enemyAdditions = this;
        _splineContainer = GetComponent<SplineContainer>();
    }

    private void Update()
    {
        if (!_isActive) return;

        _timerSpawn += Time.deltaTime;
        if (_timerSpawn > CONST_SPAWNTIME && gm.levelManager.enemyCount <= gm.maxEnemyCount )
        {
            _timerSpawn = 0f;
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        SplineAnimate sa = Instantiate(gm.splineAnimatePrefab);
        sa.splineContainer = _splineContainer;
        sa.gameObject.SetActive(true);

        RailCannon rc = sa.transform.GetChild(1).GetComponent<RailCannon>();
        rc.transform.localEulerAngles = gm.levelManager.GetStartRotationEuler();
        rc.gameObject.SetActive(true);
        rc.Inicijalizacija(shootingMode, 0f);

    }

    protected override void CallEv_GameReady()
    {
        base.CallEv_GameReady();
        _isActive = true;
    }
    protected override void CallEv_LevelDoneWin(string st, int level, bool victory)
    {
        base.CallEv_LevelDoneWin(st, level, victory);
        _isActive = false;
    }
}
