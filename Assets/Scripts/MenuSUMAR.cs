using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSUMAR : MonoBehaviour
{
    [SerializeField] private Animator anim;


    public void StartGame()
    {
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        anim.Play("ButtonsUI");
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene("Game");
    }
}

