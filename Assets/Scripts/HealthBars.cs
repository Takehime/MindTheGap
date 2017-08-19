using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthBars : MonoBehaviour {

	public RectTransform bar_decrease;
	public RectTransform bar_increase;

	void Update() {
        bar_decrease.sizeDelta = new Vector2(
            bar_decrease.rect.width,
            bar_decrease.rect.height - Time.deltaTime / 2f
        );
        bar_increase.sizeDelta = new Vector2(
            bar_increase.rect.width,
            bar_increase.rect.height + Time.deltaTime / 2f
        );
	}


}
