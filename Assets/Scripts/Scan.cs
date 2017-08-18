using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Scan : MonoBehaviour
{
    public Image scan_mask;
    public GameObject outer_window_info;
    public GameObject inner_window_info;
    [Header("Sprites")]
    public List<Sprite> estudante_info;
    public List<Sprite> idoso_info;
    public List<Sprite> turista_info;
    public List<Sprite> varejista_info;
    public List<Sprite> trabalhador_info;

    private bool scan_is_active;
    private GameObject curr_scan_window;
    private Grid grid_ref;
    private List<Sprite> sprites_to_passengers = new List<Sprite>();
    private TurnManager tm;

    public AudioManager audio;

    void Start()
    {
        tm = FindObjectOfType<TurnManager>();
        audio = AudioManager.Get_Audio_Manager();
        grid_ref = FindObjectOfType<Grid>();

        for (int i = 0; i < 60; i++) {
            sprites_to_passengers.Add(null);
        }
    }

    public void _checkIfScanWasActivated()
    {
        if (!scan_is_active && (tm.curr_turn != TurnManager.Turn.AtStation || !grid_ref.alreadySwaped))
            enterScanMode();
        else
            leaveScanMode();
    }

    void enterScanMode()
    {
        audio.Play(audio.scan_enter, 0.8f);
        scan_mask.enabled = true;
        scan_is_active = true;
        grid_ref.scan_mode_active = true;
    }

    public void leaveScanMode()
    {
        if (!scan_is_active) {
            return;
        }
        
        audio.Play(audio.scan_leave, 0.8f);
        scan_mask.enabled = false;
        scan_is_active = false;
        grid_ref.scan_mode_active = false;

        //se existir janela de scan ativa, destroi ela
        if (curr_scan_window != null)
            Destroy(curr_scan_window);
    }

    public void scanPassenger(int tile_id, PassengerType p_type)
    {
        if (scan_is_active)
        {
            //TODO: deixar o passageiro targeteado sem a mask verde por cima

            //se existe janela de scan ja ativa, destroi ela antes de abrir uma nova
            checkIfWindowAlreadyExists();
            spawnOuterInfoWindow(tile_id, p_type);
        }
    }

    void checkIfWindowAlreadyExists()
    {
        if (curr_scan_window != null)
            Destroy(curr_scan_window);
    }

    void spawnOuterInfoWindow(int tile_id, PassengerType p_type)
    {
        curr_scan_window = Instantiate(outer_window_info);
        curr_scan_window.transform.SetParent(gameObject.transform, false);
        setGameObjectPosition(curr_scan_window, tile_id, 1.3f);
        setGameObjectScale(curr_scan_window, tile_id, 1);
        spawnInnerInfoWindow(tile_id, p_type);
    }

    Sprite setImageByPassengerType(GameObject go, PassengerType p_type)
    {
        int selected = -1;
        switch (p_type)
        {
            case PassengerType.TURISTA:
                selected = Random.Range(0, turista_info.Count);
                go.GetComponent<Image>().sprite = turista_info[selected];
                break;
            case PassengerType.IDOSO:
                selected = Random.Range(0, idoso_info.Count);
                go.GetComponent<Image>().sprite = idoso_info[selected];
                break;
            case PassengerType.ESTUDANTE:
                selected = Random.Range(0, estudante_info.Count);
                go.GetComponent<Image>().sprite = estudante_info[selected];
                break;
            case PassengerType.VAREJISTA:
                selected = Random.Range(0, varejista_info.Count);
                go.GetComponent<Image>().sprite = varejista_info[selected];
                break;
            case PassengerType.TRABALHADOR:
                selected = Random.Range(0, trabalhador_info.Count);
                go.GetComponent<Image>().sprite = trabalhador_info[selected];
                break;
        }
        return go.GetComponent<Image>().sprite;
    }

    void setGameObjectPosition(GameObject go, int tile_id, float offset)
    {
        Vector3 spawn_pos = grid_ref.passengers[tile_id].transform.position;
        if (tile_id < 30)
            spawn_pos -= new Vector3(0, offset, 0);
        else
            spawn_pos += new Vector3(0, offset, 0);
        go.transform.position = spawn_pos;
    }

    void setGameObjectScale(GameObject go, int tile_id, float scale)
    {
        if (tile_id < 30)
            go.transform.localScale = new Vector3(scale, scale, scale);
        else
            go.transform.localScale = new Vector3(scale, -scale, scale);
    }

    void spawnInnerInfoWindow(int tile_id, PassengerType p_type)
    {
        GameObject child = Instantiate(inner_window_info);
        child.transform.SetParent(curr_scan_window.transform, false);
        setGameObjectScale(child, tile_id, 0.5f);

        child.transform.localPosition = new Vector3(0, -5, 0);

        if (sprites_to_passengers[tile_id] == null) {
            // print("nenhum sprite foi ainda gerado pra esse passageiro");
            Sprite s = setImageByPassengerType(child, p_type);
            sprites_to_passengers[tile_id] = s;
        } else {
            child.GetComponent<Image>().sprite = sprites_to_passengers[tile_id];
        }
    }

}
