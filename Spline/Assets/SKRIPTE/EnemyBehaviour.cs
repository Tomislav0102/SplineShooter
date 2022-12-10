using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Mathematics;

public class EnemyBehaviour : RailCannon
{
    float _tajmerShoot = -5f;
    private void Start()
    {
        AnimTank.SetInteger("faction", (int)faction);
        AnimTurret.SetInteger("faction", (int)faction);
        AnimTank.SetBool("isMoving", true);

    }
    protected override void Motion()
    {
        // _currSpeed += _acceleration * _acceleration * Time.deltaTime;
        //  _currSpeed = Mathf.Clamp(_currSpeed, rangeSpeed.x, rangeSpeed.y);
        DistanceTraveled = rangeSpeed.y;
        base.Motion();
    }
    protected override void CallEv_EnemyDestroyed(EnemyBehaviour enemyBehaviour)
    {
        base.CallEv_EnemyDestroyed(enemyBehaviour);
        if (enemyBehaviour == this) IsActive = false;
    }
    private void Update()
    {
        if (!IsActive) return;
        Motion();
        _tajmerShoot += Time.deltaTime;
        if(_tajmerShoot > 5f)
        {
            _tajmerShoot = 0f;
            Shooting();
        }
        RaysMethod();
    }


}
