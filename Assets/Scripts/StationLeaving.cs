﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum Direction {
	RIGHT, LEFT, UP, DOWN
}

public class StationLeaving : MonoBehaviour {

	public float swap_duration;

	private int max_of_leavers = 2; // 9 = (1/4) dos sentados
	private Grid grid;
	private List<Leaver> leavers;
	private bool player_up;
	private bool player_left;

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
		FindObjectOfType<TurnManager>().startStationLeaving += startStationLeaving;
	}

	#region main loop

	void startStationLeaving()
	{
		StartCoroutine (stationLeavingCoroutine());
	}

	IEnumerator stationLeavingCoroutine() {

		//passo 1
		selectSeats();
		yield return new WaitForSeconds (3f);

        //passo 2
        printLeaversList();
        for (int i = 0; i < leavers.Count; i++)
            setAllLeavers(leavers[i]);
        printLeaversList();
        StartCoroutine(leavingLoop());
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

		for (int i = 0; i < leavers.Count; i++) {
			StartCoroutine(getUpLeavers(leavers[i]));
		}
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
		return grid.posFromDoor (id);
	}

	void setAllLeavers(Leaver l) {

		int id = l.getID ();
		int player_id = getIDPlayer ();
		IDPosFromDoor pos = getPosFromDoor(id);
        l.setPos(pos);
		Direction nextDir;
        //Debug.Log("id: " + id);

        if (pos == IDPosFromDoor.ON_DOOR)
        {
//            Debug.Log("na porta");
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
				print("nextDir = sair");
				return Direction.DOWN;				
        }
	}

    IEnumerator leavingLoop()
    {
		Leaver who_left = null;
        foreach (Leaver l in leavers)
        {
            if (l.getPos() == IDPosFromDoor.ON_DOOR)
            {
                Destroy(grid.passengers[l.getID()]);
				grid.passengers[l.getID()] = null;
				who_left = l;
            }
        }

		leavers.Remove (who_left);
        //yield return new WaitForSeconds(swap_duration);

		int lastMoved = -1;
		int lastLeft = -1;
		while(leavers.Count > 0) {

			for (int i = 0; i < leavers.Count; i++)
			{
				Leaver l = leavers[i];
				int id = l.getID();

				List<GameObject> adjs = grid.calculateAdj(id);
				// print("adjs do tile #" + id + ": ");
				// Grid.printList(adjs);
				foreach (GameObject tile in adjs)
				{
					int player_id = getIDPlayer ();
					int tile_id = tile.GetComponent<Tile>().getTileId();
					IDPosFromDoor tile_pos = grid.posFromDoor(tile_id);
					
					Direction doorDir = calculateNextDir(tile_pos, tile_id, player_id);
					Direction tileDir = calculateDirByID(id, tile_id);
					//print("direction for tile #" + tile_id + " : " + nextDir);

					bool empty = grid.tileIsEmpty(tile_id);

					//if(doorDir == tileDir) {
					//	print("origin tile #" + id + ", destiny tile #" + tile_id);
					//	print("doorDir (" + doorDir + ") == tileDir ("  + tileDir + ")");
					//}

					if (empty) {
						if (lastMoved == tile_id && lastLeft == l.getID()) {
							continue;
						}
					
						print ("Tile #" + tile_id + " is empty (detected by passenger #" + id + ").");
						// print("Last Moved: " + lastMoved.getID() + ", moving " + l.getID());
						
						//Debug.Break();	
						grid.movePassenger(id, tile_id, swap_duration);
						lastLeft = tile_id;
						lastMoved = l.getID();
						
						yield return new WaitForSeconds(swap_duration);
						break;
					} else {
						//print ("Tile #" + tile_id + " is NOT empty (detected by passenger #" + id + ").");				
					}
				}
			}

			yield return new WaitForSeconds(swap_duration);

			// print("Leavers.count: " + leavers.Count);
		}

		yield break;
    }

    void callNextLeaver(int id, Direction dir)
    {
        int newId = calculateTargetID(dir, id);
        Leaver l = new Leaver(newId);
        if(!containsByID(l))
            leavers.Add(l);
        setAllLeavers(l);
    }


    /*
    void moveToDirection(Direction dir, int index) {
		int id = leavers [index].getID ();
		int newID = calculateTargetID (dir, index);
		grid.swapTwoPassengers(id, newID, 1f);

		leavers[index].setID(newID);
		leavers[index].setPos(grid.posFromDoor(newID));
	}
    */

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
