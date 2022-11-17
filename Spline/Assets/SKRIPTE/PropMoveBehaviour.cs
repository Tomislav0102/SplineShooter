using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;

public class PropMoveBehaviour : EventManager
{
    [SerializeField] MotionType motionType;
    [SerializeField] float speed;
    Transform _myTransform;
    public bool isActive;

    private void Awake()
    {
        _myTransform = transform;
    }

    private void Update()
    {
        if (!isActive) return;

        switch (motionType)
        {
            case MotionType.Rotation:
                _myTransform.Rotate(speed * Time.deltaTime * Vector3.forward);
                break;
        }
    }
    protected override void GameStart()
    {
        base.GameStart();
        isActive = true;
    }
    protected override void EndWin(string st, int level, bool victory)
    {
        base.EndWin(st, level, victory);
        isActive = false;
    }
}
