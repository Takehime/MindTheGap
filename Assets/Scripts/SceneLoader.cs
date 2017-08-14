using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {
    public static void Load_Bus_Scene() {
        SceneManager.LoadScene("BusScene");
    }
}
