using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {
    
    public int width; 
    public int height;
    public GameObject tile_prefab;

    private List<GameObject> tiles = new List<GameObject>();
    private List<GameObject> passengers = new List<GameObject>();

    void Start()
    {
        generateGrid();
    }

    void generateGrid()
    {

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                generateTile(x, y);
                //generatePassenger(x, y);
            }
        }
    }

    void generateTile(int x, int y)
    {
        GameObject go = Instantiate(tile_prefab);
        go.transform.SetParent(gameObject.transform.GetChild(0), false);
        go.GetComponent<Tile>().generateTile(x * height + y);
        tiles.Add(go);
    }

    /*
    void generatePassenger(int x, int y)
    {
        GameObject go = Instantiate(tile_prefab);
        go.transform.SetParent(gameObject.transform, false);
        go.GetComponent<Tile>().generateTile(x * height + y);
        tiles.Add(go);
    }
    */
}
