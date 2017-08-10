using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum IDPosFromDoor{
	LEFT, MID, RIGHT, ON_DOOR
}

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
	private int door_id1 = 54;
	private int door_id2 = 55;
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

    public List<GameObject> calculateAdj(int id)
    {
        List<GameObject> adj_list = new List<GameObject>();

        if (FindObjectOfType<Player>().getTileId() == id)
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

    public bool tileIsEmpty(int tile_id)
    {
        for (int i = 0; i < passengers.Count; i++)
        {
            Passenger p = passengers[i].GetComponent<Passenger>();
            if (p != null)
            {
                int p_id = p.getTileId();
                if (p_id == tile_id)
                    return false;
            }
        }
        return true;
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
			int player_id = FindObjectOfType<Player>().getTileId();
			StartCoroutine(startSwapMode(player_id, tile_id, 0.25f));
        }
    }

	IEnumerator startSwapMode(int origin_id, int target_id, float duration) {
		yield return waitForEndOfPachinkoMode();
		swapTwoPassengers(origin_id, target_id, duration);
	}
	
    IEnumerator waitForEndOfPachinkoMode()
    {
        yield return new WaitUntil(() => pachinko_mode_active);
        yield return new WaitWhile(() => pachinko.pachinko_go.activeSelf);
    }

    public void movePassenger(int id, int target_id, float duration)
    {
        GameObject pass = passengers[id];
        GameObject target = tiles[target_id];
        pass.transform.DOMove(target.transform.position, duration);
        pass.GetComponent<Passenger>().setTileId(target_id);
        passengers[target_id] = pass;
    }

    public void swapTwoPassengers(int origin_id, int target_id, float duration)
    {
		GameObject origin = passengers [origin_id];
        GameObject target = passengers[target_id];

        //animaçao
        target.transform.DOMove(origin.transform.position, duration);
        origin.transform.DOMove(target.transform.position, duration);

        //troca do curr_tile_id
		if (target.GetComponent<Passenger>() != null) 
			target.GetComponent<Passenger>().setTileId(origin_id);

		//desliga o swap_mode se o origin for o player
		if (origin.GetComponent<Player>() != null) {
			origin.GetComponent<Player>().setTileId (target_id);
			origin.GetComponent<Player>().setSwapMode(false);
		} else {
			origin.GetComponent<Passenger>().setTileId (target_id);
		}

        //troca dos objetos na lista de passageiros
        passengers[origin_id] = target;
        passengers[target_id] = origin;
        
        //atualiza lista de adjacencias e reseta cor dos tiles se o origin for o player
		if (origin.GetComponent<Player> () != null) {
			leaveSwapMode (origin_id, target_id);
		}
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

    public bool tileIsSeat(int tile_id)
    {
        List<int> seats = getAllSeats();
        if (seats.Contains(tile_id))
            return true;
        else
            return false;
    }

    public List<int> getAllSeats()
    {
        List<int> seats = new List<int>();
        for (int id = 0; id < 60; id++)
        {
            if ((id >= 0 && id < 20)
                || (id >= 40 && id < 44)
                || (id >= 46 && id < 54)
                || (id >= 56 && id < 60))
            {
                seats.Add(id);
            }
        }
        return seats;
    }

	public IDPosFromDoor posFromDoor(int id) {
		if (id == door_id1 || id == door_id2)
			return IDPosFromDoor.ON_DOOR;
		else if (id % 10 < door_id1 % 10)
			return IDPosFromDoor.LEFT;
		else if (id % 10 > door_id2 % 10)
			return IDPosFromDoor.RIGHT;
		else
			return IDPosFromDoor.MID;
			
	}

	#endregion

}
