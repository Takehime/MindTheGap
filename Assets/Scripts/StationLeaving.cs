using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationLeaving : MonoBehaviour {

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
            do
                selected = Random.Range(0, 60);
            while (leavers.Contains(selected));
            leavers.Add(selected);
        }

        for (int i = 0; i < leavers.Count; i++)
        {
            getUpLeavers(leavers[i]);
        }
    }


    void getUpLeavers(int id)
    {
        Debug.Log("o passageiro " + id + " vai saltar");
    }
}
