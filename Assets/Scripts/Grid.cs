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
    //[HideInInspector]
    public GameObject[] passengers;
    [HideInInspector]
    public bool scan_mode_active = false;
    [HideInInspector]
    public bool pachinko_mode_active = false;

    private int width = 10; 
    private int height = 6;
	public int door_id1 = 54;
	public int door_id2 = 55;
	public bool ending_event = false;
    public Dictionary<PassengerType, int> types_counter = new Dictionary<PassengerType, int>();
    private List<GameObject> player_adj;
    private TurnManager.Turn curr_turn = TurnManager.Turn.BetweenStations;
    private bool swap_mode_active = false;
    private int max_quantity;
    private Scan scan;
    private Pachinko pachinko;
    private Coroutine swapMode;
    private TurnManager tm;
    private bool alreadySwaped = false;

    #region initialization

    void Start()
    {
		passengers = new GameObject[width*height];
        //print("passengers.count: " + passengers.Length);
        scan = FindObjectOfType<Scan>();
        pachinko = FindObjectOfType<Pachinko>();
        tm = FindObjectOfType<TurnManager>();
        FindObjectOfType<TurnManager>().changeTurn += changeTurn;
        max_quantity = width * height / passenger_types.Capacity;
        initializeCounterList();
        StartCoroutine(generateGrid());
    }

    void initializeCounterList()
    {
        for (int i = 0; i < passenger_types.Capacity; i++)
        {
            PassengerData p_data = passenger_types[i];
            PassengerType p_type = p_data.type;
            types_counter.Add(p_type, 0);
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

				subscribeTileForMovement (tiles[tile_id]);
            }
        }
        
    }

    public void spawnPassenger(int tile_id)
    {
        GameObject go = Instantiate(passenger_prefab);
        go.transform.SetParent(gameObject.transform.GetChild(1), false);
        PassengerType p_type;
        
		int desistencia = 0;
		do
        {
            p_type = go.GetComponent<Passenger>().generatePassenger(passenger_types, tile_id);
		} while (types_counter[p_type] > max_quantity-1 && desistencia++ < 200);

        types_counter[p_type]++;
        //print("P_type: " + p_type + ", count : " + types_counter[p_type]);
        go.transform.position = tiles[tile_id].transform.position;
        passengers[tile_id] = go;
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
        passengers[tile_id] = go;
        go.GetComponent<Player>().swapMode += onSwapMode;
        tiles[tile_id].GetComponent<Image>().color = new Color32(226, 157, 82, 255);
    }

    void subscribePassengerForSwap(GameObject p)
    {
        p.GetComponent<Passenger>().swapTarget += checkIfPassengerCanSwap;
    }

    void subscribePassengerForScan(GameObject p)
    {
        p.GetComponent<Passenger>().scanTarget += scan.scanPassenger;
    }

	void subscribeTileForMovement(GameObject t)
	{
		t.GetComponent<Tile>().moveToTile += checkIfPlayerCanMoveToTile;
	}

    #endregion

    #region swap mode
    void onSwapMode(int tile_id, bool activate)
    {
        if (!scan_mode_active)
        {
			if (activate)
				enterSwapMode (tile_id);
            else leaveSwapMode(tile_id, tile_id);
        }
    }

    void enterSwapMode(int tile_id)
    {
        swap_mode_active = true;
        tiles[tile_id].GetComponent<Image>().color = new Color32(242, 95, 78, 255);
        player_adj = calculateAdj(tile_id);
        //printList(player_adj);

		for (int i = 0; i < player_adj.Count; i++) {
			GameObject go = player_adj [i];
			Tile tile = go.GetComponent<Tile> ();
			int index = tile.getTileId ();
			if (passengers [index] == null) {
				return;
			}
		}
        changePassengersAlpha(player_adj, true);
    }

    void leaveSwapMode(int old_tile_id, int new_tile_id )
    {
        swap_mode_active = false;
        tiles[old_tile_id].GetComponent<Image>().color = new Color32(195, 213, 255, 255);
        tiles[new_tile_id].GetComponent<Image>().color = new Color32(226, 157, 82, 255);
        player_adj = calculateAdj(new_tile_id);

		for (int i = 0; i < player_adj.Count; i++) {
			GameObject go = player_adj [i];
			Tile tile = go.GetComponent<Tile> ();
			int index = tile.getTileId ();
			if (passengers [index] == null) {
				return;
			}
		}
		changePassengersAlpha(player_adj, false);
    }

    public List<GameObject> calculateAdj(int id)
    {
        List<GameObject> adj_list = new List<GameObject>();
        if (FindObjectOfType<Player>().getTileId() == id)
            adj_list.Add(tiles[id]); //player eh adj a ele mesmo

		List<int> assentos = getAllSeats(); 
		if (assentos.Contains (id)) {
			if (AtStation.passengerOnFirstLineOfSeats(id)) {
				adj_list.Add(tiles[id + width]);
			} else if (AtStation.passengerOnSecondLineOfSeats(id)) {
				adj_list.Add(tiles[id - width]);

				if (id + width == getPlayerID ()) {
					adj_list.Add (tiles [id + width * 2]);
				} else {
					adj_list.Add (tiles [id + width]);
				}
			} 

			if (AtStation.passengerOnFirstLastLineOfSeats(id)) {
				adj_list.Add (tiles[id - width]);
				adj_list.Add (tiles[id + width]);

				if (id - width == getPlayerID ()) {
					adj_list.Add (tiles [id - width * 2]);
				} else {
					adj_list.Add (tiles [id - width]);
				}
			} else if (AtStation.passengerOnSecondLastLineOfSeats(id)) {
				adj_list.Add (tiles[id - width]);
			}
			return adj_list;
		} 
		if (id % width == 0 || id == 44 || id == 54)
			adj_list.Add (tiles [id + 1]);
		else if (id % width == width - 1 || id == 45 || id == 55)
			adj_list.Add (tiles [id - 1]);
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
        return passengers[tile_id] == null;
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

    void checkIfPassengerCanSwap(int tile_id, PassengerType p_type)
    {
        if (swap_mode_active && player_adj.Contains(tiles[tile_id]))
        {
            pachinko.enterPachinkoMode(passengers[tile_id].GetComponent<Image>().color);
            pachinko_mode_active = true;
			int player_id = FindObjectOfType<Player>().getTileId();
			swapMode = StartCoroutine(startSwapMode(player_id, tile_id, 0.25f));
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
		//Debug.Log ("movi o passageiro de id = " + id);
        GameObject pass = passengers[id];
        GameObject target = tiles[target_id];
        if (pass != null)
        {
            pass.transform.DOMove(target.transform.position, duration);
			Passenger p = pass.GetComponent<Passenger> ();
			if (p != null) {
				p.setTileId (target_id);
			} else {
				pass.GetComponent<Player> ().setTileId (target_id);
			}
            passengers[target_id] = pass;
            passengers[id] = null;
        }
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

        //inicia o turno
        /*if (!alreadySwaped) {
            tm.setTurnToBetweenStations();
            alreadySwaped = true;
        }*/
    }
    #endregion

    #region turn management

    public void changeTurn(TurnManager.Turn next_turn)
    {
        curr_turn = next_turn;
        if (curr_turn == TurnManager.Turn.AtStation) {
            cancelSwap(new Color32(195, 213, 255, 255));
        } else {
            cancelSwap(new Color32(226, 157, 82, 255));
        }
        cancelPachinko();
    }

    void cancelSwap(Color color)
    {
        FindObjectOfType<Player>().setSwapMode(false);
        int temp = FindObjectOfType<Player>().getTileId();
        tiles[temp].GetComponent<Image>().color = color;
        player_adj = calculateAdj(temp);
        swap_mode_active = false;

		for (int i = 0; i < player_adj.Count; i++) {
			//print ("passengers [player_adj [i]]: " + passengers [player_adj [i]]);
			GameObject go = player_adj [i];
			Tile tile = go.GetComponent<Tile> ();
			int index = tile.getTileId ();
			if (passengers [index] == null) {
				FindObjectOfType<Player>().changeTurn(TurnManager.Turn.BetweenStations);
				return;
			}
		}
		changePassengersAlpha (player_adj, false);
    }

    void cancelPachinko() {
        pachinko.leavePachinkoMode(false);
        if (swapMode != null) {
            StopCoroutine(swapMode);
        }
    }

    #endregion

	#region ending 

	void checkIfPlayerCanMoveToTile(int tile_id) {
		int player_id = getPlayerID ();
		var adj = player_adj;

		if (swap_mode_active && ending_event) {
			if (player_id >= 20 && player_id <= 29) {
				adj.Add (tiles[player_id - width]);
			}
			if (player_id >= 30 && player_id <= 39 && player_id != 34 && player_id != 35) {
				adj.Add (tiles[player_id + width]);
			}
		}

		if (swap_mode_active && ending_event && player_adj.Contains (tiles [tile_id]))
		{
			movePassenger (player_id, tile_id, 0.25f);
			if (AtStation.passengerOnSecondLineOfSeats (tile_id)
			    || AtStation.passengerOnFirstLastLineOfSeats (tile_id)) {
				End_Game ();
			}
		}
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

    public List<int> getAllNotSeats() {
        List<int> not_seats = new List<int>();
        for (int id = 0; id < 60; id++) {
            if ((id >= 20 && id < 40)
                || (id >= 44 && id < 46)
                || (id >= 54 && id < 56)) {
                if (id != getPlayerID()) {
                    not_seats.Add(id);
                }
            }
        }
        return not_seats;
    }

    public IDPosFromDoor posFromDoor(int id) {
        //if (id % 10 < door_id1 % 10)
        //    return IDPosFromDoor.LEFT;
        //else if (id % 10 > door_id2 % 10)
        //    return IDPosFromDoor.RIGHT;
        //else if (id % 10 == door_id1 % 10) {
        //    if (getPlayerID() % 10 == door_id1 % 10) {
        //        return IDPosFromDoor.RIGHT;
        //    } else {
        //        return IDPosFromDoor.MID;
        //    }
        //} else if (id % 10 == door_id2 % 10) {
        //    if (getPlayerID() % 10 == door_id2 % 10) {
        //        return IDPosFromDoor.LEFT;
        //    } else {
        //        return IDPosFromDoor.MID;
        //    }
        //} else //if (id == door_id1 || id == door_id2)
        //    return IDPosFromDoor.ON_DOOR;

        //=========

		if (id == door_id1 || id == door_id2)
			return IDPosFromDoor.ON_DOOR;
        else if (id % 10 < door_id1 % 10)
            return IDPosFromDoor.LEFT;
        else if (id % 10 > door_id2 % 10)
            return IDPosFromDoor.RIGHT;
        else
            return IDPosFromDoor.MID;
    }

    public void printTypesCounter()
    {
        foreach (KeyValuePair<PassengerType, int> d in types_counter) {
            print("Type = {" + d.Key + "}, Count = {" + d.Value + "}");
        }
    }

    public int getPlayerID() {
        return FindObjectOfType<Player>().getTileId();
    }

	#endregion

	/*public Passenger Get_Passenger_By_TileID(int tile_id) {
		for (int i = 0; i < passengers.Count; i++) {
			if (passengers[i].tile_id)
		}
	}*/

	public void End_Game() {
		print("vamos embora");
	}
}
