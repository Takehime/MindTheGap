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
    GameObject passengers_inside_bus_container;
    [SerializeField]
    Intro_BusCountdown bus_countdown;
    [SerializeField]
    List<Intro_PassengerStation> passengers_queue = new List<Intro_PassengerStation>();
    [SerializeField]
    List<Intro_PassengerStation> passengers_inside_bus = new List<Intro_PassengerStation>();

    [Header("Logo Fade")]
    [SerializeField]
    Image blackGround;
    [SerializeField]
    Image logo;

    AudioManager audio;

    void Start () {
        audio = AudioManager.Get_Audio_Manager();

        coroutine_shake_player = StartCoroutine(Shake_Target(player.transform));
        StartCoroutine(Handle_Bus_Behaviour());
        //StartCoroutine(QTE_Timer());
        //StartCoroutine(Player_Enter_Bus());

        // bus.gameObject.SetActive(false);
        bus_countdown.gameObject.SetActive(false);
    }

    void Update () {
		if (bus.is_bus_at_platform && Input.GetKeyDown(KeyCode.F)) {
            audio.Play(audio.shake_f_sound, 0.7f);
            last_apm++;
        }
	}

    IEnumerator Put_Background_Door() {
        yield return new WaitForSeconds(0.8f);
        passengers_inside_bus_container.SetActive(true);
    }

    IEnumerator Handle_Bus_Behaviour() {
        audio.Play_Real(audio.city_noises);
        yield return new WaitUntil(() => vtvom.times_checked == 10);

        StartCoroutine(Put_Background_Door());
        audio.Play(audio.bus_stopping_fast, 0.8f);
        yield return bus.Enter_Scene();
        yield return Passengers_To_Bus();
        audio.Play_Real(audio.metal_pesado);

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
        StartCoroutine(bus.Exit_Scene());
        yield return new WaitForSeconds(2.0f);

        yield return Fade_Title();

        SceneLoader.Load_Bus_Scene();
    }

    IEnumerator QTE_Timer() {
        while (true) {
            if (last_apm > 40) {
                yield break;
            }

            last_apm = 0;

            yield return new WaitForSeconds(10f);
        }
    }

    IEnumerator Shake_Target(Transform target) {
        Vector3 original_player_position = target.transform.position;

        while (true) {
            yield return new WaitUntil(() => last_apm != 0);

            float threshold = last_apm * 0.3f;

            target.transform.position = new Vector2(
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

    IEnumerator Passengers_To_Bus() {
        float time = 0.2f;
        
        for (int i = 0; i < passengers_queue.Count; i++) {
            var tween = passengers_queue[i].rect.DOMove(
                passengers_inside_bus[i].rect.position,
                time
            );
            tween.SetEase(Ease.InBack);
            tween = passengers_queue[i].rect.DORotateQuaternion(
                passengers_inside_bus[i].rect.rotation,
                time
            );
            tween.SetEase(Ease.InBack);
        }
        yield return new WaitForSeconds(time);

        for (int i = 0; i < passengers_queue.Count; i++) {
            passengers_queue[i].transform.SetParent(passengers_inside_bus[i].transform.parent);
            StartCoroutine(Shake_Target(passengers_queue[i].transform));
        }
    }

    IEnumerator Fade_Title() {
        blackGround.gameObject.SetActive(true);
        logo.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
    }
}
