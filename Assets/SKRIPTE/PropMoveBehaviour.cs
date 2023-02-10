using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;
using DG.Tweening;

public class PropMoveBehaviour : EventManager, IActivation
{
    [SerializeField] Transform transformToMove;
    [SerializeField] MotionType motionType;
    [SerializeField] float speed;
    [Header("ROTATION")]
    [SerializeField] Transform[] group;
    Vector2 _centerOfGroup = Vector2.zero;
    [Header("TRANSLATION")]
    List<Vector3> patrolPoints = new List<Vector3>();
    [SerializeField] bool circularPatrol;

    public bool IsActive { get => _isActive; set => _isActive = value; }
    bool _isActive;
    [SerializeField] bool activeAtStart = true;
    private void Awake()
    {
        if (transformToMove == null) transformToMove = transform;
    }
    private void Start()
    {
        IsActive = activeAtStart;
        if (!IsActive) gameObject.SetActive(false);

        switch (motionType)
        {
            case MotionType.RotateGroup:
                if (group != null && group.Length > 0) _centerOfGroup = group[0].position;
                break;
            case MotionType.Translation:
                for (int i = 0; i < transform.childCount; i++)
                {
                    patrolPoints.Add(transform.GetChild(i).position);
                }
                if (patrolPoints.Count <= 1) return;
                if (circularPatrol) patrolPoints.Add(transform.GetChild(0).position);
                transformToMove.DOPath(patrolPoints.ToArray(), speed)
                    .SetSpeedBased(true)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, circularPatrol ? LoopType.Restart : LoopType.Yoyo);
                break;

        }

    }

    private void Update()
    {
        if (!IsActive) return;

        switch (motionType)
        {
            case MotionType.Rotation:
                transformToMove.Rotate(speed * Time.deltaTime * Vector3.forward);
                break;
            case MotionType.RotateGroup:
                foreach (Transform item in group)
                {
                    item.RotateAround(_centerOfGroup, Vector3.forward, speed * Time.deltaTime);
                }
                break;
        }
    }
    protected override void CallEv_GameReady()
    {
        base.CallEv_GameReady();
        IsActive = true;
    }
    protected override void CallEv_LevelDoneWin(string st, bool victory)
    {
        base.CallEv_LevelDoneWin(st, victory);
        IsActive = false;
    }
}
