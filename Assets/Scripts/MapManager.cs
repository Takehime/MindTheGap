using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public GameObject map_window;
	public GameObject info_window;
    public GameObject station_prefab;
    public GameObject connection_prefab;
    public List<GameObject> routeMap = new List<GameObject>();
    public int ending_trigger_index = 10;
	//public List<StationData> stations;

    private bool map_window_active = false;
	private bool info_window_active = false;

    public void _openCloseMap() {
        bool aux;
		if (!map_window_active) {
            aux = true;
        } else {
            aux = false;
			info_window.SetActive (false);
			info_window_active = false;
        }
		map_window_active = aux;
        map_window.SetActive(aux);
    }

    public void attMap(int index) {
        print("index #" + index + " on routeMap is a " + routeMap[index]);
        if (index > 0) {
            routeMap[index - 1].transform.GetChild(0).gameObject.SetActive(false);
        }
        routeMap[index].transform.GetChild(0).gameObject.SetActive(true);
        //print("==> " + routeMap[index].transform.GetChild(0).gameObject.name);
        if (index == ending_trigger_index) {
            print("index: " + index);
            Ending end = FindObjectOfType<Ending>();
            end.triggerEnd();
        }
    }

	public void _showStationInfo(GameObject go) {
		print ("info_window: " + info_window);
		if (!info_window_active) {
			info_window.SetActive (true);
			info_window_active = true;
		}
		StationData st = go.GetComponent<Station> ().sd;
		setInfoText (st);
	}

	void setInfoText(StationData st) {
		//StationData data = info_window.GetComponentInChildren<Station> ().sd;
		string infoText = "ESTAÇÃO: " + st.name + "\nPERFIS: ";
		string perfis = "";
		Grid.printList (st.perfis);
		for (int i = 0; i < st.perfis.Count; i++) {
			perfis += st.perfis[i];
			if (i < st.perfis.Count - 1) {
				perfis += ", ";
			}
		}
		infoText += perfis.ToLower();
		info_window.GetComponentInChildren<Text> ().text = infoText;
	}

}