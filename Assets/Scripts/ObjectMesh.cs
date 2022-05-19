using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ObjectMesh : MonoBehaviour
{

    private const float INITIAL_SCALE = 0.005f;
    private const float FINAL_SCALE = 0.03f;
    [SerializeField] private GameObject model;
    [SerializeField] private GameObject[] positions;
    [SerializeField] private float finalScale;
    [SerializeField] private Vector3 initialRotation;
    [SerializeField] private Vector3 initialTranslation;

    private int numberOfCubes;
    private bool visible = false;


    public void Awake()
    {
        if (finalScale == 0.0)
        {
            finalScale = FINAL_SCALE;
        }
    }
    void Start()
    {
        foreach (GameObject go in positions)
        {
            go.transform.localScale = Vector3.one * INITIAL_SCALE;
            go.SetActive(false);
        }
    }

    public void CreateFigures(int amount)
    {
        StartCoroutine(RoutineCreateFigures(amount));
    }

    public IEnumerator RoutineCreateFigures(int amount)
    {
        yield return new WaitForSeconds(0.5f);
        numberOfCubes = amount;
        for (int i = 0; i < amount; i++)
        {
            positions[i].SetActive(true);
            var og = Instantiate(model, positions[i].transform);
            og.transform.Rotate(initialRotation);
            og.transform.localPosition += initialTranslation;
        }
        visible = false;
        EnableFigures();
    }

    public void EnableFigures()
    {
        if (!visible)
        {
            StartCoroutine(EnableFigure());
        }
    }

    private IEnumerator EnableFigure()
    {
        visible = true;
        for (int i = 0; i < numberOfCubes; i++)
        {
            positions[i].transform.DOScale(finalScale, 1f);
            yield return new WaitForSeconds(1.0f);
        }
        Controller.Instance.TellNumber(numberOfCubes);
    }

    public void DisableFigures()
    {
        if (visible)
        {
            for (int i = 0; i < numberOfCubes; i++)
            {
                positions[i].transform.DOScale(INITIAL_SCALE, 0.1f);
            }
            visible = false;
        }
    }
}
