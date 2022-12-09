using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;
using UnityEngine.Splines;
using Unity.VisualScripting;

public class LevelManager : EventManager
{
    GameManager gm;
   // [SerializeField] SplineAnimate player, enemy;
    [SerializeField] SplineContainer container;
    [SerializeField] Transform parActors;
    List<RailCannon> _allActors = new List<RailCannon>();
    [SerializeField] RailCannon playerPrefab;
    [SerializeField] RailCannon enemyPrefab;
    [SerializeField] SplineAnimate splineAnimatePrefab;
    List<SplineAnimate> _splineAnimates = new List<SplineAnimate>();

    [Header("LEVEL SPECIFIC")]
    readonly Vector3 _faceIn = new Vector3(90f, 90f, 180f);
    readonly Vector3 _faceOut = new Vector3(90f, 180f, 90f);

    [SerializeField] bool facingOutward;
    int _enemyCount;
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
        for (int i = 1; i < shootingMode.Length; i++)
        {
            _enemyCount++;
            if (shootingMode[i] != ShootingMode.NoBullets) enemyWithCannonsCount++;
        }
    }

    protected override void GameStart()
    {
        base.GameStart();
        container = GameObject.FindGameObjectWithTag("Path").GetComponent<SplineContainer>();
        gm.splineContainer = container;

        //actors initialization
        for (int i = 0; i < shootingMode.Length; i++)
        {
            _splineAnimates.Add(Instantiate(splineAnimatePrefab, parActors));
            _splineAnimates[i].splineContainer = container;
            _splineAnimates[i].gameObject.SetActive(true);

            _allActors.Add(Instantiate((i == 0 ? playerPrefab : enemyPrefab), _splineAnimates[i].transform));
            RailCannon rc = _allActors[i];
            rc.transform.localPosition = Vector3.zero;
            rc.transform.localEulerAngles = facingOutward ? _faceOut : _faceIn;
            StartCoroutine(rc.Inicijalizacija(shootingMode[i], startPositions[i]));
        }

        gm.playerControll = _allActors[0].GetComponent<PlayerControll>();

        //fuel and ammo stats for player
        if (fuel <= 0)
        {
            gm.playerControll.hasFuelFeature = false;
            gm.uimanager.fuelParent.SetActive(false);
        }
        if (ammo > 0) gm.playerControll.Ammo = ammo;
        else
        {
            gm.playerControll.hasAmmoFeature = false;
            gm.uimanager.ammoParent.SetActive(false);
        }

        //pickups
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
        _enemyCount--;
        for (int i = 1; i < _allActors.Count; i++)
        {
            if(enemyBehaviour == _allActors[i] && shootingMode[i] != ShootingMode.NoBullets )
            {
                enemyWithCannonsCount--;
                return;
            }
        }
    }






}

