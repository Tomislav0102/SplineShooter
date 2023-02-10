using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControll : RailCannon
{
    float _acceleration = 3f;

    public int Ammo
    {
        get => _ammo;
        set
        {
            _ammo = value;
            if (hasAmmoFeature) gm.uimanager.AmmoDisplay(_ammo);
            if (_ammo <= 0) StartCoroutine(AmmoPause());
        }
    }
    int _ammo;
    [HideInInspector] public bool hasAmmoFeature = true;

    float _fuel;
    [HideInInspector] public float fuelTankCapacity = 1f;
    [HideInInspector] public bool hasFuelFeature = true;
    Vector2 _currentPos, _oldPos;
    float _totalDistance, _lengthOfCurve, _fuelRemaining;

    private void Start()
    {
        _oldPos = myTransform.position;
        _lengthOfCurve = gm.splineContainer.CalculateLength();

        Refuel();
      //  print(gm.splineContainer.EvaluatePosition(0.79f));
    }

    private void Update()
    {
        if (!IsActive) return;
        Motion();

        FuelManagement();

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (hasAmmoFeature && Ammo > 0)
            {
                Ammo--;
                Shooting();
            }
            else Shooting();
        }
        RaysMethod();
    }
    protected override void Motion()
    {
        bool keyPressed = Input.GetKey(KeyCode.Space);

        if (keyPressed) distanceTraveled += _acceleration * _acceleration * Time.deltaTime;
        else distanceTraveled -= _acceleration * 2f * Time.deltaTime;
        distanceTraveled = Mathf.Clamp(distanceTraveled, 0f, maxSpeed);

        animTank.SetBool("isMoving", keyPressed);

        base.Motion();
    }

    private void FuelManagement()
    {
        if (!hasFuelFeature) return;
        
        _currentPos = myTransform.position; 
        _totalDistance += (_currentPos - _oldPos).magnitude;
        _oldPos = _currentPos;
        _fuel = (_fuelRemaining - _totalDistance) / _lengthOfCurve;
        gm.uimanager.FuelDisplay(_fuel * 0.5f);
        if (_fuel < 0f) LevelDoneWin?.Invoke("Out of fuel!", false);
    }

    public void Refuel()
    {
        _fuelRemaining = _lengthOfCurve * 2f * fuelTankCapacity + _totalDistance;
    }
    IEnumerator AmmoPause()
    {
        yield return new WaitForSeconds(2f);
        if (Ammo <= 0) LevelDoneWin?.Invoke("No more bullets!", false);
    }

}
