using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{

    public enum Turn
    {
        AtStation, BetweenStations
    };

    public delegate void ChangeTurn(Turn next_turn);
    public event ChangeTurn changeTurn;

    public delegate void StartStationLeaving();
    public event StartStationLeaving startStationLeaving;

    public float secs_between_turns;

    private Turn curr_turn;

    void Start()
    {
        curr_turn = Turn.BetweenStations ;
        //StartCoroutine(turnLoop());
    }

    void Update()
    {
        changeTurnByInput();
    }

    public void setTurn(Turn turn)
    {
        curr_turn = turn;
        Debug.Log(curr_turn);
        if (changeTurn != null)
        {
            changeTurn(curr_turn);
        }
        if (turn == Turn.AtStation)
        {
            if (startStationLeaving != null)
            {
                startStationLeaving();
            }
        }
    }

    void changeTurnByInput()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (curr_turn == Turn.BetweenStations)
                setTurn(Turn.AtStation);
            else
                setTurn(Turn.BetweenStations);
        }
    }

    IEnumerator turnLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(secs_between_turns);
            
            if (curr_turn == Turn.BetweenStations)
                setTurn(Turn.AtStation);
            else
                setTurn(Turn.BetweenStations);
        }
    }
}
