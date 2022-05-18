using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SumaButton : MonoBehaviour
{
    [SerializeField] int index;

    private Animator anim;
    public static Controller cc;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ClickButton()
    {
        anim.Play("AwnserClickButton");
        cc.speaker.PlayOneShot(cc.numbers[0]);
        Controller.Instance.CheckAwnser(index);
    }
}
