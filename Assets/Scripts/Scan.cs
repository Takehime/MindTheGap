using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Scan : MonoBehaviour
{
    public Image scan_mask;
    public GameObject outer_window_info;
    public GameObject inner_window_info;
    public Sprite estudante_info;
    public Sprite idoso_info;
    public Sprite turista_info;
    public Sprite varejista_info;
    
    private bool scan_is_active;
    private GameObject curr_scan_window;
    private Grid grid_ref;

    void Start()
    {
        grid_ref = FindObjectOfType<Grid>();
    }

    void Update()
    {
        checkIfScanWasActivated();
    }

    void checkIfScanWasActivated()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (!scan_is_active)
                enterScanMode();
            else
                leaveScanMode();
        }
    }

    void enterScanMode()
    {
        scan_mask.enabled = true;
        scan_is_active = true;
        grid_ref.scan_mode_active = true;
    }

    void leaveScanMode()
    {
        scan_mask.enabled = false;
        scan_is_active = false;
        grid_ref.scan_mode_active = false;

        //se existir janela de scan ativa, destroi ela
        if (curr_scan_window != null)
            Destroy(curr_scan_window);
    }

    public void scanPassenger(int tile_id, PassengerData.PassengerType p_type)
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

    void spawnOuterInfoWindow(int tile_id, PassengerData.PassengerType p_type)
    {
        curr_scan_window = Instantiate(outer_window_info);
        curr_scan_window.transform.SetParent(gameObject.transform, false);
        setGameObjectPosition(curr_scan_window, tile_id, 1.3f);
        setGameObjectScale(curr_scan_window, tile_id, 1);
        spawnInnerInfoWindow(tile_id, p_type);
    }

    void setImageByPassengerType(GameObject go, PassengerData.PassengerType p_type)
    {
        switch (p_type)
        {
            case PassengerData.PassengerType.TURISTA:
                go.GetComponent<Image>().sprite = turista_info;
                break;
            case PassengerData.PassengerType.IDOSO:
                go.GetComponent<Image>().sprite = idoso_info;
                break;
            case PassengerData.PassengerType.ESTUDANTE:
                go.GetComponent<Image>().sprite = estudante_info;
                break;
            case PassengerData.PassengerType.VAREJISTA:
                go.GetComponent<Image>().sprite = varejista_info;
                break;
        }
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

    void spawnInnerInfoWindow(int tile_id, PassengerData.PassengerType p_type)
    {
        GameObject child = Instantiate(inner_window_info);
        child.transform.SetParent(curr_scan_window.transform, false);
        child.transform.localPosition = new Vector3(0, 0, 0);
        setGameObjectScale(child, tile_id, 0.5f);
        setImageByPassengerType(child, p_type);
    }

}
