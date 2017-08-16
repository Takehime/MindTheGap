using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class IntroVTVOM : MonoBehaviour {

    [SerializeField]
    GameObject VTVOM_screen;
    [SerializeField]
    TextMeshProUGUI destination;
    [SerializeField]
    TextMeshProUGUI estimated_arrival;
    [SerializeField]
    TextMeshProUGUI clock_text;

    public int times_checked = 0;
    
	// Use this for initialization
	void Start () {
        Initialize();
	}
	
	// Update is called once per frame
	void Update () {
        Handle_Clock();
	}

    void Handle_Clock() {
        var now = System.DateTime.Now;
        string hour = "", minute = "";

        hour = now.Hour.ToString();
        minute = now.Minute.ToString();
        
        if (hour.Length == 0) hour.Insert(0, "0");
        if (minute.Length == 0) minute.Insert(0, "0");

		clock_text.text = hour + ":" + minute;
    }

    void Initialize() {
        destination.text = "UNIVERSIDADE";
        estimated_arrival.text = "1 min";
    }

    void Increase_ETA() {
        float current_time = float.Parse(estimated_arrival.text.Split(' ')[0]);
        estimated_arrival.text = Mathf.Round(current_time * Random.Range(1.8f, 2.2f)) + " min";
    }

    public void Toggle_VTVOM(bool value) {
        bool currently_active = VTVOM_screen.activeSelf;

        VTVOM_screen.SetActive(!currently_active);

        //was activated
        if (!currently_active) {
            times_checked++;
            if (times_checked > 4)
                Increase_ETA();
        }
    }
}
