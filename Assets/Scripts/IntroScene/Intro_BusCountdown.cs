using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Intro_BusCountdown : MonoBehaviour {

    [SerializeField]
    RectTransform rect;

	void Update () {
        rect.sizeDelta = new Vector2(
            rect.rect.width,
            rect.rect.height + Time.deltaTime * 25
        );
	}
}
