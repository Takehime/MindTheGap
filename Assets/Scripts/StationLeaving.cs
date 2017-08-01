using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationLeaving : MonoBehaviour {

	public float swap_duration;

    private int max_of_leavers = 9; // (1/4) dos sentados
    private Grid grid;

	void Start () {
        grid = FindObjectOfType<Grid>();
        FindObjectOfType<TurnManager>().startStationLeaving += startStationLeaving;
    }

    void startStationLeaving()
    {
        selectPassengersToLeaveBus();
    }

    void selectPassengersToLeaveBus()
    {
        List<int> leavers = new List<int>();
        for (int i = 0; i < max_of_leavers; i++)
        {
            int selected;
			do {
                selected = Random.Range(0, 60);
				if (leavers.Contains(selected + 10) || leavers.Contains(selected - 10)) {
				}
			}
			while (leavers.Contains(selected) || leavers.Contains(selected + 10) || leavers.Contains(selected - 10));
            leavers.Add(selected);
        }

        for (int i = 0; i < leavers.Count; i++)
        {
			StartCoroutine(getUpLeavers(leavers[i]));
        }
    }

    IEnumerator getUpLeavers(int id)
    {
		yield return new WaitForSeconds (Random.Range(0.3f, 0.5f));
		if (passengerOnFirstLineOfSeats(id)) {
			grid.swapTwoPassengers (id, getIDPassengerBellow(id), swap_duration);
			float threshold = 0.3f;
			float random_wait_time = Random.Range (0f, threshold);
			yield return new WaitForSeconds (swap_duration + random_wait_time);
			grid.swapTwoPassengers (
				getIDPassengerBellow(id), 
				getIDPassengerBellow(getIDPassengerBellow(id)), 
				swap_duration
			);
		} else if (passengerOnSecondLineOfSeats(id)) {
			grid.swapTwoPassengers (id, getIDPassengerBellow(id), swap_duration);
		} else if (passengerOnFirstLastLineOfSeats(id)) {
			grid.swapTwoPassengers (id, getIDPassengerUp(id), swap_duration);
		} else if (passengerOnSecondLastLineOfSeats(id)) {
			grid.swapTwoPassengers (id, getIDPassengerUp(id), swap_duration);
			yield return new WaitForSeconds (swap_duration);
			grid.swapTwoPassengers (
				getIDPassengerUp(id), 
				getIDPassengerUp(getIDPassengerUp(id)), 
				swap_duration
			);
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
