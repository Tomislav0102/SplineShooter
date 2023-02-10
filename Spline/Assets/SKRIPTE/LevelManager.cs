using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;
using UnityEngine.Splines;

public class LevelManager : EventManager
{
    GameManager gm;
    [SerializeField] SplineContainer container;

    [SerializeField] int fuel = 1;
    [SerializeField] int ammo = 10;
    [SerializeField] Transform puParent;
    [SerializeField] Pickup puPrefab;


    public bool HasEnemiesWithCannons()
    {
        //for (int i = 1; i < shootingMode.Count; i++)
        //{
        //    if (shootingMode[i] != ShootingMode.NoBullets) return true;
        //}
        for (int i = 1; i < actors.Count; i++)
        {
            if (actors[i].sm != ShootingMode.NoBullets) return true;
        }
        return false;
    }
    public Transform parActors;
    public List<SpawnedActors> actors = new List<SpawnedActors>(1);

    private void Awake()
    {
        gm = GameManager.gm;
        gm.levelManager = this;
    }


    #region //OVERRIDES
    protected override void CallEv_GameReady()
    {
        base.CallEv_GameReady();
        container = GameObject.FindGameObjectWithTag("Path").GetComponent<SplineContainer>();
        gm.splineContainer = container;

        //actors initialization
        for (int i = 0; i < actors.Count; i++)
        {
            actors[i].AddEnemyOnMap(container);
        }
        gm.playerControll = actors[0].rc.GetComponent<PlayerControll>();

        //pickups and stats for player
        for (int i = 0; i < 3; i++)
        {
            bool hasPU = puParent.GetChild(i).gameObject.activeInHierarchy;
            if (hasPU)
            {
                Pickup currentPU = Instantiate(puPrefab, puParent.GetChild(i).position, Quaternion.identity);
                currentPU.pUtype = (PUtype)i;
                currentPU.gameObject.SetActive(true);
            }

            if (i == 1)
            {
                gm.playerControll.hasFuelFeature = hasPU;
                gm.playerControll.fuelTankCapacity = fuel;
                gm.uimanager.fuelParent.SetActive(hasPU);
            }
            else if (i == 2)
            {
                gm.playerControll.Ammo = ammo;
                gm.playerControll.hasAmmoFeature = hasPU;
                gm.uimanager.ammoParent.SetActive(hasPU);
            }
        }
    }


    protected override void CallEv_EnemyDestroyed(EnemyBehaviour enemyBehaviour)
    {
        base.CallEv_EnemyDestroyed(enemyBehaviour);
        for (int i = 1; i < actors.Count; i++)
        {
            if(enemyBehaviour == actors[i].rc)
            {
                actors.RemoveAt(i);
                return;
            }
        }
    }
    #endregion

}

[System.Serializable]
public class SpawnedActors
{
    GameManager _gm;
    [HideInInspector] public RailCannon rc;
    Transform _actorTransform;

    public ShootingMode sm;
    public float startPosition;
    public bool facingOutward;
    public float maxSpeed;
    public bool startsWithShield;
    public SplineAnimate boss;

    readonly Vector3 _faceIn = -90f * Vector3.forward;
    readonly Vector3 _faceOut = 90f * Vector3.forward;


    public void AddEnemyOnMap(SplineContainer splineContainer)
    {
        _gm = GameManager.gm;

        if (boss == null)
        {
            _actorTransform = _gm.poolManager.GetActor();
            if (_actorTransform == null) return;
        }
        else
        {
            _actorTransform = _gm.InstantiateMethod(boss.transform);
            _gm.uimanager.bossParent.SetActive(true);
        }

        _actorTransform.GetComponent<SplineAnimate>().splineContainer = splineContainer;
        _actorTransform.parent = _gm.levelManager.parActors;
        _actorTransform.gameObject.SetActive(true);

        rc = _actorTransform.GetComponentInChildren<RailCannon>();
        rc.transform.GetChild(1).localEulerAngles = facingOutward ? _faceOut : _faceIn;
        rc.gameObject.SetActive(true);
        rc.Inicijalizacija(sm, startPosition, maxSpeed, boss != null);
        if (startsWithShield) _gm.poolManager.DeployShield(rc);
    }

}


