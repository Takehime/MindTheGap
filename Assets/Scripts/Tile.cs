using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    private int id;

    public void generateTile(int tile_id)
    {
        id = tile_id;
        gameObject.name = "Tile " + tile_id;
    }
}
