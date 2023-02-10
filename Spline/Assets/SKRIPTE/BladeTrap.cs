using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;
using DG.Tweening;

public class BladeTrap : EventManager, IActivation
{
    Transform _myTransform;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            if (_isActive)
            {
                TweenRotate();
            }
        }
    }
    bool _isActive;
    bool _canDamage; //ensures only one hit per trigger

    void Awake()
    {
        _myTransform = transform;
    }


    void TweenRotate()
    {
        if (!IsActive) return;
        _canDamage = true;

        _myTransform.DORotate(360f * Vector3.forward, 1.5f)
            .SetDelay(1f)
            .SetRelative()
            .SetEase(Ease.InBack)
            .OnComplete(TweenRotate);
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (!IsActive || !_canDamage) return;
        _canDamage = false;
        if (collision.TryGetComponent(out ITakeDamage takeDamage))
        {
            takeDamage.TakeDamage(Faction.Neutral, 1);
        }

    }

}