using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SumaButton : MonoBehaviour
{
    [SerializeField] int index;

    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void ClickButton()
    {
        anim.Play("AwnserClickButton");
        Controller.Instance.CheckAwnser(index);
    }
}
