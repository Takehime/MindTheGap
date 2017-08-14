using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class IntroManager : MonoBehaviour {

    float last_apm = 0f;
    Coroutine coroutine_shake_player;

    [SerializeField]
    Image player;
    [SerializeField]
    IntroBus bus;
    [SerializeField]
    IntroVTVOM vtvom;
    [SerializeField]
    GameObject indicator;
    [SerializeField]
    Intro_BusCountdown bus_countdown;

    void Start () {
        coroutine_shake_player = StartCoroutine(Shake_Player());
        StartCoroutine(Handle_Bus_Behaviour());
        //StartCoroutine(QTE_Timer());
        //StartCoroutine(Player_Enter_Bus());

        bus.gameObject.SetActive(false);
        bus_countdown.gameObject.SetActive(false);
    }

    void Update () {
		if (bus.is_bus_at_platform && Input.GetKeyDown(KeyCode.F)) {
            last_apm++;
        }
	}

    IEnumerator Handle_Bus_Behaviour() {
        yield return new WaitUntil(() => vtvom.times_checked == 10);

        bus.gameObject.SetActive(true);

        yield return bus.Enter_Scene();

        indicator.SetActive(true);
        bus_countdown.gameObject.SetActive(true);
        yield return QTE_Timer();
        indicator.SetActive(false);
        bus_countdown.gameObject.SetActive(false);

        if (coroutine_shake_player != null) {
            StopCoroutine(coroutine_shake_player);
            coroutine_shake_player = null;
        }

        yield return Player_Enter_Bus();

        yield return bus.Exit_Scene();

        yield return new WaitForSeconds(2.0f);

        SceneLoader.Load_Bus_Scene();
    }

    IEnumerator QTE_Timer() {
        while (true) {
            if (last_apm > 20) {
                yield break;
            }

            print("APM in the last 5 seconds: " + last_apm);
            last_apm = 0;

            yield return new WaitForSeconds(5f);
        }
    }

    IEnumerator Shake_Player() {
        Vector3 original_player_position = player.transform.position;

        while (true) {
            yield return new WaitUntil(() => last_apm != 0);

            float threshold = last_apm * 0.3f;

            player.transform.position = new Vector2(
                original_player_position.x + Random.Range(-threshold, threshold),
                original_player_position.y + Random.Range(-threshold, threshold)
            );

            for (int i = 0; i < 2; i++)
                yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Player_Enter_Bus() {
        float time = 0.5f;
        player.transform.DOMove(bus.inside_bus.position, time);
        yield return new WaitForSeconds(time / 2);
        player.transform.SetParent(bus.transform);
        player.transform.SetSiblingIndex(0);
        yield return new WaitForSeconds(time / 2);
    }
}
