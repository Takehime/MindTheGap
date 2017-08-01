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
		if (id >= 0 && id < 10) {
			//Debug.Log ("id: " + id + " swap duas vezes pra baixo");
			grid.swapTwoPassengers (id, id + 10, swap_duration);
			yield return new WaitForSeconds (swap_duration);
			grid.swapTwoPassengers (id + 10, id + 20, swap_duration);
		} else if (id >= 10 && id < 20) {
			//Debug.Log ("id: " + id + " swap uma vez pra baixo");
			grid.swapTwoPassengers (id, id + 10, swap_duration);
		} else if ((id >= 40 && id < 44) || (id >= 46 && id < 50)) {
			//Debug.Log ("id: " + id + " swap uma vez pra cima");
			grid.swapTwoPassengers (id, id - 10, swap_duration);
		} else if ((id >= 50 && id < 54) || (id >= 56 && id < 60)) {
			//Debug.Log ("id: " + id + " swap duas vezes pra cima");
			grid.swapTwoPassengers (id, id - 10, swap_duration);
			yield return new WaitForSeconds (swap_duration);
			grid.swapTwoPassengers (id - 10, id - 20, swap_duration);
		}
    }
}
