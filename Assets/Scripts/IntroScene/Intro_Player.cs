using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro_Player : MonoBehaviour {

	[SerializeField]
	Rigidbody2D rb;
	[SerializeField]
	float speed = 10f;

	void Update () {
		Handle_Movement();		
		Handle_Direction();
	}

	void Handle_Movement() {
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		rb.velocity = new Vector2(horizontal, vertical) * speed;
	}

	void Handle_Direction() {
		if (rb.velocity == Vector2.zero) {
			return;
		}

		Vector3 scale = this.GetComponent<RectTransform>().localScale;

		this.GetComponent<RectTransform>().localScale = new Vector3(
			(rb.velocity.x < 0 ? 1 : -1) * Mathf.Abs(scale.x),
			scale.y,
			scale.z
		);
	}
}
