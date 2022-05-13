using System.Collections;
using UnityEngine;
using DG.Tweening;

public class ObjectMesh : MonoBehaviour
{

    private const float INITIAL_SCALE = 0.005f;
    private const float FINAL_SCALE = 0.03f;
    [SerializeField] private GameObject prefab;
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
    IEnumerator Start()
    {
        foreach (GameObject go in positions)
        {
            go.transform.localScale = Vector3.one * INITIAL_SCALE;
            go.SetActive(false);
        }
        yield return new WaitForSeconds(0.5f);
    }

    public void CreateCubes(int amount)
    {
        StartCoroutine(RoutineCreateCubes(amount));
    }

    public IEnumerator RoutineCreateCubes(int amount)
    {
        yield return new WaitForSeconds(0.5f);
        numberOfCubes = amount;
        for (int i = 0; i < amount; i++)
        {
            positions[i].SetActive(true);
            var og = Instantiate(prefab, positions[i].transform);
            og.transform.Rotate(initialRotation);
            og.transform.localPosition += initialTranslation;
            positions[i].transform.DOScale(finalScale, 3f);
        }
        visible = true;
    }

    public void EnableCubes()
    {
        if (!visible)
        {
            for (int i = 0; i < numberOfCubes; i++)
            {
                positions[i].transform.DOScale(finalScale, 3f);
            }
            visible = true;
        }
    }

    public void DisableCubes()
    {
        if (visible)
        {
            for (int i = 0; i < numberOfCubes; i++)
            {
                positions[i].transform.DOScale(INITIAL_SCALE, 1f);
            }
            visible = false;
        }
    }

}
