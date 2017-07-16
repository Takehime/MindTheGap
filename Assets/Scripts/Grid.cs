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

    void Start()
    {
        StartCoroutine(generateGrid());
    }

    IEnumerator generateGrid()
    {

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                int tile_id = x * width + y;
                spawnTile(tile_id);
                //spawnPassenger(tile_id);
            }
        }


        yield return new WaitForSeconds(2f);

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                int tile_id = x * width + y;
                //spawnTile(tile_id);
                spawnPassenger(tile_id);
            }
        }
    }

    void spawnTile(int tile_id)
    {
        GameObject go = Instantiate(tile_prefab);
        go.transform.SetParent(gameObject.transform.GetChild(0), false);
        go.GetComponent<Tile>().generateTile(tile_id);
        go.GetComponent<Text>().text = tile_id.ToString();
        //Debug.Log(go.transform.position);
        tiles.Add(go);
    }

    void spawnPassenger(int tile_id)
    {
        GameObject go = Instantiate(passenger_prefab);
        go.transform.SetParent(gameObject.transform.GetChild(1), false);
        go.GetComponent<Passenger>().generatePassenger(passenger_types, tile_id);
        go.transform.position = tiles[tile_id].transform.position;
        passengers.Add(go);
    }
}
