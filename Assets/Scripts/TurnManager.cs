using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    public GameObject map;
    public float space_between_stations_on_map;
    public float time_between_turns;

    private Turn curr_turn;

    void Start()
    {
        setTurnToBetweenStations();
        StartCoroutine(turnLoop());
    }

    void Update()
    {
        //changeTurnByInput();
    }

    void changeTurnByInput() {
        if (Input.GetKeyDown(KeyCode.T)) {
            if (curr_turn == Turn.BetweenStations)
                setTurnToAtStation();
            else
                setTurnToBetweenStations();
        }
    }

    #region turn main loop

    void setTurnToAtStation() {
        Debug.Log("turn: " + curr_turn);
        curr_turn = Turn.AtStation;
        if (changeTurn != null) {
            changeTurn(curr_turn);
        }
        if (startStationLeaving != null) {
            startStationLeaving();
        }
    }

    public void setTurnToBetweenStations() {
        Debug.Log("turn: " + curr_turn);
        curr_turn = Turn.BetweenStations;
        if (changeTurn != null) {
            changeTurn(curr_turn);
        }
        moveMap();
        print("movi o mapa");
    }

    IEnumerator turnLoop()
    {
        while(true)
        {
            yield return new WaitForSeconds(time_between_turns);
            
            if (curr_turn == Turn.BetweenStations)
                setTurnToAtStation();
        }
    }
    #endregion

    #region map handling

    void moveMap() {
        Vector3 target = map.transform.position - new Vector3(space_between_stations_on_map, 0, 0);
        map.transform.DOMove(target, time_between_turns);
    }

    #endregion

}
