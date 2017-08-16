using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using DG.Tweening;

public class Intro_PassengerStation : MonoBehaviour {
	[SerializeField]
	Image image;
	[SerializeField]
	bool on_queue;

	public RectTransform rect;
	public List<PassengerData> possible_passengers = new List<PassengerData>();

	void Start () {
		Initialize_Random_Color();
	}
	
	void Initialize_Random_Color() {
		image.color = possible_passengers[Random.Range(0, possible_passengers.Count)].color;
	}
}