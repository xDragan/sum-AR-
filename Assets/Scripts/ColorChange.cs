using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ColorChange : MonoBehaviour
{

    private Image img;
    [SerializeField] private Color firstCol;
    [SerializeField] private Color secCol;
    [SerializeField] private float changeDuration;
    [SerializeField] private int iterations;

    void Start()
    {
        img = GetComponent<Image>();
        img.color = firstCol;
        img.DOColor(secCol, changeDuration).SetEase(Ease.Linear);

    }

}
