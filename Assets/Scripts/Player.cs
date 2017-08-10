using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPassenger : MonoBehaviour {}

public class Player : IPassenger {

    public delegate void SwapMode(int id, bool bo);
    public event SwapMode swapMode;


    private int curr_tile_id;
    private bool swap_mode_active = false;
    private TurnManager.Turn curr_turn;
    
    public void generatePlayer(int tile_id)
    {
        curr_tile_id = tile_id;
        FindObjectOfType<TurnManager>().changeTurn += changeTurn;
        curr_turn = TurnManager.Turn.BetweenStations;
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
    }

    void changeTurn(TurnManager.Turn turn)
    {
        curr_turn = turn;
    }

    public void onSwapMode()
    {
        if (swapMode != null)
        {
            if (!swap_mode_active)
            {
                if (curr_turn == TurnManager.Turn.BetweenStations)
                {
                    swap_mode_active = true;
                    swapMode(curr_tile_id, true);
                }
            }
            else
            {
                if (curr_turn == TurnManager.Turn.BetweenStations)
                {
                    swap_mode_active = false;
                    swapMode(curr_tile_id, false);
                }

            }
        }
    }

}
