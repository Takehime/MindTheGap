using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public enum Direction {
	RIGHT, LEFT, UP, DOWN
}

public class StationLeaving : MonoBehaviour {

	public float swap_duration;

	private int max_of_leavers = 9; // (1/4) dos sentados
	private Grid grid;
	private List<Leaver> leavers;

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

		Debug.Log (leavers.Count);

		//passo 2
		getXPosFromDoor();
		StartCoroutine(passengersLeavingLoop());
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
		Debug.Log (leavers.Count);

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

	void getXPosFromDoor() {
		for (int i = 0; i < leavers.Count; i++) {
			int id = leavers [i].getID();
			IDPosFromDoor pos_from_door = grid.posFromDoor (id);
			leavers[i].setPos(pos_from_door);
			//Debug.Log ("id: " + id + ", position from door: " + leavers[i].getPos());
		}
	}

	IEnumerator passengersLeavingLoop() {
		//Debug.Log("id: " + leavers[0].getID());

		IDPosFromDoor pos = leavers[0].getPos();
		switch (pos) {
		case IDPosFromDoor.LEFT:
			while (leavers [0].getPos () != IDPosFromDoor.MID) {
				Direction nextDir = Direction.RIGHT;
				if (!playerIsOnTheWay (nextDir, /*index*/0))
					moveToDirection (nextDir, /*index*/0);
				else {
					Debug.Log ("player está no caminho");
					bypassPlayer (nextDir, /*index*/0);
				}
				yield return new WaitForSeconds (1.5f);
			}

			while (leavers [0].getPos () != IDPosFromDoor.ON_DOOR) {
				Direction nextDir = Direction.DOWN;
				if (!playerIsOnTheWay (nextDir, /*index*/0))
					moveToDirection (nextDir, /*index*/0);
				else {
					Debug.Log ("player está no caminho");
					bypassPlayer (nextDir, /*index*/0);
				}
				yield return new WaitForSeconds (1.5f);
			}
			break;

		case IDPosFromDoor.RIGHT:
			while (leavers [0].getPos () != IDPosFromDoor.MID) {
				Direction nextDir = Direction.LEFT;
				if (!playerIsOnTheWay (nextDir, /*index*/0))
					moveToDirection (nextDir, /*index*/0);
				else {
					Debug.Log ("player está no caminho");
					bypassPlayer (nextDir, /*index*/0);
				}
				yield return new WaitForSeconds (1.5f);
			}

			while (leavers [0].getPos () != IDPosFromDoor.ON_DOOR) {
				Direction nextDir = Direction.DOWN;
				if (!playerIsOnTheWay (nextDir, /*index*/0))
					moveToDirection (nextDir, /*index*/0);
				else {
					Debug.Log ("player está no caminho");
					bypassPlayer (nextDir, /*index*/0);
				}
				yield return new WaitForSeconds (1.5f);
			}
			break;

		case IDPosFromDoor.MID:
			while (leavers [0].getPos () != IDPosFromDoor.ON_DOOR) {
				Direction nextDir = Direction.DOWN;
				if (!playerIsOnTheWay (nextDir, /*index*/0))
					moveToDirection (nextDir, /*index*/0);
				else {
					Debug.Log ("player está no caminho");
					bypassPlayer (nextDir, /*index*/0);
				}
				yield return new WaitForSeconds (1.5f);
			}
			break;
		}
	}

	void bypassPlayer(Direction nextDir, int index) {
		int id = leavers [index].getID ();
		//int targetID = calculateTargetID (nextDir, index);
		if (nextDir == Direction.LEFT || nextDir == Direction.RIGHT) {
			if (isOnHorizontalLineUp(index))
				moveToDirection (Direction.DOWN, index);
			else
				moveToDirection (Direction.UP, index);
		} else {
			if (isOnVerticalLineLeft(index))
				moveToDirection (Direction.RIGHT, index);
			else
				moveToDirection (Direction.LEFT, index);
		}
	}

	bool playerIsOnTheWay(Direction dir, int index) {
		int targetID = calculateTargetID (dir, index);
		if (getIDPlayer () == targetID)
			return true;
		else
			return false;
	}

	void moveToDirection(Direction dir, int index) {
		int id = leavers [index].getID ();
		int newID = calculateTargetID (dir, index);
		grid.swapTwoPassengers(id, newID, 1f);

		leavers[index].setID(newID);
		leavers[index].setPos(grid.posFromDoor(newID));
	}

	int calculateTargetID(Direction dir, int index) {
		int newID = -1;
		switch (dir) {
			case Direction.RIGHT:
				newID = leavers[index].getID() + 1;
				break;
			case Direction.LEFT:
				newID = leavers[index].getID() - 1;
				break;
			case Direction.UP:
				newID = leavers[index].getID() - 10;
				break;
			case Direction.DOWN:
				newID = leavers[index].getID() + 10;
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

	bool isOnHorizontalLineUp(int index) {
		if (leavers [index].getID () >= 20 && leavers [index].getID () < 30)
			return true;
		else
			return false;
	}

	bool isOnVerticalLineLeft(int index) {
		if (leavers [index].getID () % 10 <= 4)
			return true;
		else 
			return false;
	}


	#endregion
}
