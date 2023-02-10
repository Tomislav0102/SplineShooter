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
    int _counterFindNeighbour;

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
        _counterFindNeighbour = 0;
        switch (fact)
        {
            case Faction.Ally:
                _hitTileTransform = tiles[tilePoz.x, tilePoz.y].transform;
                _razlika = ((Vector2)bulletPoz.position - (Vector2)_hitTileTransform.position).normalized;
                dotX = Vector2.Dot(_hitTileTransform.right, _razlika);
                dotY = Vector2.Dot(_hitTileTransform.up, _razlika);
                if (Mathf.Abs(dotX) > Mathf.Abs(dotY))
                {
                    _neighbourTile = FindNeighbourX(tilePoz);
                }
                else
                {
                    _neighbourTile = FindNeighbourY(tilePoz);
                }

                if (_neighbourTile.CurrentState == TileState.Danger)
                {
                    _neighbourTile.WrongTile();
                    EventManager.LevelDoneWin?.Invoke("Wrong tile!", false);
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
                EventManager.LevelDoneWin?.Invoke("No more tiles, game over!", false);
                break;
        }
    }

    private Tile FindNeighbourX(Vector2Int tilePoz)
    {
        _counterFindNeighbour++;
        if (_counterFindNeighbour > 5) return tiles[tilePoz.x, tilePoz.y];

        Tile tl = tiles[tilePoz.x + (dotX > 0f ? 1 : -1), tilePoz.y];
        if (tl.CurrentState == TileState.Closed) return FindNeighbourY(tilePoz);
        return tl;
    }
    private Tile FindNeighbourY(Vector2Int tilePoz)
    {
        _counterFindNeighbour++;
        if (_counterFindNeighbour > 5) return tiles[tilePoz.x, tilePoz.y];

        Tile tl = tiles[tilePoz.x, tilePoz.y + (dotY > 0f ? 1 : -1)];
        if (tl.CurrentState == TileState.Closed) return FindNeighbourX(tilePoz);
        return tl;
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
            EventManager.LevelDoneWin?.Invoke("Level completed!", true);
            return;
        }

        if (!isRotating && !gm.levelManager.HasEnemiesWithCannons()) gm.closedAreas.EnclosureCheck();


    }
}
