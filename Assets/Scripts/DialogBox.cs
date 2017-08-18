using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogBox : MonoBehaviour {

	[SerializeField]
	TextMeshProUGUI dialogText;
	[SerializeField]
	AudioManager audio;

	public GameObject dialogBox;

	bool skip_display = false;
	bool next_dialog = false;
	bool text_running = false;
	bool dialog_active = false;

	void Start() {
		audio = AudioManager.Get_Audio_Manager();
		dialogBox.SetActive(false);
	}

	public IEnumerator Text() {
		dialogBox.SetActive(true);

		List<string> aux = new List<string> {
			"foi mal meu chapa",
			"tenta sentar mais cedo da prox. vez" 
		};

		for (int i = 0; i < aux.Count; i++) {
			yield return Display_String(aux[i], 3);
			yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.F));
		}

		yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.F));
		dialogBox.SetActive(false);
	}
	
	IEnumerator Display_String(string text, int speed) {
		int current_character = 0;
		text_running = true;

		while (true) {
			if (current_character == text.Length ||
				skip_display) {
				break;
			}

			dialogText.text = text.Substring(0, current_character++) + "<color=#00000000>a</color>";
			audio.Play(audio.shake_f_sound, 0.4f);			
			yield return HushPuppy.WaitForEndOfFrames(speed);
		}

		skip_display = false;
		text_running = false;
		dialogText.text = text;
	}
}
