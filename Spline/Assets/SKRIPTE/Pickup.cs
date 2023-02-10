using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FirstCollection;

public class Pickup : EventManager, ITakeDamage, IActivation
{
    public PUtype pUtype;
    GameManager gm;
    GameObject[] _gos = new GameObject[3];
    Transform _myTransform;
    BoxCollider2D _boxCollider;
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;
            _gos[(int)pUtype].SetActive(_isActive);
            _boxCollider.enabled = _isActive;
            canvas.enabled = _isActive;
            _timer = CONST_DURATION;
        }
    }
    bool _isActive;
    const float CONST_DURATION = 8f;
    float _timer;

    [SerializeField] Canvas canvas;
    Image _imgTimer;
    readonly Color _startCol = new Color(0f, 1f, 0f, 1f);
    readonly Color _endCol = new Color(1f, 0f, 0f, 1f);

    private void Awake()
    {
        gm = GameManager.gm;
        _myTransform = transform;
        _boxCollider = GetComponent<BoxCollider2D>();
        for (int i = 0; i < _gos.Length; i++)
        {
            _gos[i] = _myTransform.GetChild(i).gameObject;
        }

        canvas.worldCamera = gm.mainCam;
        canvas.enabled = false;
        _imgTimer = canvas.transform.GetChild(0).GetComponent<Image>();
        _imgTimer.fillAmount = 1f;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        for (int i = 0; i < _gos.Length; i++)
        {
            _gos[i].SetActive(false);
        }
        if (pUtype == PUtype.Random) pUtype = (PUtype)Random.Range(0, 3);

        Invoke(nameof(MetodaRepeat), Random.Range(4f, 6f));
    }

    void Update()
    {
        if (IsActive)
        {
            if (pUtype == PUtype.Diamond && gm.playerControll.activeShield != null) RestartPU();

            _timer -= Time.deltaTime;
            _imgTimer.fillAmount = _timer / CONST_DURATION;
            _imgTimer.color = Color.Lerp(_endCol, _startCol, _imgTimer.fillAmount);

            if (_timer <= 0f) RestartPU();

        }
    }

    void MetodaRepeat()
    {
        IsActive = true;
    }
    void RestartPU()
    {
        CancelInvoke();
        Invoke(nameof(MetodaRepeat), 4f);
        IsActive = false;
    }
    void OnPickup(Faction fact)
    {
        RestartPU();

        if (fact != Faction.Ally) return;
        
        switch (pUtype)
        {
            case PUtype.Diamond:
                gm.uimanager.PointsMethod(100);
                break;
            case PUtype.Fuel:
                gm.playerControll.Refuel();
                break;
            case PUtype.Ammo:
                gm.playerControll.Ammo = 10;
                break;
        }
    }

    public void TakeDamage(Faction attackerFaction, int dam)
    {
        OnPickup(attackerFaction);
    }
}
