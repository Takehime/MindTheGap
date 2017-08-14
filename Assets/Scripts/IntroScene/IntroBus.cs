using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroBus : MonoBehaviour {

    public Transform inside_bus;
    public bool is_bus_at_platform = false;

    Animator anim;
    bool end_animation = false;

    private void Start() {
        anim = GetComponentInChildren<Animator>();
    }

    public void Anim_End() {
        end_animation = true;
    }

    public IEnumerator Exit_Scene() {
        anim.SetTrigger("exit");
        yield return End_Current_Animation();

        is_bus_at_platform = false;
        this.gameObject.SetActive(false);
    }

    public IEnumerator Enter_Scene() {
        anim.SetTrigger("enter");
        yield return End_Current_Animation();

        is_bus_at_platform = true;
    }

    IEnumerator End_Current_Animation() {
        yield return new WaitUntil(() => end_animation);
        end_animation = false;
    }
}
