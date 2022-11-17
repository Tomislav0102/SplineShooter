using FirstCollection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AstarManager : MonoBehaviour
{
    [SerializeField] Transform parTiles;
    readonly Vector2Int gridDim = new Vector2Int(10, 7);
    Tile[,] _allTiles;
    List<Tile> _openTiles = new List<Tile>();
    public class Grupa
    {
        public HashSet<Tile> tiles = new HashSet<Tile>();
        bool _hasExit;
        public bool HasExit
        {
            get => _hasExit;
            set
            {
                _hasExit = value;
                foreach (Tile item in tiles)
                {
                    item.HasExit = _hasExit;
                }
            }
        }
    }
    List<Grupa> _sveGrupe = new List<Grupa>();

    private void Awake()
    {
        _allTiles = new Tile[gridDim.x, gridDim.y];
        int count = parTiles.childCount;
        for (int i = 0; i < count; i++)
        {
            int xVal = int.Parse(parTiles.GetChild(i).name[5].ToString());
            int yVal = int.Parse(parTiles.GetChild(i).name[7].ToString());
            _allTiles[xVal, yVal] = parTiles.GetChild(i).GetComponent<Tile>();
            if (_allTiles[xVal, yVal].editorSetupTileState == TileState.Open) _openTiles.Add(_allTiles[xVal, yVal]);
        }

    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) RunAstar();
    }
    bool ValidNeighbour(Tile neigbourTile, int x, int y)
    {
        if (Mathf.Abs(x) != Mathf.Abs(y) && neigbourTile.CurrentState == TileState.Open) return true;
        return false;
    }
    bool ExitNeighbour(Tile neigbourTile, int x, int y)
    {
        if (Mathf.Abs(x) != Mathf.Abs(y) && neigbourTile.CurrentState == TileState.Danger) return true;
        return false;
    }
    public void RunAstar()
    {
        _sveGrupe.Add(new Grupa());
        _sveGrupe[0].tiles.Add(_openTiles[0]);

        for (int k = 0; k < _openTiles.Count; k++)
        {
            Tile tl = _openTiles[k];
            if (tl.SectorOrdinal == -1)
            {
                _sveGrupe.Add(new Grupa());
                _sveGrupe[_sveGrupe.Count - 1].tiles.Add(tl);
                AzurirajGrupu();
            }
            Rekurziva(tl);
        }

        CheckForExits();
    }
    void Rekurziva(Tile tl)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Tile neighbour = _allTiles[tl.pozicija.x + i, tl.pozicija.y + j];
                if (!ValidNeighbour(neighbour, i, j)) continue;
                if (neighbour.SectorOrdinal == -1)
                {
                    _sveGrupe[_sveGrupe.Count - 1].tiles.Add(neighbour);
                    AzurirajGrupu();
                    Rekurziva(neighbour);
                }
                else if (neighbour.SectorOrdinal != tl.SectorOrdinal)
                {
                    foreach (Tile item in _sveGrupe[neighbour.SectorOrdinal].tiles)
                    {
                        _sveGrupe[tl.SectorOrdinal].tiles.Add(item);
                    }
                    _sveGrupe.RemoveAt(neighbour.SectorOrdinal);
                    AzurirajGrupu();
                    Rekurziva(neighbour);
                }


            }
        }

    }
    void AzurirajGrupu()
    {
        for (int i = 0; i < _sveGrupe.Count; i++)
        {
            foreach (Tile item in _sveGrupe[i].tiles)
            {
                item.SectorOrdinal = i;
            }
        }
    }
    void CheckForExits()
    {
        for (int k = 0; k < _sveGrupe.Count; k++)
        {
            foreach (Tile item in _sveGrupe[k].tiles)
            {
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        Tile neighbour = _allTiles[item.pozicija.x + i, item.pozicija.y + j];
                        if (ExitNeighbour(neighbour, i, j))
                        {
                          //  print($"Sve grupe rb je {k}, a nighbour je {neighbour.pozicija}");
                            _sveGrupe[k].HasExit = true;
                        }
                    }
                }
            }

        }
    }
}
