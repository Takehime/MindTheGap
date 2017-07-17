using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private int curr_tile_id;


    public void generatePlayer(int tile_id)
    {
        curr_tile_id = tile_id;
    }

}
