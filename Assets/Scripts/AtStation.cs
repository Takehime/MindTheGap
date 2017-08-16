using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum Direction {
	RIGHT, LEFT, UP, DOWN
}

public class AtStation : MonoBehaviour {

	public float swap_duration;
    public float adtional_time_between_phases;


    private int max_of_leavers = 2; // 9 = (1/4) dos sentados
	private Grid grid;
	private TurnManager tm;
    private List<Leaver> leavers;
	private List<Leaver> enterers;
    private bool player_up;
	private bool player_left;
    private int n_leavers;
    private int added;
    private bool ready_to_advance = false;

	class Leaver {
		private int id;
		private IDPosFromDoor pos;

		public Leaver(int id) {
			this.id = id;
		}
        public void setID(int id) {
			this.id = id;
		} 
		public void setPos(IDPosFromDoor pos) {
			this.pos = pos;
		}
		public int getID() {
			return this.id;
		}
		public IDPosFromDoor getPos() {
			return this.pos;
		}
	}

	void Start () {
		grid = FindObjectOfType<Grid>();
		tm = FindObjectOfType<TurnManager>();
        FindObjectOfType<TurnManager>().startStationLeaving += startStationLeaving;
	}

	#region main loop

	void startStationLeaving()
	{
		StartCoroutine (stationLeavingCoroutine());
	}

	IEnumerator stationLeavingCoroutine() {
        float time_stop_mov_anim = 1f;
        yield return new WaitForSeconds(time_stop_mov_anim);

        //phase 1 : sentados vao pro corredor

        //print("etrei no passo 1, vou selecionar os seats");
        //Debug.Break();

        selectSeats();

        //print("vou esperar por wait for advance");
        //Debug.Break();

        yield return waitForReadyForAdvance();

        //phase 2 : leavers saem do onibus

        //print("entrei no passo 2, vou printar a lista de leavers");
        //Debug.Break();

        printLeaversList();

        //print("vou entrar no for que chama setAllLeavers");
        //Debug.Break();

        for (int i = 0; i < leavers.Count; i++)
            setAllLeavers(leavers[i]);

        //print("vou printar os leavers de novo");
        //Debug.Break();

        printLeaversList();
        StartCoroutine(leavingLoop());
        yield return waitForReadyForAdvance();

        //phase 3 : novos passageiros entram no onibus
        resetLeaversList();
        StartCoroutine(enteringLoop());
        yield return waitForReadyForAdvance();
        resetEnterersList();

        //muda o turno
        tm.setTurnToBetweenStations();
    }

    IEnumerator waitForReadyForAdvance()
    {
        //print("1!!!ready_for_advance: " + ready_to_advance);
        yield return new WaitUntil(() => ready_to_advance);
        //print("2!!!ready_for_advance: " + ready_to_advance);
        yield return new WaitForSeconds(adtional_time_between_phases);
        ready_to_advance = false;
        //print("3!!!ready_for_advance: " + ready_to_advance);

    }

    #endregion

    #region passo 1
    void selectSeats() {
		List<int> leavers = new List<int>();
        for (int i = 0; i < max_of_leavers; i++) {
            int selected;
            do {
                List<int> seats = grid.getAllSeats();
                int index = Random.Range(0, grid.getAllSeats().Count);
                selected = seats[index];
                if (checkIfTwoPassengersAreOnSameXPos(selected, getIDPlayer())) {
                    //Debug.Log("id player: " + getIDPlayer() + " id passenger: " + selected + " (mesmo X)");
                }
            }
            while (leavers.Contains(selected)
                || leavers.Contains(getIDPassengerBellow(selected))
                || leavers.Contains(getIDPassengerUp(selected))
                || checkIfTwoPassengersAreOnSameXPos(selected, getIDPlayer())
            );
            leavers.Add(selected);
        }
        //leavers.Add(42);
        //leavers.Add(49);
        StartCoroutine(joinThreads(leavers));
	}

    IEnumerator joinThreads(List<int> leavers) {
        List<Coroutine> c = new List<Coroutine>();
        for (int i = 0; i < leavers.Count; i++) {
            Coroutine ci = StartCoroutine(getUpLeavers(leavers[i]));
            c.Add(ci);
        }

        for (int i = 0; i < leavers.Count; i++) {
            yield return c[i];
        }
        ready_to_advance = true;
    }

	IEnumerator getUpLeavers(int id) {
		leavers = new List<Leaver>();
        float threshold = 0.3f;
		float random_wait_time = Random.Range(0f, threshold);
		yield return new WaitForSeconds(Random.Range(0.3f, 0.5f));
		if (passengerOnFirstLineOfSeats(id)) {
			grid.swapTwoPassengers(id,
				getIDPassengerBellow(id),
				swap_duration
			);

			yield return new WaitForSeconds(swap_duration + random_wait_time);
			grid.swapTwoPassengers(
				getIDPassengerBellow(id),
				getIDPassengerBellow(getIDPassengerBellow(id)),
				swap_duration
			);
			Leaver l = new Leaver(getIDPassengerBellow(getIDPassengerBellow(id)));
			leavers.Add(l);
		} else if (passengerOnSecondLineOfSeats(id)) {
			grid.swapTwoPassengers(
				id,
				getIDPassengerBellow(id),
				swap_duration
			);
			Leaver l = new Leaver(getIDPassengerBellow(id));
			leavers.Add(l);
		} else if (passengerOnFirstLastLineOfSeats(id)) {
			grid.swapTwoPassengers(
				id,
				getIDPassengerUp(id),
				swap_duration
			);
			Leaver l = new Leaver((getIDPassengerUp(id)));
			leavers.Add(l);
		} else if (passengerOnSecondLastLineOfSeats(id)) {
			grid.swapTwoPassengers(
				id,
				getIDPassengerUp(id),
				swap_duration
			);
			yield return new WaitForSeconds(swap_duration + random_wait_time);
			grid.swapTwoPassengers(
				getIDPassengerUp(id),
				getIDPassengerUp(getIDPassengerUp(id)),
				swap_duration
			);
			Leaver l = new Leaver((getIDPassengerUp(getIDPassengerUp(id))));
			leavers.Add(l);
		} else {
			Debug.Log("erro, id: " + id);
		}
	}
    #endregion

    #region passo 2
	IDPosFromDoor getPosFromDoor(int id) {
        print("id #" + id + ", posFromDoor: " + grid.posFromDoor(id));
		return grid.posFromDoor (id);
	}

	void setAllLeavers(Leaver l) {
		int id = l.getID ();
		int player_id = getIDPlayer ();
		IDPosFromDoor pos = getPosFromDoor(id);
        l.setPos(pos);
		Direction nextDir;
        if (pos == IDPosFromDoor.ON_DOOR) {
            return;
        }
		nextDir = calculateNextDir(pos, id, player_id);
		callNextLeaver(id, nextDir);

    }

	Direction calculateNextDir(IDPosFromDoor pos, int id, int player_id) {
		switch (pos)
        {
            case IDPosFromDoor.LEFT:
                if (isOnHorizontalUp(id) && isOnHorizontalUp(player_id))
                    return Direction.DOWN;
                else if (isOnHorizontalDown(id) && isOnHorizontalDown(player_id))
                    return Direction.UP;
                else
                    return Direction.RIGHT;
            case IDPosFromDoor.RIGHT:
                if (isOnHorizontalUp(id) && isOnHorizontalUp(player_id))
                    return Direction.DOWN;
                else if (isOnHorizontalDown(id) && isOnHorizontalDown(player_id))
                    return Direction.UP;
                else
                    return Direction.LEFT;
            case IDPosFromDoor.MID:
                if (isOnVerticalLeft(id) && isOnVerticalLeft(player_id))
                    return Direction.RIGHT;
                else if (isOnVerticalRight(id) && isOnVerticalRight(player_id))
                    return Direction.LEFT;
                else
                    return Direction.DOWN;
			default:
				//print("nextDir = sair");
				return Direction.DOWN;				
        }
	}

    Direction getOppositeDirection(Direction dir)
    {
        switch (dir)
        {
            case Direction.UP:
                return Direction.DOWN;
            case Direction.DOWN:
                return Direction.UP;
            case Direction.LEFT:
                return Direction.RIGHT;
            case Direction.RIGHT:
                return Direction.LEFT;
        }
        print("entao... isso nao deveria acontecer");
        return Direction.DOWN;
    }

    List<Direction> forbiddenDirections(IDPosFromDoor pos, int id) {
        List<Direction> forbiddenDirs = new List<Direction>();
        switch (pos)
        {
            case IDPosFromDoor.LEFT:
                forbiddenDirs.Add(Direction.LEFT);
                break;
            case IDPosFromDoor.MID:
                forbiddenDirs.Add(Direction.UP);
                if (id % 10 == 4) {
                    forbiddenDirs.Add(Direction.LEFT);
                } else if (id % 10 == 5) {
                    forbiddenDirs.Add(Direction.RIGHT);
                }
                break;
            case IDPosFromDoor.RIGHT:
                forbiddenDirs.Add(Direction.RIGHT);
                break;
            case IDPosFromDoor.ON_DOOR:
                forbiddenDirs.Add(Direction.UP);
                break;
        }
        return forbiddenDirs;
    }

    IEnumerator leavingLoop()
    {
        int leavers_count = leavers.Count;
        foreach (Leaver l in leavers)
        {
            if (l.getPos() == IDPosFromDoor.ON_DOOR)
            {
                Destroy(grid.passengers[l.getID()]);
				grid.passengers[l.getID()] = null;
                leavers_count--;
                break;
            }
        }
		while (leavers_count > 0) {
            bool moved = false;
            for (int i = 0; i < leavers.Count; i++)
			{
				Leaver l = leavers[i];
				int id = l.getID();
                IDPosFromDoor pos = l.getPos();
                if (grid.passengers[id] == null) {
                    continue;
                }
				List<GameObject> adjs = grid.calculateAdj(id);
				foreach (GameObject tile in adjs) {
					int player_id = getIDPlayer ();
					int tile_id = tile.GetComponent<Tile>().getTileId();
                    IDPosFromDoor tile_pos = grid.posFromDoor(tile_id);
                    bool empty = grid.tileIsEmpty(tile_id);

                    Direction tileDir = calculateDirByID(id, tile_id);
                    Direction doorDir = calculateNextDir(pos, id, player_id);
                    //List<Direction> forbidden = forbiddenDirections(pos, id);

                    if (empty) {
                        if (/*forbidden.Contains(tileDir) || */tileDir != doorDir) {
                            continue;
                        }
                        //print ("Tile #" + tile_id + " is empty (detected by passenger #" + id + ").");
                        float mov_threshold = 0.3f;
                        float mov_time = Random.Range(swap_duration - mov_threshold, swap_duration + mov_threshold);
                        grid.movePassenger(id, tile_id, mov_time);
                        moved = true;
						//yield return new WaitForSeconds(0.5f);
						break;
					} else {
						//print ("Tile #" + tile_id + " is NOT empty (detected by passenger #" + id + ").");				
					}
				}
			}
            if (!moved) {
                for (int i = 0; i < leavers.Count; i++) {
                    Leaver l = leavers[i];
                    int id = l.getID();
                    if (l.getPos() == IDPosFromDoor.ON_DOOR) {
                        GameObject go = grid.passengers[l.getID()];
                        PassengerType p_type = go.GetComponent<Passenger>().getPassengerType();
                        grid.types_counter[p_type]--;
                        Destroy(go);
                        grid.passengers[l.getID()] = null;
                        leavers_count--;
                        break;
                    }
                }
            }
            float cycle_threshold = 0.4f;
            float cycle_time = Random.Range(swap_duration - cycle_threshold, swap_duration);
            yield return new WaitForSeconds(cycle_time);
        }
        print("Leavers.count: " + leavers.Count);
        ready_to_advance = true;
    }

    void callNextLeaver(int id, Direction dir)
    {
        int newId = calculateTargetID(dir, id);
        Leaver l = new Leaver(newId);
        if(!containsByID(l))
            leavers.Add(l);
        setAllLeavers(l);
    }


	Direction calculateDirByID (int origin, int destiny) {
		if (getIDPassengerRight(origin) == destiny) {
			return Direction.RIGHT;
		} else if (getIDPassengerLeft(origin) == destiny) {
			return Direction.LEFT;
		} else if (getIDPassengerUp(origin) == destiny) {
			return Direction.UP;
		} else if (getIDPassengerBellow(origin) == destiny) {
			return Direction.DOWN;
		} 
		print("opa, isso nao deveria estar acontecendo");
		return Direction.UP;
	}

    int calculateTargetID(Direction dir, int id) {
		int newID = -1;
		switch (dir) {
			case Direction.RIGHT:
				newID = getIDPassengerRight(id);
				break;
			case Direction.LEFT:
				newID = getIDPassengerLeft(id);
				break;
			case Direction.UP:
				newID = getIDPassengerUp(id);
				break;
			case Direction.DOWN:
				newID = getIDPassengerBellow(id);
				break;
		}
		return newID;
	}

    #endregion

    #region passo 3

    void resetEnterersList() {
        n_leavers = 0;
        enterers.Clear();
    }

    void resetLeaversList()
    {
        n_leavers = leavers.Count;
        leavers.Clear();
    }

    IEnumerator enteringLoop()
    {
        enterers = new List<Leaver>();
        added = 0;
        createNewPassenger();

        while (added < n_leavers)
        {
            for (int i = 0; i < added; i++)
            {
                Leaver p = enterers[i];
                int p_id = p.getID();

                List<GameObject> adjs = grid.calculateAdj(p_id);
                foreach (GameObject tile in adjs)
                {
                    int player_id = getIDPlayer();
                    int tile_id = tile.GetComponent<Tile>().getTileId();
                    bool empty = grid.tileIsEmpty(tile_id);

                    if (empty)
                    {
                        //print ("Tile #" + tile_id + " is empty (detected by passenger #" + p_id + ").");
                        float mov_threshold = 0.3f;
                        float mov_time = Random.Range(swap_duration - mov_threshold, swap_duration + mov_threshold);
                        grid.movePassenger(p_id, tile_id, mov_time);
                        p.setID(tile_id);
                        break;
                    }
                }
                yield return new WaitForSeconds(0.3f);
            }
            createNewPassenger();
            yield return new WaitForSeconds(0.3f);
        }
        print("aaaaaaaa, added: " + added + ", n_leavers: " + n_leavers);
        ready_to_advance = true;

        print("vou pausar");
        Debug.Break();
    }

    void createNewPassenger ()
    {
        int door_id = getDoorID();
        grid.spawnPassenger(door_id);
        Leaver l = new Leaver(door_id);
        added++;
        enterers.Add(l);
    }

    int getDoorID()
    {
        int door_id = grid.door_id1;
        if (grid.passengers[door_id] == null) {
            return door_id;
        }
        else {
            door_id = grid.door_id2;
            return door_id;
        }
    }

    #endregion

    #region utility

    int getIDPassengerBellow(int id) {
		return id + 10;
	}

	int getIDPassengerUp(int id) {
		return id - 10;
	}

    int getIDPassengerLeft(int id)
    {
        return id - 1;
    }

    int getIDPassengerRight(int id)
    {
        return id + 1;
    }

    bool passengerOnFirstLineOfSeats(int id) {
		return id >= 0 && id < 10;
	}

	bool passengerOnSecondLineOfSeats(int id) {
		return id >= 10 && id < 20;
	}

	bool passengerOnFirstLastLineOfSeats(int id) {
		return (id >= 40 && id < 44) || (id >= 46 && id < 50);
	}

	bool passengerOnSecondLastLineOfSeats(int id) {
		return (id >= 50 && id < 54) || (id >= 56 && id < 60);
	}

	bool checkIfTwoPassengersAreOnSameXPos(int id_tile1, int id_tile2) {
		return (id_tile1 % 10 == id_tile2 % 10);
	}

	int getIDPlayer() {
		return FindObjectOfType<Player>().getTileId();
	}

	bool isOnHorizontalUp(int id) {
		return (id >= 20 && id < 30);
	}

	bool isOnHorizontalDown(int id) {
		return (id >= 30 && id < 40);
	}

	bool isOnVerticalLeft(int id) {
		return (id % 10 == 4);
	}

	bool isOnVerticalRight(int id) {
		return (id % 10 == 5);
	}

    void printLeaversList()
    {
        string temp = "{ ";
        for (int i = 0; i < leavers.Count; i++)
        {
            temp += leavers[i].getID() + ", ";
        }
        temp += "}";
        Debug.Log(temp);
    }

    bool containsByID(Leaver l)
    {
        foreach (Leaver leaver in leavers)
        {
            if (leaver.getID() == l.getID())
                return true;
        }
        return false;
    }

    #endregion
}
