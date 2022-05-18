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
        StartCoroutine(AudioNumber(1, 2));
        Controller.Instance.CheckAwnser(index);
    }

    IEnumerator AudioNumber(int n1, int n2)
    {
        cc.speaker.PlayOneShot(cc.numbers[n1]);
        yield return new WaitForSeconds(0.65f);
        cc.speaker.PlayOneShot(cc.sum);
        yield return new WaitForSeconds(0.65f);
        cc.speaker.PlayOneShot(cc.numbers[n2]);
    }
}
