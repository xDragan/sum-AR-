using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class MenuSUMAR : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private ARSession arSession;
    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private GameObject button;



    IEnumerator Start()
    {
        errorMessage.text = "";
        if ((ARSession.state == ARSessionState.None) ||
            (ARSession.state == ARSessionState.CheckingAvailability))
        {
            yield return ARSession.CheckAvailability();
        }

        if (ARSession.state == ARSessionState.Unsupported)
        {
            errorMessage.text = "ERROR - El dispositivo no acepta AR";
            button.SetActive(false);
        }
        else
        {
            // Start the AR session
            arSession.enabled = true;
            button.SetActive(true);
        }
    }


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

