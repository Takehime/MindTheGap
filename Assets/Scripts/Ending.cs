using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ending : MonoBehaviour {

    private AtStation at;
    private Grid grid;
    private TurnManager tm;
    private List<Leaver> leavers = new List<Leaver>();

	public IEnumerator triggerEnd() {
        print("o começo do fim chegou");
        at = FindObjectOfType<AtStation>();
        grid = FindObjectOfType<Grid>();
        tm = FindObjectOfType<TurnManager>();

		StopCoroutine(at.leaving_coroutine);
        StopCoroutine(tm.turn_loop);

        //List<int> not_seats = grid.getAllNotSeats();
		for (int i = 0; i < grid.tiles.Count; i++) {
			if (grid.getPlayerID () == i) {
				continue;
			}
			Leaver l = new Leaver(i);
			l.setPos(grid.posFromDoor(i));
            leavers.Add(l);
        }
        at.leavers = leavers;
        Coroutine end_loop = StartCoroutine(at.leavingLoop());

		yield return at.waitForReadyForAdvance();

		grid.changeTurn (TurnManager.Turn.BetweenStations);
		grid.ending_event = true;

    }


}
