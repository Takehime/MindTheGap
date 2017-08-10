using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Passenger : MonoBehaviour {

    public delegate void InteractWithPassenger(int id, PassengerData.PassengerType p_type);
    public event InteractWithPassenger swapTarget;
    public event InteractWithPassenger scanTarget;

    private PassengerData.PassengerType p_type;
    private Color color;

    private int curr_tile_id;

    public int generatePassenger(List<PassengerData> passenger_types, int tile_id)
    {
        gameObject.name = "Passenger " + tile_id;
        curr_tile_id = tile_id;
        int chosen = Random.Range(0, passenger_types.Capacity);
        p_type = passenger_types[chosen].type;
        color = passenger_types[chosen].color;
        GetComponent<Image>().color = color;
        return chosen;
    }

    public void setTileId(int newId)
    {
        curr_tile_id = newId;
    }

    public int getTileId()
    {
        return curr_tile_id;
    }

    public void onSwapMode()
    {
        if (swapTarget != null)
        {
            swapTarget(curr_tile_id, p_type);
        }
    }

    public void onScanMode()
    {
        if (scanTarget != null)
        {
            scanTarget(curr_tile_id, p_type);
        }
    }

}
