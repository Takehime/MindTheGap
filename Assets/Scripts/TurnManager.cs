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

    public float secs_between_turns;

    private Turn curr_turn;

    void Start()
    {
        curr_turn = Turn.BetweenStations ;
        StartCoroutine(turnLoop());
    }

    public void setTurn(Turn turn)
    {
        curr_turn = turn;
        changeTurn(curr_turn);
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
