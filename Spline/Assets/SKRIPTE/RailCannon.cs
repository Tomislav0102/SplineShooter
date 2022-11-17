using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;
using UnityEngine.Splines;

public class RailCannon : EventManager
{
    internal GameManager gm;
    SplineContainer _container;
    BoxCollider2D _boxCollider2D;
    internal SplineAnimate splineAnimate;
    internal float distanceTraveled;
    Transform _parentTr; 
    internal Transform _myTransform;
    public Vector2 rangeSpeed = new Vector2(-10f, 10f);

    Transform[] _bulletSpawnPoints = new Transform[3];
    LineRenderer[] _lrs = new LineRenderer[3];
    public Faction faction;
    ShootingMode _shootingMode;
    SpriteRenderer[] _sprites = new SpriteRenderer[2];
    internal Animator _animTank, _animTurret;
    bool _isActive;
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

            for (int i = 0; i < _sprites.Length; i++)
            {
                _sprites[i].enabled = _isActive;
            }
            _animTank.enabled = _isActive;
            _animTurret.enabled = _isActive;
        }
    }
    Ray2D ray2D;
    RaycastHit2D hit2D;

    public IEnumerator Inicijalizacija(ShootingMode sm, float pocetnaPoz)
    {
        _myTransform = transform;
        _parentTr = _myTransform.parent;
        _boxCollider2D = GetComponent<BoxCollider2D>();
        gm = GameManager.gm;

        _container = gm.splineContainer;
        splineAnimate = _parentTr.GetComponent<SplineAnimate>();
        splineAnimate.normalizedTime = pocetnaPoz;

        _sprites[0] = _myTransform.GetChild(0).GetComponent<SpriteRenderer>();
        _sprites[1] = _myTransform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>();
        _animTank= _sprites[0].GetComponent<Animator>();
        _animTurret = _sprites[1].GetComponent<Animator>();
        for (int i = 0; i < 3; i++)
        {
            _bulletSpawnPoints[i] = _sprites[1].transform.GetChild(i);
            _lrs[i] = _bulletSpawnPoints[i].GetComponent<LineRenderer>();
            _lrs[i].startColor = faction == Faction.Ally ? gm.colAlly : gm.colEnemy;
        }
        _shootingMode = sm;
        yield return null;
        IsActive = true;
    }

    protected virtual void Shooting()
    {
        if (!IsActive) return;
        _animTurret.SetTrigger("fire");

        switch (_shootingMode)
        {
            case ShootingMode.OneBullet:
                gm.ShootBullet(faction, _bulletSpawnPoints[0]);
                break;
            case ShootingMode.TwoBullets:
                gm.ShootBullet(faction, _bulletSpawnPoints[1]);
                gm.ShootBullet(faction, _bulletSpawnPoints[2]);
                break;
        }
    }
    protected virtual void Motion()
    {
        splineAnimate.normalizedTime += Time.deltaTime * distanceTraveled * 0.1f;
        if (splineAnimate.normalizedTime > 1f) splineAnimate.normalizedTime = 0f;

    }

    protected override void EndWin(string message, int level, bool victory)
    {
        base.EndWin(message, level, victory);
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
        ray2D.origin = _bulletSpawnPoints[counterSpawnPoint].position;
        ray2D.direction = _bulletSpawnPoints[counterSpawnPoint].up;
        hit2D = Physics2D.Raycast(ray2D.origin, ray2D.direction, 20f);
        _lrs[counterSpawnPoint].SetPosition(0, ray2D.origin);
        _lrs[counterSpawnPoint].startWidth = _lrs[counterSpawnPoint].endWidth = 0.1f;
        if (hit2D)
        {
            if (hit2D.collider.TryGetComponent(out Projectile projectile) && projectile.faction == faction)
            {
                return;
            }
            _lrs[counterSpawnPoint].SetPosition(1, hit2D.point);
            _lrs[counterSpawnPoint].startWidth = _lrs[counterSpawnPoint].endWidth = 0.5f;
        }
        else
        {
            _lrs[counterSpawnPoint].SetPosition(1, _bulletSpawnPoints[counterSpawnPoint].position + 20f * _bulletSpawnPoints[counterSpawnPoint].up);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision) //samo za cannon vs cannon, ne radi za metke
    {
        if (!IsActive) return;
        if (collision.TryGetComponent(out RailCannon rc) && rc.IsActive)
        {
            EventManager.LevelDoneWin("Don't touch the enemy!", gm.postavke.level, false);
        }
    }

}
