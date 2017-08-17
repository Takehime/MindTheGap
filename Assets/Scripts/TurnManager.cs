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
    public Coroutine turn_loop;

    private Turn curr_turn;
    private MapManager mp;
    private Scan scan;
    private static int routeMapIndex = 0;

    void Start()
    {
        mp = FindObjectOfType<MapManager>();
        scan = FindObjectOfType<Scan>();
        setTurnToBetweenStations();
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
        curr_turn = Turn.AtStation;
        Debug.Log("turn: " + curr_turn);
        if (changeTurn != null) {
            changeTurn(curr_turn);
        }
        if (startStationLeaving != null) {
            startStationLeaving();
        }
        scan.leaveScanMode();
        advanceOnMapRoute();
    }

    public void setTurnToBetweenStations() {
        curr_turn = Turn.BetweenStations;
        Debug.Log("turn: " + curr_turn);
        if (changeTurn != null) {
            changeTurn(curr_turn);
        }
        //moveMap();
        //print("movi o mapa");

        advanceOnMapRoute();
        turn_loop = StartCoroutine(BetweenStationTurnLoop());

    }

    public IEnumerator BetweenStationTurnLoop()
    {
        yield return new WaitForSeconds(time_between_turns);
        setTurnToAtStation();
    }
    #endregion

    #region map handling

    void moveMap() {
        Vector3 target = map.transform.position - new Vector3(space_between_stations_on_map, 0, 0);
        map.transform.DOMove(target, time_between_turns);
    }

    void advanceOnMapRoute() {
        routeMapIndex++;
        mp.attMap(routeMapIndex);
    }

    #endregion

}
