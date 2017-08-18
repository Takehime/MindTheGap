using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Pachinko : MonoBehaviour {

    [Header("References")]
    public GameObject pachinko_go;
    public GameObject passenger;
    public GameObject roulette;
    public GameObject signal;
    public TextMeshProUGUI word_sequence;
    public GameObject checkMark;

    [Header("Timers")]
    public float time_to_start_next_roulette_round;
    public float time_roulette_spot_is_selected;

    private GameObject selected;
    private Grid grid;
    private List<List<string>> correct_word_sequences = new List<List<string>>();
    private List<List<string>> generic_word_sequences = new List<List<string>>();
    private List<string> selected_sequence = new List<string>();
    private int rounds;

    public AudioManager audio;

    void Start()
    {
        grid = FindObjectOfType<Grid>();
        audio = AudioManager.Get_Audio_Manager();

        initializeCorrectWordSequences();
        initializeGenericWordSequences();
    }

    #region passenger color

    void setPassengerColor(Color color)
    {
        passenger.GetComponent<Image>().color = color;
    }

    #endregion

    #region pachinko main loop
    public void enterPachinkoMode(Color color)
    {
        pachinko_go.SetActive(true);
        setPassengerColor(color);
        setCorrectWordSequence();
        initializeWordSequenceText();
        StartCoroutine(startRouletteLoop());
    }

    void setNextRound(int value)
    {
        setNextSignal();
        setNextWordSequences(value);
    }

    IEnumerator startRouletteLoop()
    {
        for (int i = 0; i < rounds; i++)
        {
            setNextRound(i);
            Coroutine roulette_loop = StartCoroutine(rouletteLooping());
            yield return StartCoroutine(waitForKeyDown(KeyCode.F));
            StopCoroutine(roulette_loop);
            updateWordSequence();
            yield return new WaitForSeconds(time_to_start_next_roulette_round);
            resetRouletteScale();
        }
        //checkMark.SetActive(true);
        leavePachinkoMode(true);
    }

    IEnumerator waitForKeyDown(KeyCode keyCode)
    {
        yield return new WaitUntil(() => Input.GetKeyDown(keyCode));
        audio.Play(audio.pachinko_button, 0.8f);
    }

    IEnumerator rouletteLooping()
    {
        for (int i = 0; ; i++)
        {
            i = i % 4;
            selected = roulette.transform.GetChild(i).gameObject;
            selected.transform.localScale = new Vector3(1.1f, 1, 1);
            yield return new WaitForSeconds(time_roulette_spot_is_selected);
            selected.transform.localScale = new Vector3(1f, 1, 1);
        }
    }

    public void leavePachinkoMode(bool confirm)
    {
        string trigger = "";
        if (confirm) {
            audio.Play(audio.pachinko_confirmation, 0.8f);
            trigger = "unshow_confirm";
        } else {
            if (grid.pachinko_mode_active)
                audio.Play(audio.pachinko_cancel, 0.8f);
            trigger = "unshow_cancel";
        }
        grid.pachinko_mode_active = false;
        pachinko_go.GetComponentInChildren<Animator>().SetTrigger(trigger);
    }
    #endregion

    #region word management
    void setNumberOfRounds(int valor)
    {
         rounds = valor;
    }

    void initializeCorrectWordSequences()
    {
        List<string> s1 = new List<string> { "me da", "passagem", "amigo" };
        correct_word_sequences.Add(s1);
        List<string> s2 = new List<string> { "com licença", "meu chapa" };
        correct_word_sequences.Add(s2);
    }

    void initializeGenericWordSequences()
    {
        generic_word_sequences.Add(
            new List<string> {
                "com licença",
                "por favor",
                "me da uma passagem ai",
                "me da",
                "*empurra*",
                "me deixa"
            }
        );
        generic_word_sequences.Add(
            new List<string> {
                "por favor",
                "com licenca",
                "na moral",
                "*empurra*",
                "*esmaga*",
                "uma passagem",
                "licença"
            }
        );
        generic_word_sequences.Add(
            new List<string> {
                "meu chapa",
                "minha senhora",
                "meu querido",
                "minha querida",
                "meu nobre",
                "*atropela*",
                "menor",
                "senhor",
                "senhora",
                "amigo",
                "camarada"
            }
        );
    }

    void initializeWordSequenceText()
    {
        word_sequence.text = "";
    }

    void updateWordSequence()
    {
        word_sequence.text += " " + selected.transform.GetChild(0).GetComponent<Text>().text;
    }

    void setCorrectWordSequence()
    {
        List<string> selected = correct_word_sequences[(Random.Range(0, correct_word_sequences.Count))];
        selected_sequence = selected;
        setNumberOfRounds(selected.Count);
    }

    void setNextWordSequences(int index)
    {
        string correct_fragment = selected_sequence[index];
        List<string> round_words = new List<string>();
        round_words = generic_word_sequences[index];
        List<string> used = new List<string>();
        used.Add(correct_fragment);
        string generic_fragment = "";

        for (int i = 0; i < 4; i++)
        {
            GameObject roulette_element = roulette.transform.GetChild(i).gameObject;
            if (roulette_element.GetComponent<Image>().color == signal.GetComponent<Image>().color) //sequencia correta
                roulette_element.transform.GetChild(0).GetComponent<Text>().text = correct_fragment;
            else //sequencia generica
            {
                do
                    generic_fragment = round_words[Random.Range(0, round_words.Count)];
                while (used.Contains(generic_fragment));
                used.Add(generic_fragment);
                roulette_element.transform.GetChild(0).GetComponent<Text>().text = generic_fragment;
            }
        }
    }
    #endregion

    #region roulette management
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

    public void makePachinkoFaster() {
        time_roulette_spot_is_selected -= 0.2f;
        time_to_start_next_roulette_round -= 0.1f;
    }

    #endregion

    #region signal management
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
    #endregion

}
