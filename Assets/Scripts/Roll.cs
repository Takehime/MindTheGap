using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roll : MonoBehaviour {

	public RectTransform rect;
	public bool ended = false;
	
	void Update () {
		this.transform.position += new Vector3(0, Time.deltaTime * 20);
		if (this.transform.position.y > 1800) {
			ended = true;
		}
	}
}