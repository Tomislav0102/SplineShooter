using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;

public class PropMoveBehaviour : EventManager
{
    [SerializeField] MotionType motionType;
    [SerializeField] float speed;
    [SerializeField] Transform[] group;
    Vector2 _centerOfGroup = Vector2.zero;
    Transform _myTransform;
    public bool isActive;

    private void Awake()
    {
        _myTransform = transform;
    }
    private void Start()
    {
        switch (motionType)
        {
            case MotionType.RotateGroup:
                if (group != null && group.Length > 0) _centerOfGroup = group[0].position;
                break;
        }

    }

    private void Update()
    {
        if (!isActive) return;

        switch (motionType)
        {
            case MotionType.Rotation:
                _myTransform.Rotate(speed * Time.deltaTime * Vector3.forward);
                break;
            case MotionType.RotateGroup:
                foreach (Transform item in group)
                {
                    item.RotateAround(_centerOfGroup, Vector3.forward, speed * Time.deltaTime);
                }
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
