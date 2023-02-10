using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;

public class Shield : MonoBehaviour, ITakeDamage
{
    GameManager gm;
    [HideInInspector] public Faction fact;
    RailCannon _railCannon;
    Transform _rcTransform;
    Transform _myTransform;
    int _hitPoints;
    [SerializeField] int maxHitPoints = 1;
    [SerializeField] SpriteRenderer sprite;
    private void Awake()
    {
        _myTransform = transform;
    }
    private void Update()
    {
        if (_rcTransform != null) _myTransform.position = _rcTransform.position;
    }
    public void DeployMe(RailCannon rc)
    {
        _hitPoints = maxHitPoints;
        _railCannon = rc;
        _railCannon.activeShield = this;
        fact = _railCannon.faction;
        _rcTransform = _railCannon.transform;
        if (gm == null) gm = GameManager.gm;
        sprite.color = fact == Faction.Ally ? gm.colAlly : gm.colEnemy;
    }
    void End()
    {
        if(_railCannon!= null)
        {
            _railCannon.activeShield = null;
            _railCannon = null;
        }
        gameObject.SetActive(false);
    }

    public void TakeDamage(Faction attackerFaction, int dam)
    {
        _hitPoints -= dam;
        if (_hitPoints <= 0) End();
    }
}