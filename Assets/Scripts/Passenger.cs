using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Passenger : MonoBehaviour {
    
    private PassengerData.PassengerType p_type;
    private int curr_tile_id;

    public void generatePassenger(List<PassengerData> passenger_types, int tile_id)
    {
        curr_tile_id = tile_id;
        PassengerData passenger = passenger_types[Random.Range(0, passenger_types.Capacity)];
        p_type = passenger.type;
        GetComponent<Image>().color = passenger.color;
    }
    
}
