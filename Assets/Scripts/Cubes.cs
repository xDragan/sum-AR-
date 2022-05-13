using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Cubes : MonoBehaviour
{
    [SerializeField] private GameObject[] cubes;
    public static Controller cc;

    IEnumerator Start()
    {
        foreach (GameObject go in cubes)
        {
            go.GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1);
        }
        yield return new WaitForSeconds(2f);

    }

    public void EnableCubes(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            cubes[i].SetActive(true);
            cubes[i].transform.DOScale(0.06f, 3f);
        }
    }


}
