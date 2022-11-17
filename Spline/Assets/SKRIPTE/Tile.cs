using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;
using DG.Tweening;
using TMPro;

public class Tile : MonoBehaviour
{
    Transform _myTransform;
    [SerializeField] TextMeshPro displaySektor;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] BoxCollider2D _boxCollider2D;
    readonly Color prozirno = new Color(1f, 1f, 1f, 0.5f);
    readonly Color zatvoreno = new Color(0f, 1f, 0f, 1f);
    readonly Color pogresno = new Color(1f, 0f, 0f, 0f);
    /*[HideInInspector]*/ public TileState editorSetupTileState;
    TileState _tl;
    public TileState CurrentState
    {
        get => _tl;
        set
        {
            _boxCollider2D.enabled = false;
            _tl = value;
            switch (_tl)
            {
                case TileState.Open:
                    _spriteRenderer.color = prozirno;
                    break;
                case TileState.Closed:
                    _spriteRenderer.color = zatvoreno;
                    _boxCollider2D.enabled = true;
                    SectorOrdinal = -1;
                    break;
                case TileState.Danger:
                    _spriteRenderer.color = pogresno;
                    break;
            }
        }
    }
    [HideInInspector] public Vector2Int pozicija;
    int _sectorOrdinal = -1;
    public int SectorOrdinal //ordinal in a List<Sector>
    {
        get => _sectorOrdinal;
        set
        {
            _sectorOrdinal = value;
            displaySektor.text = _sectorOrdinal.ToString();
        }
    }
    bool _hasExit;
    public bool HasExit
    {
        get => _hasExit;
        set
        {
            _hasExit = value;
            displaySektor.color = _hasExit ? Color.blue : Color.red;
        }
    }
    private void Awake()
    {
        _myTransform = transform;
    }
    private void Start()
    {
        CurrentState = editorSetupTileState;
    }


    public void WrongTile()
    {
        _spriteRenderer.DOFade(1f, 0.2f)
            .OnComplete(EndTween);
    }
    void EndTween()
    {
        _spriteRenderer.DOFade(0f, 1f);
    }



}
