using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

	public delegate void InteractWithTile(int id);
	public event InteractWithTile moveToTile;

    private int id;

    public void generateTile(int tile_id)
    {
        id = tile_id;
        gameObject.name = "Tile " + tile_id;
    }

    public int getTileId()
    {
        return id;
    }

	public void _onMoveMode()
	{
		if (moveToTile != null)
		{
			moveToTile(id);
		}
	}
}
