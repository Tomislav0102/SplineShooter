using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;

public class GridControll : MonoBehaviour
{
    GameManager gm;
    public Tile[,] tiles;
    [HideInInspector] public Vector2Int dimenzije;
    [SerializeField] bool isRotated45;
    // public int[,] grid;
    Vector2 _razlika;
    Tile _neighbourTile;


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
                if (isRotated45)
                {
                    bulletPoz.parent = transform;
                    _razlika = bulletPoz.localPosition - tiles[tilePoz.x, tilePoz.y].transform.localPosition;
                }
                else
                {
                    _razlika = (Vector2)bulletPoz.position - (Vector2)tiles[tilePoz.x, tilePoz.y].transform.position;
                }

                if (Mathf.Abs(_razlika.x) > Mathf.Abs(_razlika.y))
                {
                    _neighbourTile = tiles[tilePoz.x + (_razlika.x > 0f ? 1 : -1), tilePoz.y];
                }
                else
                {
                    _neighbourTile = tiles[tilePoz.x, tilePoz.y + (_razlika.y > 0f ? 1 : -1)];
                }

                if (_neighbourTile.CurrentState == TileState.Danger)
                {
                    _neighbourTile.WrongTile();
                    EventManager.LevelDoneWin("Wrong tile!", gm.postavke.level, false);
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
                EventManager.LevelDoneWin("No more tiles, game over!", gm.postavke.level, false);

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
            EventManager.LevelDoneWin("Level completed!", gm.postavke.level, true);
            return;
        }

       // if (gm.levelManager.enemyWithCannonsCount > 0) return; ovdje je bug, ispraviti
        gm.closedAreas.EnclosureCheck();

    }
}
