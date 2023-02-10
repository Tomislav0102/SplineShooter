using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : RailCannon
{
    public bool isBoss;
    float _tajmerShoot = -5f;
    float _effDistanceTraveled;
    private void Start()
    {
        animTank.SetBool("isMoving", false);
        _effDistanceTraveled = 0f;
        Invoke(nameof(BeginPause), 2f);
    }
    void BeginPause()
    {
        animTank.SetBool("isMoving", true);
        _effDistanceTraveled = maxSpeed;
    }
    protected override void Motion()
    {
        distanceTraveled = _effDistanceTraveled;
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
