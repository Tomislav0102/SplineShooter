using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirstCollection;

[ExecuteInEditMode]
public class EEM_tileDefinition : MonoBehaviour
{
    public Tile tile;
    public TileState tileState;
   // public bool pokreniEditMode;

    private void Update()
    {
        tile.CurrentState = tileState;
        tile.editorSetupTileState = tileState;
    }

}
