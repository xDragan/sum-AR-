using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SumaButton : MonoBehaviour
{
    [SerializeField] int index;


    public void ClickButton()
    {
        Controller.Instance.CheckAwnser(index);

    }
}
