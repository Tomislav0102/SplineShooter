using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class EnemyBehaviour : RailCannon
{
    float _tajmerShoot = -5f;


    private void Start()
    {
        _animTank.SetInteger("faction", (int)faction);
        _animTurret.SetInteger("faction", (int)faction);
        _animTank.SetBool("isMoving", true);

    }
    protected override void Motion()
    {
        // _currSpeed += _acceleration * _acceleration * Time.deltaTime;
        //  _currSpeed = Mathf.Clamp(_currSpeed, rangeSpeed.x, rangeSpeed.y);
        distanceTraveled = rangeSpeed.y;
        base.Motion();
    }
    protected override void EnemyRemoved(EnemyBehaviour enemyBehaviour)
    {
        base.EnemyRemoved(enemyBehaviour);
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
