using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;

[ExecuteInEditMode]
public class EEM_generateGrid : MonoBehaviour
{
    [SerializeField] GridControll gr;
    [SerializeField] Tile prefabTile;
    public Tile[,] tiles;
    [SerializeField] float _tileDim;
    [SerializeField] Vector2Int dimenzije;



    public void CreateGrid()
    {
        tiles = new Tile[dimenzije.x, dimenzije.y];
        for (int i = 0; i < dimenzije.x; i++)
        {
            for (int j = 0; j < dimenzije.y; j++)
            {
                tiles[i, j] = Instantiate(prefabTile, new Vector3(transform.position.x + (i + 0.5f) * _tileDim, transform.position.y + (j + 0.5f) * _tileDim, 0), Quaternion.identity, transform);
                tiles[i, j].transform.localScale *= _tileDim;
                tiles[i, j].pozicija = new Vector2Int(i, j);
                tiles[i, j].name = "Tile " + i + " " + j;
                // if (i == 0 || i == dimenzije.x - 1 || j == 0 || j == dimenzije.y - 1) tiles[i, j].CurrentState = TileState.Danger;
            }
        }

        transform.position = new Vector3(-dimenzije.x * 0.5f * _tileDim, -dimenzije.y * 0.5f * _tileDim, 0f);
        gr.dimenzije = dimenzije;

    }

    public void DestroyGrid()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }

    }
}
