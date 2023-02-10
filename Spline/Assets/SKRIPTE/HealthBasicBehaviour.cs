using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;

public class HealthBasicBehaviour : MonoBehaviour, ITakeDamage
{
    int _hitPoints;
    [Header("-1 maxhitpoints is invulnerability")]
    [SerializeField] int maxHitPoints = 1;

    private void OnEnable()
    {
        _hitPoints = maxHitPoints;
    }
    public void TakeDamage(Faction attackerFaction, int dam)
    {
        if (maxHitPoints < 0) return;

        _hitPoints -= dam;
        if (_hitPoints <= 0) gameObject.SetActive(false);

    }
}
