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
    public float initial_time_between_turns;
    public Coroutine turn_loop;
    public Turn curr_turn;

    public Animator background_animator;
    public Animator camera_animator;

    private MapManager mp;
    private Scan scan;
    private Pachinko pachinko;
    private static int routeMapIndex = 0;
    bool first_time = true;

    void Start()
    {
        mp = FindObjectOfType<MapManager>();
        scan = FindObjectOfType<Scan>();
        pachinko = FindObjectOfType<Pachinko>();
        //setTurnToBetweenStations();
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
        //background_animator.SetTrigger("stop");

        if (changeTurn != null) {
            changeTurn(curr_turn);
        }
        if (startStationLeaving != null) {
            startStationLeaving();
        }

        scan.leaveScanMode();
        advanceOnMapRoute();
        pachinko.makePachinkoFaster();
    }

    public void setTurnToBetweenStations() {
        if (first_time) {
            first_time = false;
        } else {
            background_animator.SetTrigger("start");
            camera_animator.SetTrigger("start_shake");
        }

        curr_turn = Turn.BetweenStations;

        if (changeTurn != null) {
            changeTurn(curr_turn);
        }
            advanceOnMapRoute();
        //moveMap();
        //print("movi o mapa");

        turn_loop = StartCoroutine(BetweenStationTurnLoop());
    }

    public void setTurnToBetweenStations_Ending() {
        background_animator.SetTrigger("stop");
        camera_animator.SetTrigger("stop_shake");

        curr_turn = Turn.BetweenStations;
        if (changeTurn != null) {
            changeTurn(curr_turn);
        }
    }

    public IEnumerator BetweenStationTurnLoop()
    {
        yield return new WaitForSeconds(initial_time_between_turns);
        setTurnToAtStation();
    }
    #endregion

    #region map handling

    void moveMap() {
        Vector3 target = map.transform.position - new Vector3(space_between_stations_on_map, 0, 0);
        map.transform.DOMove(target, initial_time_between_turns);
    }

    void advanceOnMapRoute() {
        routeMapIndex++;
        mp.attMap(routeMapIndex);
    }

    #endregion
}