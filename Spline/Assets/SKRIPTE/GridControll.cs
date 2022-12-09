using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;

public class GridControll : MonoBehaviour
{
    GameManager gm;
    public Tile[,] tiles;
    [HideInInspector] public Vector2Int dimenzije;
    Tile _neighbourTile;
    Transform _hitTileTransform;
    Vector2 _razlika;
    float dotX, dotY;
    public bool isRotating;

    private void Awake()
    {
        gm = GameManager.gm;
        gm.gridControll = this;
    }
    private void Start()
    {
        tiles = new Tile[dimenzije.x, dimenzije.y];
        for (int i = 0; i < transform.childCount; i++)
        {
            Tile tl = transform.GetChild(i).GetComponent<Tile>();
            tiles[tl.pozicija.x, tl.pozicija.y] = tl;
        }
    }



    public void BulletHitTile(Faction fact, Transform bulletPoz, Vector2Int tilePoz)
    {
        switch (fact)
        {
            case Faction.Ally:
                _hitTileTransform = tiles[tilePoz.x, tilePoz.y].transform;
                _razlika = ((Vector2)bulletPoz.position - (Vector2)_hitTileTransform.position).normalized;
                dotX = Vector2.Dot(_hitTileTransform.right, _razlika);
                dotY = Vector2.Dot(_hitTileTransform.up, _razlika);
                if (Mathf.Abs(dotX) > Mathf.Abs(dotY))
                {
                    _neighbourTile = tiles[tilePoz.x + (dotX > 0f ? 1 : -1), tilePoz.y];
                }
                else
                {
                    _neighbourTile = tiles[tilePoz.x, tilePoz.y + (dotY > 0f ? 1 : -1)];
                }

                if (_neighbourTile.CurrentState == TileState.Danger)
                {
                    _neighbourTile.WrongTile();
                    EventManager.LevelDoneWin?.Invoke("Wrong tile!", gm.postavke.level, false);
                    return;
                }

                _neighbourTile.CurrentState = TileState.Closed;

                CheckGameOverPlayerAction();
                break;

            case Faction.Enemy:
                tiles[tilePoz.x, tilePoz.y].CurrentState = TileState.Open;

                //provjera level lost
                for (int i = 0; i < dimenzije.x; i++)
                {
                    for (int j = 0; j < dimenzije.y; j++)
                    {
                        if (tiles[i, j].CurrentState == TileState.Closed)
                        {
                            return;
                        }
                    }
                }
                EventManager.LevelDoneWin?.Invoke("No more tiles, game over!", gm.postavke.level, false);
                break;
        }
    }
    void CheckGameOverPlayerAction()
    {
        bool gameWon = true; //all tiles are closed
        for (int i = 0; i < dimenzije.x; i++)
        {
            for (int j = 0; j < dimenzije.y; j++)
            {
                if (tiles[i, j].CurrentState == TileState.Open)
                {
                    gameWon = false;
                    break;
                }
            }
        }
        if (gameWon)
        {
            EventManager.LevelDoneWin?.Invoke("Level completed!", gm.postavke.level, true);
            return;
        }

        if (!isRotating && gm.levelManager.enemyWithCannonsCount <= 0) gm.closedAreas.EnclosureCheck();


    }
}
