using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    public delegate void SwapMode(int id, bool bo);
    public event SwapMode swapMode;

    private int curr_tile_id;
    private bool swap_mode_active = false;
    
    public void generatePlayer(int tile_id)
    {
        curr_tile_id = tile_id;
    }

    public void setSwapMode(bool bo)
    {
        swap_mode_active = bo;
    }

    public int getTileId()
    {
        return curr_tile_id;
    }

    public void setTileId(int newId)
    {
        curr_tile_id = newId;
        Debug.Log("Player tile_id: " + newId);
    }

    public void onSwapMode()
    {
        if (swapMode != null)
        {
            if (!swap_mode_active)
            {
                swap_mode_active = true;
                swapMode(curr_tile_id, true);
            }
            else
            {
                swap_mode_active = false;
                swapMode(curr_tile_id, false);
            }
        }
    }

}
