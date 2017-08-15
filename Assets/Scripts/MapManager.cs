using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour {

    public GameObject map_window;
    public GameObject station_prefab;
    public GameObject connection_prefab;
    public List<GameObject> routeMap = new List<GameObject>();

    private bool window_active = false;

    void Awake() {
        //routeMap.Add(station_prefab);
        //for (int i = 0; i < number_of_stations; i++) {
        //    routeMap.Add(connection_prefab);
        //    routeMap.Add(station_prefab);
        //}
        //Grid.printList(routeMap);
    }

    public void _openCloseMap() {
        bool aux;
        if (!window_active) {
            aux = true;
        } else {
            aux = false;
        }
        window_active = aux;
        map_window.SetActive(aux);
    }

    public void attMap(int index) {
        print("index #" + index + " on routeMap is a " + routeMap[index]);
        if (index > 0) {
            routeMap[index - 1].transform.GetChild(0).gameObject.SetActive(false);
        }
        routeMap[index].transform.GetChild(0).gameObject.SetActive(true);
        print("==> " + routeMap[index].transform.GetChild(0).gameObject.name);
    }

}
