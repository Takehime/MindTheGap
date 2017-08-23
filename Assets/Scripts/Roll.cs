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
	public GameObject target;

	public float time = 8.4f;

	void Start() {
		logo.DOFade(1, time);
		background.DOFade(1, time);
	}
	
	void End_Animation() {
		ended = true;
	}
}