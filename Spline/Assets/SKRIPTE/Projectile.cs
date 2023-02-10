using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;

public class Projectile : MonoBehaviour
{
    GameManager gm;
    Transform _myTransform;
    Rigidbody2D _r2D;
    float _speed;
    const float CONST_BASESPEED = 0.1f;
    bool _pomocniOneHit;
    [HideInInspector] public Faction faction;
    [HideInInspector] public ProjectileType projectileType;
    [SerializeField] SpriteRenderer sprite;

    [SerializeField] int damage = 1;

    private void Awake()
    {
        gm = GameManager.gm;
        _myTransform = transform;
        _r2D = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        switch (faction)
        {
            case Faction.Ally:
                _speed = CONST_BASESPEED * 2f;
                break;
            case Faction.Enemy:
                _speed = CONST_BASESPEED;
                break;
        }
        switch (projectileType)
        {
            case ProjectileType.Blue:
                sprite.color = gm.colAlly;
                break;
            case ProjectileType.Yellow:
                sprite.color = gm.colEnemy;
                break;
            case ProjectileType.Red:
                sprite.color = gm.colRed;
                break;
        }
        StartCoroutine(EnableContinuation());
    }
    IEnumerator EnableContinuation()
    {
        float startTime = CONST_BASESPEED / (_speed * 15f);
        yield return new WaitForSeconds(startTime);
        _pomocniOneHit = true;
        yield return new WaitForSeconds(3f);
        End();
    }

    private void FixedUpdate()
    {
        _r2D.MovePosition(_myTransform.position + _speed * _myTransform.up);
    }

    public void End()
    {
        StopAllCoroutines();
        _pomocniOneHit = false;
        gameObject.SetActive(false);
    }

    IEnumerator ReflexijaPauza()
    {
        _pomocniOneHit = false;
        yield return new WaitForSeconds(0.1f);
        _pomocniOneHit = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!_pomocniOneHit) return;
     //   print(collision.name);
        if (collision.TryGetComponent(out Tile tl))
        {
            if (tl.CurrentState == TileState.Closed)
            {
                gm.gridControll.BulletHitTile(faction, _myTransform, tl.pozicija);
            }
        }
        else if (collision.TryGetComponent(out ITakeDamage takeDamage))
        {
            takeDamage.TakeDamage(faction, damage);
        }
        else if (collision.CompareTag("Zrcalo"))
        {
            _myTransform.up = Vector2.Reflect(_myTransform.up, collision.transform.right);
            StartCoroutine(ReflexijaPauza());
            return;
        }
        //else if (collision.TryGetComponent(out RailCannon railCannon) && railCannon.IsActive)
        //{
        //    switch (railCannon.faction)
        //    {
        //        case Faction.Ally:
        //            string st = faction == Faction.Ally ? "You killed yourself!" : "Killed by enemy bullet...";
        //            gm.playerControll.TakeDamage(damage, st);
        //            break;
        //        case Faction.Enemy:
        //            railCannon.TakeDamage(damage, "");
        //            break;
        //    }
        //}
        //else if (collision.TryGetComponent(out Projectile projectile))
        //{
        //    if (faction != projectile.faction) projectile.End();
        //    else return;
        //}
        //else if (collision.TryGetComponent(out Pickup pu))
        //{
        //    pu.OnPickup(faction);
        //}
        //else if (collision.TryGetComponent(out Shield shield) && shield.fact != faction)
        //{
        //    shield.End();
        //}
        //else if (collision.CompareTag("Barrier"))
        //{
        //    collision.gameObject.SetActive(false);
        //}

        End();

    }
}
