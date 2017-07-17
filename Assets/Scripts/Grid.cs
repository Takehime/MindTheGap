using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour {
    
    public int width; 
    public int height;
    public GameObject tile_prefab;
    public GameObject passenger_prefab;
    public List<PassengerData> passenger_types;

    private List<GameObject> tiles = new List<GameObject>();
    private List<GameObject> passengers = new List<GameObject>();
    private List<int> types_counter = new List<int>();
    private int max_quantity;

    void Start()
    {
        max_quantity = width * height / passenger_types.Capacity;
        initializeCounterList();
        StartCoroutine(generateGrid());
    }

    void initializeCounterList()
    {
        for (int i = 0; i < passenger_types.Capacity; i++)
        {
            types_counter.Add(0);
        }
    }

    IEnumerator generateGrid()
    {

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                int tile_id = x * width + y;
                spawnTile(tile_id);
            }
        }

        yield return new WaitForEndOfFrame();
        
        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                int tile_id = x * width + y;
                spawnPassenger(tile_id);
            }
        }
    }

    void spawnTile(int tile_id)
    {
        GameObject go = Instantiate(tile_prefab);
        go.transform.SetParent(gameObject.transform.GetChild(0), false);
        go.GetComponent<Tile>().generateTile(tile_id);
        tiles.Add(go);
    }

    void spawnPassenger(int tile_id)
    {
        GameObject go = Instantiate(passenger_prefab);
        go.transform.SetParent(gameObject.transform.GetChild(1), false);
        int chosen = 0;
        do {
            chosen = go.GetComponent<Passenger>().generatePassenger(passenger_types, tile_id);
            types_counter[chosen]++;
        } while (types_counter[chosen] > max_quantity);
        go.transform.position = tiles[tile_id].transform.position;
        passengers.Add(go);
    }

}
