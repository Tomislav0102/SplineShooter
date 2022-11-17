using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;

public class Pickup : EventManager
{
    public PUtype pUtype;
    int _currentPU;
    GameManager gm;
    GameObject[] gos = new GameObject[3];
    Transform _myTransform;
    BoxCollider2D _boxCollider;
    bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            gos[_currentPU].SetActive(_isActive);
            _boxCollider.enabled = _isActive;
        }
    }
    bool _isActive;

    private void Awake()
    {
        gm = GameManager.gm;
        _myTransform = transform;
        _boxCollider = GetComponent<BoxCollider2D>();
        for (int i = 0; i < _myTransform.childCount; i++)
        {
            gos[i] = _myTransform.GetChild(i).gameObject;
        }
    }
    private void OnEnable()
    {
        for (int i = 0; i < _myTransform.childCount; i++)
        {
            gos[i].SetActive(false);
        }
        _currentPU = pUtype == PUtype.Random ? Random.Range(0, 3) : (int)pUtype;
        IsActive = true;

        InvokeRepeating(nameof(MetodaRepeat), Random.Range(4f, 6f), 4f);
    }

    void MetodaRepeat()
    {
        IsActive = !IsActive;
    }
    public void OnPickup()
    {
        switch (pUtype)
        {
            case PUtype.Diamond:
                gm.PointsMethod(100);
                break;
            case PUtype.Fuel:
                gm.playerControll.Refuel();
                break;
            case PUtype.Ammo:
                gm.playerControll.Ammo = 10;
                break;
        }
        CancelInvoke();
        IsActive = false;
        InvokeRepeating(nameof(MetodaRepeat), 4f, 4f);
    }
    
}
