using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Grid : MonoBehaviour {

    [Header("Variables")]
    [Range(54, 55)]
    public int player_spawn_point;
    [Header("Prefabs")]
    public GameObject tile_prefab;
    public GameObject passenger_prefab;
    public GameObject player_prefab;
    public GameObject father_tile;
    [Header("List of passenger types")]
    public List<PassengerData> passenger_types;

    [HideInInspector]
    public List<GameObject> tiles = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> passengers = new List<GameObject>();
    [HideInInspector]
    public bool scan_mode_active = false;
    [HideInInspector]
    public bool pachinko_mode_active = false;

    private int width = 10; 
    private int height = 6;
    private List<int> types_counter = new List<int>();
    private List<GameObject> player_adj;
    private TurnManager.Turn curr_turn = TurnManager.Turn.BetweenStations;
    private bool swap_mode_active = false;
    private int max_quantity;
    private Scan scan;
    private Pachinko pachinko;

    #region initialization

    void Start()
    {
        scan = FindObjectOfType<Scan>();
        pachinko = FindObjectOfType<Pachinko>();
        FindObjectOfType<TurnManager>().changeTurn += changeTurn;
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
        subscribePassengerForSwap(go);
        subscribePassengerForScan(go);
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

    void subscribePassengerForSwap(GameObject p)
    {
        p.GetComponent<Passenger>().swapTarget += checkIfPassengerCanSwap;
    }

    void subscribePassengerForScan(GameObject p)
    {
        p.GetComponent<Passenger>().scanTarget += scan.scanPassenger;
    }

    #endregion

    #region swap mode
    void onSwapMode(int tile_id, bool activate)
    {
        if (!scan_mode_active)
        {
            if (activate)
                enterSwapMode(tile_id);
            else leaveSwapMode(tile_id, tile_id);
        }
    }

    void enterSwapMode(int tile_id)
    {
        swap_mode_active = true;
        tiles[tile_id].GetComponent<Image>().color = new Color32(226, 157, 82, 255);
        player_adj = calculateAdj(tile_id);
        Debug.Log("player_id" + tile_id);
        printList(player_adj);
        changePassengersAlpha(player_adj, true);
    }

    void leaveSwapMode(int old_tile_id, int new_tile_id )
    {
        swap_mode_active = false;
        tiles[old_tile_id].GetComponent<Image>().color = new Color32(195, 213, 255, 255);
        changePassengersAlpha(player_adj, false);
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
        if (id < width || (id >= 20 && id < 30))
            adj_list.Add(tiles[id + width]);
        else if (id == 54 || id == 55 || (id >=30 && id < 34) || (id >= 36 && id < 40))
            adj_list.Add(tiles[id - width]);
        else {
            adj_list.Add(tiles[id - width]);
            adj_list.Add(tiles[id + width]);
        }
        return adj_list;
    }

    void changePassengersAlpha(List<GameObject> adj_list, bool reduce_alpha)
    {
        for (int i = 0; i < tiles.Count; i ++)
        {
            if (adj_list.Count != 0 && !adj_list.Contains(tiles[i]))
            {
                var color = passengers[i].GetComponent<Image>().color;
                passengers[i].GetComponent<Image>().color = new Color(
                    color.r,
                    color.g,
                    color.b,
                    reduce_alpha ? 0.5f : 1
                );
            }
        }
    }

    void checkIfPassengerCanSwap(int tile_id, PassengerData.PassengerType p_type)
    {
        if (swap_mode_active && player_adj.Contains(tiles[tile_id]))
        {
            pachinko.enterPachinkoMode(passengers[tile_id].GetComponent<Image>().color);
            pachinko_mode_active = true;
            StartCoroutine(swapPassengerWithPlayer(tile_id, 0.25f));
        }
    }

    IEnumerator waitForEndOfPachinkoMode()
    {
        yield return new WaitUntil(() => pachinko_mode_active);
        yield return new WaitWhile(() => pachinko.pachinko_go.activeSelf);
    }

    IEnumerator swapPassengerWithPlayer(int target_id, float duration)
    {
        yield return waitForEndOfPachinkoMode();

        GameObject player = FindObjectOfType<Player>().gameObject;
        int player_id = player.GetComponent<Player>().getTileId();
        GameObject target = passengers[target_id];

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
    #endregion

    #region turn management

    void changeTurn(TurnManager.Turn next_turn)
    {
        curr_turn = next_turn;
        cancelSwap();
    }

    void cancelSwap()
    {
        FindObjectOfType<Player>().setSwapMode(false);
        int temp = FindObjectOfType<Player>().getTileId();
        tiles[temp].GetComponent<Image>().color = new Color32(195, 213, 255, 255);
        player_adj = calculateAdj(temp);
        changePassengersAlpha(player_adj, false);
    }

    #endregion

    #region utility
    public static void printList<T>(List<T> lista)
    {
        string temp = "{ ";
        foreach(T elem in lista)
        {
            temp = temp + elem + ", ";
        }
        temp = temp + "}";
        Debug.Log(temp);
    }
    #endregion

}
