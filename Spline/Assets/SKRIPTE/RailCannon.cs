using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;
using UnityEngine.Splines;

public class RailCannon : EventManager, ITakeDamage, IActivation
{
    internal GameManager gm;
    BoxCollider2D _boxCollider2D;
    internal SplineAnimate splineAnimate;
    internal float distanceTraveled;
    Transform _parentTr; 
    internal Transform myTransform;
    internal float maxSpeed;
    public int maxHitPoints = 1;
    int _hitpoints;

    Transform[] _bulletSpawnPoints = new Transform[3];
    LineRenderer[] _lrs = new LineRenderer[3];
    public Faction faction;
    ShootingMode _shootingMode;
    [SerializeField] ProjectileType projectileType;
    SpriteRenderer[] _sprites = new SpriteRenderer[2];
    internal Animator animTank, animTurret;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            _boxCollider2D.enabled = _isActive;
            for (int i = 0; i < _lrs.Length; i++)
            {
                _lrs[i].enabled = false;
            }

            for (int i = 0; i < _sprites.Length; i++)
            {
                _sprites[i].enabled = _isActive;
            }
            switch (_shootingMode)
            {
                case ShootingMode.NoBullets:
                    _sprites[1].enabled = false;
                    break;
                case ShootingMode.OneBullet:
                    _lrs[0].enabled = _isActive;
                    break;
                case ShootingMode.TwoBullets:
                    _lrs[1].enabled = _isActive;
                    _lrs[2].enabled = _isActive;
                    break;
            }
            animTank.enabled = _isActive;
            animTurret.enabled = _isActive;
            if (!_isActive)
            {
                if(activeShield != null)
                {
                    activeShield.TakeDamage(faction, 100000);
                    activeShield = null;
                }

                gm.poolManager.ReturnActor(_parentTr);
            } 
        }
    }
    bool _isActive;
    Ray2D _ray2D;
    RaycastHit2D _hit2D;
    [HideInInspector] public Shield activeShield;

    public void Inicijalizacija(ShootingMode sm, float pocetnaPoz, float speed, bool isboss)
    {
        myTransform = transform;
        _parentTr = myTransform.parent;
        _boxCollider2D = GetComponent<BoxCollider2D>();
        gm = GameManager.gm;

        splineAnimate = _parentTr.GetComponent<SplineAnimate>();
        splineAnimate.normalizedTime = pocetnaPoz;

        _sprites[0] = myTransform.GetChild(0).GetComponent<SpriteRenderer>();
        _sprites[1] = myTransform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
        animTank = _sprites[0].GetComponent<Animator>();
        animTurret = _sprites[1].GetComponent<Animator>();
        for (int i = 0; i < 3; i++)
        {
            _bulletSpawnPoints[i] = myTransform.GetChild(1).GetChild(1).GetChild(i);
            _lrs[i] = _bulletSpawnPoints[i].GetComponent<LineRenderer>();
            switch (projectileType)
            {
                case ProjectileType.Blue:
                    _lrs[i].startColor = gm.colAlly;
                    break;
                case ProjectileType.Yellow:
                    _lrs[i].startColor = gm.colEnemy;
                    break;
                case ProjectileType.Red:
                    _lrs[i].startColor = gm.colRed;
                    break;
            }


        }
        _shootingMode = sm;
        _hitpoints = maxHitPoints;
        maxSpeed = speed;
        if (GetComponent<EnemyBehaviour>() != null) GetComponent<EnemyBehaviour>().isBoss = isboss;
        IsActive = true;
    }

    #region//OVERRIDES
    protected override void CallEv_LevelDoneWin(string message, bool victory)
    {
        base.CallEv_LevelDoneWin(message, victory);
        if (IsActive) IsActive = false;
    }
    #endregion
    #region//VIRTUALS
    protected virtual void Motion()
    {
        splineAnimate.normalizedTime += Time.deltaTime * distanceTraveled * 0.1f;
        if (splineAnimate.normalizedTime > 1f)
        {
            if (gm.enemyAdditions != null && splineAnimate.splineContainer != gm.splineContainer)
            {
                splineAnimate.splineContainer = gm.splineContainer;
                splineAnimate.normalizedTime = gm.enemyAdditions.insertionNorTime;
            }
            else splineAnimate.normalizedTime = 0f;
        }

    }
    #endregion
    #region//INTERNALS
    internal void Shooting()
    {
        if (!IsActive) return;
        animTurret.SetTrigger("fire");

        switch (_shootingMode)
        {
            case ShootingMode.OneBullet:
                gm.poolManager.ShootBullet(faction, projectileType, _bulletSpawnPoints[0]);
                break;
            case ShootingMode.TwoBullets:
                gm.poolManager.ShootBullet(faction, projectileType, _bulletSpawnPoints[1]);
                gm.poolManager.ShootBullet(faction, projectileType, _bulletSpawnPoints[2]);
                break;
            case ShootingMode.ThreeBullets:
                gm.poolManager.ShootBullet(faction, projectileType, _bulletSpawnPoints[0]);
                gm.poolManager.ShootBullet(faction, projectileType, _bulletSpawnPoints[1]);
                gm.poolManager.ShootBullet(faction, projectileType, _bulletSpawnPoints[2]);
                break;
        }
    }
    internal void RaysMethod()
    {
        switch (_shootingMode)
        {
            case ShootingMode.OneBullet:
                OneRay(0);
                break;
            case ShootingMode.TwoBullets:
                OneRay(1);
                OneRay(2);
                break;
        }
    }
    #endregion

    void OneRay(int counterSpawnPoint)
    {
        _ray2D.origin = _bulletSpawnPoints[counterSpawnPoint].position;
        _ray2D.direction = _bulletSpawnPoints[counterSpawnPoint].up;
        _hit2D = Physics2D.Raycast(_ray2D.origin, _ray2D.direction, 20f);
        _lrs[counterSpawnPoint].SetPosition(0, _ray2D.origin);
        _lrs[counterSpawnPoint].SetPosition(1, _bulletSpawnPoints[counterSpawnPoint].position + 20f * _bulletSpawnPoints[counterSpawnPoint].up);
        _lrs[counterSpawnPoint].startWidth = _lrs[counterSpawnPoint].endWidth = 0.1f;
        if (_hit2D)
        {
            if (_hit2D.collider.TryGetComponent(out Projectile projectile) && projectile.faction == faction ||
                _hit2D.collider.TryGetComponent(out Shield shield) && shield.fact == faction)
            {
                return;
            }
            _lrs[counterSpawnPoint].SetPosition(1, _hit2D.point);
            _lrs[counterSpawnPoint].startWidth = _lrs[counterSpawnPoint].endWidth = 0.5f;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) //samo za cannon vs cannon i cannon vs shield, ne radi za metke
    {
        if (!IsActive) return;

        if (collision.TryGetComponent(out Shield shield))
        {
           if(shield.fact != faction) shield.TakeDamage(faction, 1);
        }
        else if (collision.TryGetComponent(out ITakeDamage takeDamage))
        {
            takeDamage.TakeDamage(faction, 1);
        }
        //else if (collision.TryGetComponent(out RailCannon rc) && rc.IsActive)
        //{
        //    if (faction == Faction.Ally) TakeDamage(1, "Don't touch the enemy!");
        //    else if (faction == Faction.Enemy && rc.faction == Faction.Enemy)
        //    {
        //        TakeDamage(1, "");
        //        rc.TakeDamage(1, "");
        //    }
        //}

    }

    public void TakeDamage(Faction attackerFaction, int dam)
    {
        _hitpoints -= dam;
        switch (faction)
        {
            case Faction.Ally:
                if (_hitpoints <= 0) LevelDoneWin?.Invoke(attackerFaction == Faction.Ally ? "You killed yourself!" : "Killed by enemy..." , false);
                break;
            case Faction.Enemy:
                gm.uimanager.BossHealthDisplay((float)_hitpoints / (float)maxHitPoints);
                if (_hitpoints <= 0)
                {
                    gm.uimanager.bossParent.SetActive(false);
                    EnemyDestroyed?.Invoke(GetComponent<EnemyBehaviour>());
                }
                break;
        }

    }
}
