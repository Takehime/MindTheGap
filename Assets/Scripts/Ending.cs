using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending : MonoBehaviour {

    private AtStation at;
    private Grid grid;
    private TurnManager tm;
    private List<Leaver> leavers = new List<Leaver>();

    public void triggerEnd() {
        print("o começo do fim chegou");
        at = FindObjectOfType<AtStation>();
        grid = FindObjectOfType<Grid>();
        tm = FindObjectOfType<TurnManager>();

        StopIEnumerator(at.leaving_coroutine);
        StopCoroutine(tm.turn_loop);

        List<int> not_seats = grid.getAllNotSeats();
        for (int i = 0; i < not_seats.Count; i++) {
            Leaver l = new Leaver(not_seats[i]);
            l.setPos(grid.posFromDoor(not_seats[i]));
            leavers.Add(l);
        }
        at.leavers = leavers;
        StartCoroutine(at.leavingLoop());

    }
}
