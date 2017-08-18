using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.PostProcessing;

public class Ending : MonoBehaviour {

    private AtStation at;
    private Grid grid;
    private TurnManager tm;
    private List<Leaver> leavers = new List<Leaver>();
    private AudioManager audio;

    public DialogBox dialog;
    public Transform driver;

    public Camera mainCamera;
    public Roll credits;

    void Start() {
        audio = AudioManager.Get_Audio_Manager();
        at = FindObjectOfType<AtStation>();
        grid = FindObjectOfType<Grid>();
        tm = FindObjectOfType<TurnManager>();
        // StartCoroutine(Driver_Ending());
    }

	public IEnumerator triggerEnd() {
        print("o começo do fim chegou");

		StopCoroutine(at.leaving_coroutine);
        StopCoroutine(tm.turn_loop);

        StartCoroutine(audio.Fade_Out(4f));
        audio.bus_background_source.Stop();
        StartCoroutine(Ending_Song());

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

    IEnumerator Ending_Song() {
        yield return new WaitForSeconds(6f);
        audio.Play_Real(audio.libera_me);
        at.swap_duration = 0.8f;
    }

    public IEnumerator Driver_Ending() {
        float time = 1f;
        final_breath = true;
        yield return new WaitForSeconds(time);
        driver.DOLocalMoveY(driver.transform.localPosition.y + 65, time);
        yield return new WaitForSeconds(time);
        driver.localScale = new Vector3(
            driver.localScale.x * -1,
            driver.localScale.y,
            driver.localScale.z
        );
        yield return new WaitForSeconds(time);

        var player = GameObject.FindGameObjectWithTag("Player");

        if (AtStation.passengerOnSecondLineOfSeats(grid.getPlayerID())) {
            driver.DOMove(player.transform.position - new Vector3(0, 2), time * 5);
            yield return new WaitForSeconds(time*5);
            driver.DOMove(player.transform.position - new Vector3(0, 1), time * 5);
            yield return new WaitForSeconds(time*5);
        }
        else if (AtStation.passengerOnFirstLastLineOfSeats(grid.getPlayerID())) {
            driver.DOMove(player.transform.position + new Vector3(0, 1), time * 10);
            yield return new WaitForSeconds(time*10);
        }

       yield return dialog.Text();
       
       audio.Play_Real(audio.enya_time);

       yield return Roll_Credits();

       yield return new WaitForSeconds(3.0f);

       audio.Play(audio.passenger_in, 1f);
       yield return new WaitForSeconds(0.3f);

       Application.Quit();
    }

    public static bool final_breath = false;

    IEnumerator Roll_Credits() {
        credits.gameObject.SetActive(true);
        mainCamera.GetComponentInChildren<PostProcessingBehaviour>().enabled = true;
        yield return new WaitUntil(() => credits.ended);
    }
}