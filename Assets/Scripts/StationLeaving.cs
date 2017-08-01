using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationLeaving : MonoBehaviour {

	public float swap_duration;

    private int max_of_leavers = 9; // (1/4) dos sentados
    private Grid grid;
	private List<int> leavers_ids;

	void Start () {
        grid = FindObjectOfType<Grid>();
        FindObjectOfType<TurnManager>().startStationLeaving += startStationLeaving;
    }

	void startStationLeaving()
    {
		StartCoroutine (stationLeavingCoroutine());

    }

	IEnumerator stationLeavingCoroutine() {

		//passo 1
		selectSeats();
		yield return new WaitForSeconds (3f);
		Grid.printList (leavers_ids);

		//passo 2
		selectPassengersFromCorridor();

	}

	void selectPassengersFromCorridor() {
		for (int i = 0; i < max_of_leavers; i++) {
			int id = leavers_ids [i];
			Grid.IDPosFromDoor pos_from_door = grid.posFromDoor (id);
			Debug.Log ("id: " + id + ", position from door: " + pos_from_door);
		}
	}

	void selectSeats()
    {
        List<int> leavers = new List<int>();
        for (int i = 0; i < max_of_leavers; i++)
        {
            int selected;
			do {
				List<int> seats = grid.getAllSeats();
				int index = Random.Range(0, grid.getAllSeats().Count);
				selected = seats[index];
			}
			while (leavers.Contains(selected) 
				|| leavers.Contains(getIDPassengerBellow(selected)) 
				|| leavers.Contains(getIDPassengerUp(selected)));
            leavers.Add(selected);
        }
		Grid.printList (leavers);


        for (int i = 0; i < leavers.Count; i++)
        {
			StartCoroutine(getUpLeavers(leavers[i]));
        }
    }

    IEnumerator getUpLeavers(int id)
    {
		leavers_ids = new List<int> ();
		yield return new WaitForSeconds (Random.Range(0.3f, 0.5f));
		if (passengerOnFirstLineOfSeats (id)) {
			grid.swapTwoPassengers (id, 
				getIDPassengerBellow (id), 
				swap_duration
			);
			float threshold = 0.3f;
			float random_wait_time = Random.Range (0f, threshold);
			yield return new WaitForSeconds (swap_duration + random_wait_time);
			grid.swapTwoPassengers (
				getIDPassengerBellow (id), 
				getIDPassengerBellow (getIDPassengerBellow (id)), 
				swap_duration
			);
			leavers_ids.Add (getIDPassengerBellow (getIDPassengerBellow (id)));
		} else if (passengerOnSecondLineOfSeats (id)) {
			grid.swapTwoPassengers (
				id, 
				getIDPassengerBellow (id), 
				swap_duration
			);
			leavers_ids.Add (getIDPassengerBellow (id));
		} else if (passengerOnFirstLastLineOfSeats (id)) {
			grid.swapTwoPassengers (
				id, 
				getIDPassengerUp (id), 
				swap_duration
			);
			leavers_ids.Add ((getIDPassengerUp (id)));
		} else if (passengerOnSecondLastLineOfSeats (id)) {
			grid.swapTwoPassengers (
				id, 
				getIDPassengerUp (id), 
				swap_duration
			);
			yield return new WaitForSeconds (swap_duration);
			grid.swapTwoPassengers (
				getIDPassengerUp (id), 
				getIDPassengerUp (getIDPassengerUp (id)), 
				swap_duration
			);
			leavers_ids.Add ((getIDPassengerUp (getIDPassengerUp (id))));
		} else {
			Debug.Log ("erro, id: " + id);
		}

    }

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

}
