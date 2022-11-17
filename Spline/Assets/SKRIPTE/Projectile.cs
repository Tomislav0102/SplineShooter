using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;

public class Projectile : MonoBehaviour
{
    GameManager gm;
    Transform _myTransform;
    Rigidbody2D _r2D;
    float speed;
    bool pomocniOneHit;
    [HideInInspector] public Faction faction;
    [SerializeField] SpriteRenderer sprite;
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
                speed = 10f;
                sprite.color = gm.colAlly;
                break;
            case Faction.Enemy:
                speed = 5f;
                sprite.color = gm.colEnemy;
                break;
        }

        _r2D.velocity = _myTransform.up * speed;
        Invoke(nameof(End), 3f);
        pomocniOneHit = true;
    }


    public void End()
    {
        CancelInvoke();
        _r2D.velocity = Vector2.zero;
        gameObject.SetActive(false);
    }
    IEnumerator ReflexijaPauza()
    {
        yield return new WaitForSeconds(0.1f);
        pomocniOneHit = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pomocniOneHit) return;
        pomocniOneHit = false;
        if (collision.TryGetComponent(out Tile tl))
        {
            if (tl.CurrentState == TileState.Closed)
            {
                gm.gridControll.BulletHitTile(faction, _myTransform, tl.pozicija);
            }
        }
        else if (collision.TryGetComponent(out RailCannon railCannon) && railCannon.IsActive)
        {
            switch (railCannon.faction)
            {
                case Faction.Ally:
                    string st = faction == Faction.Ally ? "You killed yourself!" : "Killed by enemy bullet...";
                    EventManager.LevelDoneWin(st, gm.postavke.level, false);
                    break;
                case Faction.Enemy:
                    // railCannon.IsActive = false;
                    EventManager.EnemyDestroyed(railCannon.GetComponent<EnemyBehaviour>());
                    break;
            }
        }
        else if (collision.TryGetComponent(out Projectile projectile))
        {
            if (faction != projectile.faction) projectile.End();
            else
            {
                pomocniOneHit = true;
                return;
            }
        }
        else if(collision.TryGetComponent(out Pickup pu) && faction == Faction.Ally)
        {
            pu.OnPickup();
        }
        else if (collision.CompareTag("Zrcalo"))
        {
            Vector2 v2 = _myTransform.up;
            _myTransform.up = Vector2.Reflect(v2, collision.transform.right);
            _r2D.velocity = _myTransform.up * speed;
            StartCoroutine(ReflexijaPauza());
            return;
        }
        else return;

        End();

    }
}
