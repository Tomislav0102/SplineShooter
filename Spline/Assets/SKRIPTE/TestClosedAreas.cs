using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;


public class TestClosedAreas : MonoBehaviour
{
    GameManager gm;
    Camera cam;
    Tile tile;
    
    LayerMask layerMask;
    Ray ray;
    RaycastHit hit;
    Ray2D ray2D;
    RaycastHit2D hit2D;
    Vector2 mousePoz;
    Vector2 _tilePoz, _tileSize;
    BoxCollider2D _box2D;

    private void Awake()
    {
        gm = GameManager.gm;
        tile = GetComponent<Tile>();
        layerMask = LayerMask.GetMask("Grid");
        cam = gm.mainCam;
        _box2D = tile.GetComponent<BoxCollider2D>();
    }
    private void Start()
    {
        _tilePoz = new Vector2(tile.transform.position.x, tile.transform.position.y);
        _tileSize = new Vector2(tile.transform.localScale.x, tile.transform.localScale.y);
    }

    private void Update()
    {
        mousePoz = cam.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D hit2d = Physics2D.OverlapCircle(mousePoz, 0.02f);
            if (hit2d != null && hit2d == _box2D)
            {
                if (tile.CurrentState == TileState.Open) tile.CurrentState = TileState.Closed;
                else if (tile.CurrentState == TileState.Closed) tile.CurrentState = TileState.Open;
                gm.closedAreas.EnclosureCheck();
            } 
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(mousePoz, 0.2f);
    }
}
