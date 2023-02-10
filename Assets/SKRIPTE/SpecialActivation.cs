using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;

public class SpecialActivation : EventManager
{
    [SerializeField] ConditionForActivation condition;
    [SerializeField] bool activateWhenConditionMet;
    [SerializeField] GameObject[] objToActivate;
    IActivation[] _aktivations;

    private void Awake()
    {
        if (objToActivate.Length == 0) this.enabled = false;

        _aktivations = new IActivation[objToActivate.Length];
        for (int i = 0; i < _aktivations.Length; i++)
        {
            if (objToActivate[i].TryGetComponent(out IActivation act))
            {
                _aktivations[i] = objToActivate[i].GetComponent<IActivation>();
            }
            else this.enabled = false;
        }
    }

    protected override void CallEv_EnemyDestroyed(EnemyBehaviour enemyBehaviour)
    {
        base.CallEv_EnemyDestroyed(enemyBehaviour);
        switch (condition)
        {
            case ConditionForActivation.BossKilled:
                if (enemyBehaviour.isBoss)
                {
                    for (int i = 0; i < _aktivations.Length; i++)
                    {
                        _aktivations[i].IsActive = activateWhenConditionMet;
                    }
                }
                break;
        }
    }

}
