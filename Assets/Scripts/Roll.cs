using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class Roll : MonoBehaviour {

	public RectTransform rect;
	public bool ended = false;
	public Image logo;
	public Image background;

	public float time = 8.4f;

	void Start() {
		logo.DOFade(1, time);
		background.DOFade(1, time);
		StartCoroutine(Start_Rolling());
	}

	bool can_roll = false;

	void Update () {
		if (!can_roll) return;
		rect.gameObject.transform.position += new Vector3(0, Time.deltaTime * 30);
		if (this.transform.position.y > 1800) {
			ended = true;
		}
	}

	IEnumerator Start_Rolling() {
		yield return new WaitForSeconds(time);
		can_roll = true;
	}
}