using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;
using UnityEngine.Splines;

public class RailCannon : EventManager
{
    internal GameManager gm;
    BoxCollider2D _boxCollider2D;
    internal SplineAnimate SplineAnimate;
    internal float DistanceTraveled;
    Transform _parentTr; 
    internal Transform MyTransform;
    public Vector2 rangeSpeed = new Vector2(-10f, 10f);

    Transform[] _bulletSpawnPoints = new Transform[3];
    LineRenderer[] _lrs = new LineRenderer[3];
    public Faction faction;
    ShootingMode _shootingMode;
    SpriteRenderer[] _sprites = new SpriteRenderer[2];
    internal Animator AnimTank, AnimTurret;
    internal bool IsActive
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
            AnimTank.enabled = _isActive;
            AnimTurret.enabled = _isActive;
            if (!_isActive && activeShield != null)
            {
                activeShield.End();
                activeShield = null;
            } 
        }
    }
    bool _isActive;
    Ray2D _ray2D;
    RaycastHit2D _hit2D;
    [HideInInspector] public Shield activeShield;

    public void Inicijalizacija(ShootingMode sm, float pocetnaPoz)
    {
        MyTransform = transform;
        _parentTr = MyTransform.parent;
        _boxCollider2D = GetComponent<BoxCollider2D>();
        gm = GameManager.gm;

        SplineAnimate = _parentTr.GetComponent<SplineAnimate>();
        SplineAnimate.normalizedTime = pocetnaPoz;

        _sprites[0] = MyTransform.GetChild(0).GetComponent<SpriteRenderer>();
        _sprites[1] = MyTransform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        AnimTank= _sprites[0].GetComponent<Animator>();
        AnimTurret = _sprites[1].GetComponent<Animator>();
        for (int i = 0; i < 3; i++)
        {
            _bulletSpawnPoints[i] = MyTransform.GetChild(1).GetChild(i);
            _lrs[i] = _bulletSpawnPoints[i].GetComponent<LineRenderer>();
            _lrs[i].startColor = faction == Faction.Ally ? gm.colAlly : gm.colEnemy;
        }
        _shootingMode = sm;
        IsActive = true;
    }

    protected virtual void Shooting()
    {
        if (!IsActive) return;
        AnimTurret.SetTrigger("fire");

        switch (_shootingMode)
        {
            case ShootingMode.OneBullet:
                gm.poolManager.ShootBullet(faction, _bulletSpawnPoints[0]);
                break;
            case ShootingMode.TwoBullets:
                gm.poolManager.ShootBullet(faction, _bulletSpawnPoints[1]);
                gm.poolManager.ShootBullet(faction, _bulletSpawnPoints[2]);
                break;
        }
    }
    protected virtual void Motion()
    {
        SplineAnimate.normalizedTime += Time.deltaTime * DistanceTraveled * 0.1f;
        if (SplineAnimate.normalizedTime > 1f)
        {
            if (gm.enemyAdditions != null && SplineAnimate.splineContainer != gm.splineContainer)
            {
                gm.levelManager.AddEnemy(SplineAnimate, _shootingMode, gm.enemyAdditions.insertionNorTime);
            }
            else SplineAnimate.normalizedTime = 0f;
        }

    }

    protected override void CallEv_LevelDoneWin(string message, int level, bool victory)
    {
        base.CallEv_LevelDoneWin(message, level, victory);
        if (IsActive) IsActive = false;
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
        if (collision.TryGetComponent(out RailCannon rc) && rc.IsActive)
        {
            LevelDoneWin?.Invoke("Don't touch the enemy!", gm.postavke.level, false);
        }
        else if (collision.TryGetComponent(out Shield shield) && shield.fact != faction)
        {
            shield.End();
        }
    }

}
