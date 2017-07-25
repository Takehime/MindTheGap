using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pachinko : MonoBehaviour {

    [Header("References")]
    public GameObject pachinko_go;
    public GameObject passenger;
    public GameObject roulette;
    public GameObject signal;

    private GameObject selected;
    private Grid grid;

    void Start()
    {
        grid = FindObjectOfType<Grid>();
    } 

    public void enterPachinkoMode(Color color)
    {
        pachinko_go.SetActive(true);
        setPassengerColor(color);
        StartCoroutine(startRouletteLoop());
    }

    void setPassengerColor(Color color)
    {
        passenger.GetComponent<Image>().color = color;
    }

    void setNextSignal()
    {
        List<Color> possible_colors = setListOfColorsFromRouletteColors();
        Color signal_color = possible_colors[Random.Range(0, possible_colors.Count)];
        signal.GetComponent<Image>().color = signal_color;
    }

    List<Color> setListOfColorsFromRouletteColors()
    {
        List<Color> roulette_colors = new List<Color>();
        for (int i = 0; i < 4; i++)
        {
            Color color = roulette.transform.GetChild(i).GetComponent<Image>().color;
            roulette_colors.Add(color);
        }
        return roulette_colors;
    }

    IEnumerator startRouletteLoop()
    {
        bool success = false;
        for (int i = 0; i < 3; i++)
        {
            setNextSignal();
            Coroutine roulette_loop = StartCoroutine(rouletteLooping());
            yield return StartCoroutine(waitForKeyDown(KeyCode.Space));
            StopCoroutine(roulette_loop);
            bool isCorrect = checkIfSelectedIsCorrect();
            if (isCorrect)
            {
                Debug.Log("sinal certo");
                success = true;
            }
            else
            {
                Debug.Log("sinal errado");
                yield return new WaitForSeconds(3f);
                resetRouletteScale();
                success = false;
                //cancela o swap (ver com o vinicius o que deve acontecer exatamente)
                break;
            }
            yield return new WaitForSeconds(2f);
            resetRouletteScale();
        }
        leavePachinkoMode(success);
    }

    IEnumerator waitForKeyDown(KeyCode keyCode)
    {
        while (!Input.GetKeyDown(keyCode))
            yield return null;
    }

    IEnumerator rouletteLooping()
    {
        for (int i = 0; ; i++)
        {
            i = i % 4;
            selected = roulette.transform.GetChild(i).gameObject;
            selected.transform.localScale = new Vector3(1.1f, 1, 1);
            yield return new WaitForSeconds(0.5f);
            selected.transform.localScale = new Vector3(1f, 1, 1);
        }
    }

    void resetRouletteScale()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject aux = roulette.transform.GetChild(i).gameObject;
            aux.transform.localScale = new Vector3(1f, 1, 1);
        }
    }
    bool checkIfSelectedIsCorrect()
    {
        Color roulette_color = selected.GetComponent<Image>().color;
        Color signal_color = signal.GetComponent<Image>().color;
        if (roulette_color == signal_color)
            return true;
        else
            return false;
    }

    void leavePachinkoMode(bool success)
    {
        grid.pachinko_mode_active = false;
        grid.pachinko_success = success;
        pachinko_go.SetActive(false);
    }

}
