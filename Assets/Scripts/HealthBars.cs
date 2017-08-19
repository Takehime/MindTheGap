using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthBars : MonoBehaviour {

	public RectTransform bar_decrease;
	public RectTransform bar_increase;

    bool started_shaking = false;
    Coroutine shaking = null;
    float threshold = 0.025f;
    public bool is_shaking = false;

	void Update() {
        is_shaking = shaking != null;

        bar_decrease.sizeDelta = new Vector2(
            bar_decrease.rect.width,
            bar_decrease.rect.height - Time.deltaTime / 2f
        );

        if (bar_decrease.sizeDelta.y < 0 && !started_shaking) {
            started_shaking = true;
            shaking = StartCoroutine(Shake_Player());
        }

        bar_increase.sizeDelta = new Vector2(
            bar_increase.rect.width,
            bar_increase.rect.height + Time.deltaTime / 2f
        );
	}

    public IEnumerator Pause_Shake_Player(float duration) {
        if (!started_shaking) {
            yield break;
        }
        
        Stop_Shake_Player();

        yield return new WaitForSeconds(duration);

        shaking = StartCoroutine(Shake_Player());
    }

    public void Stop_Shake_Player() {
        if (shaking == null) {
            return;
        }

        StopCoroutine(shaking);
        shaking = null;
    }

    public Vector3 original_player_position;

    IEnumerator Shake_Player() {
        GameObject target = GameObject.FindGameObjectWithTag("Player");
        original_player_position = target.transform.position;

        while (true) {
            target.transform.position = new Vector2(
                original_player_position.x + Random.Range(-threshold, threshold),
                original_player_position.y + Random.Range(-threshold, threshold)
            );

            for (int i = 0; i < 2; i++)
                yield return new WaitForEndOfFrame();
        }
    }
}
