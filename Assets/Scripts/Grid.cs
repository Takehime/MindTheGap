﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Grid : MonoBehaviour {
    
    public int width; 
    public int height;
    public int player_spawn_point;
    public GameObject tile_prefab;
    public GameObject passenger_prefab;
    public GameObject player_prefab;
    public GameObject father_tile;
    public List<PassengerData> passenger_types;

    private List<GameObject> tiles = new List<GameObject>();
    private List<GameObject> passengers = new List<GameObject>();
    private List<int> types_counter = new List<int>();
    private int max_quantity;
    private bool swap_mode_active = false;
    List<GameObject> player_adj;

    #region initialization
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
    #endregion

    #region grid generation
    IEnumerator generateGrid()
    {
        for (int i = 0; i < width * height; i++)
        {
            GameObject child_tile = father_tile.transform.GetChild(i).gameObject;
            child_tile.GetComponent<Tile>().generateTile(i);
            tiles.Add(child_tile);
        }

        yield return new WaitForEndOfFrame();

        for (int x = 0; x < height; x++)
        {
            for (int y = 0; y < width; y++)
            {
                int tile_id = x * width + y;
                if (tile_id == player_spawn_point)
                    spawnPlayer(tile_id);
                else spawnPassenger(tile_id);
            }
        }
    }

    void spawnPassenger(int tile_id)
    {
        GameObject go = Instantiate(passenger_prefab);
        go.transform.SetParent(gameObject.transform.GetChild(1), false);
        int chosen = 0;
        do
        {
            chosen = go.GetComponent<Passenger>().generatePassenger(passenger_types, tile_id);
            types_counter[chosen]++;
        } while (types_counter[chosen] > max_quantity);
        go.transform.position = tiles[tile_id].transform.position;
        passengers.Add(go);
        go.GetComponent<Passenger>().swapTarget += checkIfCanSwap;
    }

    void spawnPlayer(int tile_id)
    {
        GameObject go = Instantiate(player_prefab);
        go.transform.SetParent(gameObject.transform.GetChild(1), false);
        go.GetComponent<Player>().generatePlayer(tile_id);
        go.transform.position = tiles[tile_id].transform.position;
        go.gameObject.name = "Player";
        passengers.Add(go);
        go.GetComponent<Player>().swapMode += onSwapMode;
    }
    #endregion

    #region swap mode
    void checkIfCanSwap(int tile_id)
    {
        if (swap_mode_active && player_adj.Contains(tiles[tile_id]))
            swap(tile_id, 0.25f);
    }

    void swap(int target_id, float duration)
    {
        GameObject player = FindObjectOfType<Player>().gameObject;
        GameObject target = passengers[target_id];
        int player_id = player.GetComponent<Player>().getTileId();

        //animaçao
        target.transform.DOMove(player.transform.position, duration);
        player.transform.DOMove(target.transform.position, duration);
        
        //troca do curr_tile_id
        target.GetComponent<Passenger>().setTileId(player_id);
        player.GetComponent<Player>().setTileId(target_id);

        //troca dos objetos na lista de passageiros
        passengers[player_id] = target;
        passengers[target_id] = player;

        //desliga o swap_mode
        player.GetComponent<Player>().setSwapMode(false);

        //atualiza lista de adjacencias e reseta cor dos tiles
        leaveSwapMode(player_id, target_id);
    }

    void onSwapMode(int tile_id, bool activate)
    {
        //FindObjectOfType<Player>().swapMode -= onSwapMode;
        if (activate)
            enterSwapMode(tile_id);
        else leaveSwapMode(tile_id, tile_id);
    }

    void enterSwapMode(int tile_id)
    {
        swap_mode_active = true;
        tiles[tile_id].GetComponent<Image>().color = new Color32(226, 157, 82, 255);
        player_adj = calculateAdj(tile_id);
        changeAlfa(player_adj, true);
    }

    void leaveSwapMode(int old_tile_id, int new_tile_id )
    {
        swap_mode_active = false;
        tiles[old_tile_id].GetComponent<Image>().color = new Color32(195, 213, 255, 255);
        changeAlfa(player_adj, false);
        player_adj = calculateAdj(new_tile_id);
    }

    List<GameObject> calculateAdj(int id)
    {
        List<GameObject> adj_list = new List<GameObject>();
        adj_list.Add(tiles[id]); //player eh adj a ele mesmo
        if (id % width == 0 || id == 44 || id == 54)
            adj_list.Add(tiles[id + 1]);
        else if (id % width == width - 1 || id == 45 || id == 55)
            adj_list.Add(tiles[id - 1]);
        else {
            adj_list.Add(tiles[id - 1]);
            adj_list.Add(tiles[id + 1]);
        }
        if (id < width)
            adj_list.Add(tiles[id + width]);
        else if (id >= width * height - width)
            adj_list.Add(tiles[id - width]);
        else {
            adj_list.Add(tiles[id - width]);
            adj_list.Add(tiles[id + width]);
        }
        return adj_list;
    }

    void changeAlfa(List<GameObject> adj_list, bool reduce_alpha)
    {
        for (int i = 0; i < tiles.Count; i ++)
        {
            if (!adj_list.Contains(tiles[i]))
            {
                if (reduce_alpha)
                    passengers[i].GetComponent<Image>().color -= new Color32(0, 0, 0, 155);
                else
                    passengers[i].GetComponent<Image>().color += new Color32(0, 0, 0, 155);
            }
        }
    }
    #endregion

    #region utility
    void printList<T>(List<T> lista)
    {
        string temp = "{ ";
        foreach(T elem in lista)
        {
            temp = temp + elem + " ";
        }
        temp = temp + "}";
        Debug.Log(temp);
    }
    #endregion

}
