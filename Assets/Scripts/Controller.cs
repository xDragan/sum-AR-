using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]
public class Controller : MonoBehaviour
{
    public static Controller Instance = null;

    public struct Operation
    {
        public int x, y, z;
        public Operation Addition(int x, int y)
        {
            Operation op = new Operation();
            op.x = x;
            op.y = y;
            op.z = op.y + op.x;
            return op;
        }
    }

    [Header("Result")]
    [SerializeField] private GameObject win;
    [SerializeField] private GameObject lose;

    [Header("Audio")]
    [SerializeField] public AudioSource speaker;
    [SerializeField] public AudioClip[] numbers;
    [SerializeField] public AudioClip sum;
    [SerializeField] public AudioClip audioWin;
    [SerializeField] public AudioClip audioLose;


    [Header("Canvas")]
    [SerializeField] private TextMeshProUGUI[] awnser;
    [SerializeField] private TextMeshProUGUI firstNumber, secondNumber, sign, endMessage;

    [Header("Images")]
    [SerializeField] private GameObject[] prefabs;
    [SerializeField] ARTrackedImageManager m_TrackedImageManager;
    [SerializeField] private Camera arCamera;

    Dictionary<Guid, GameObject> m_Instantiated = new Dictionary<Guid, GameObject>();
    Dictionary<Guid, String> m_InstantiatedName = new Dictionary<Guid, String>();
    HashSet<Guid> m_Cards = new HashSet<Guid>();
    List<int> indexPrefabs;

    private int numberCounter;
    private ParticleSystem confetti;

    Operation op;
    int correctID;
    int contador = 0;
    int lastSum = 0;
    bool flag;

    int lastNumb1, lastNumb2;

    private void Awake()
    {
        flag = false;
        Instance = this;
        indexPrefabs = Utils.RandomListOfInt(0, prefabs.Length, 9);
        confetti = win.GetComponentInChildren<ParticleSystem>();
        lastNumb1 = lastNumb2 = -1;
        numberCounter = 0;
    }

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    private void Update()
    {
        if (win.activeSelf && confetti)
        {
            confetti.transform.position = arCamera.transform.position + arCamera.transform.forward * 1.0f +  arCamera.transform.up * 0.7f;
        }
    }

    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    IEnumerator ClearText()
    {
        yield return new WaitForSeconds(2);
        lose.SetActive(false);
    }

    public void NewOp(int x, int y)
    {
        if (x + y == lastSum)
        {
            return;
        }

        op = new Operation();
        op = op.Addition(x,y);

        lastSum = op.z;

        correctID = UnityEngine.Random.Range(0, 3);
        
        for(int i = 0; i < 3; i++)
        {
            var tmp = UnityEngine.Random.Range(2, 19);
            while (tmp == op.z)
            {
                tmp = UnityEngine.Random.Range(2, 19);
            }

            awnser[i].text = tmp.ToString();
        }
        awnser[correctID].text = op.z.ToString();
    }

    public void CheckAwnser(int id)
    {
        if (id == correctID)
        {
            win.SetActive(true);
            speaker.PlayOneShot(audioWin);
        }
        else
        {
            lose.SetActive(true);
            speaker.PlayOneShot(audioLose);
            StartCoroutine(ClearText());
        }
    }


    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventAR)
    {
        if (eventAR.added.Count > 0)
        {

            foreach (var trackedImage in eventAR.added)
            {
                if (!m_Instantiated.TryGetValue(trackedImage.referenceImage.guid, out var p))
                {
                    var prefabIndex = indexPrefabs[GetNumber(trackedImage.referenceImage.name) - 1];
                    var prefab = Instantiate(prefabs[prefabIndex], trackedImage.transform);
                    var cubes = prefab.GetComponent<ObjectMesh>();
                    prefab.SetActive(true);
                    cubes.CreateCubes(GetNumber(trackedImage.referenceImage.name));
                    m_Instantiated[trackedImage.referenceImage.guid] = cubes.gameObject;
                }
                m_InstantiatedName[trackedImage.referenceImage.guid] = trackedImage.referenceImage.name;
                m_Cards.Add(trackedImage.referenceImage.guid);
            }
        }

        if (eventAR.updated.Count > 0)
        {
                foreach (var trackedImage in eventAR.updated)
                {
                    if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                    {
                        if (m_Instantiated.TryGetValue(trackedImage.referenceImage.guid, out var p))
                        {
                            p.SetActive(true);
                            var cubes = p.GetComponent<ObjectMesh>();
                            cubes.EnableCubes();
                        }
                        else
                        {
                            var prefabIndex = indexPrefabs[GetNumber(trackedImage.referenceImage.name) - 1];
                            var prefab = Instantiate(prefabs[prefabIndex], trackedImage.transform);
                            var cubes = prefab.GetComponent<ObjectMesh>();
                            prefab.SetActive(true);
                            cubes.CreateCubes(GetNumber(trackedImage.referenceImage.name));
                            m_Instantiated[trackedImage.referenceImage.guid] = cubes.gameObject;

                        }
                        m_Cards.Add(trackedImage.referenceImage.guid);
                    } else
                    {
                        if (m_Instantiated.TryGetValue(trackedImage.referenceImage.guid, out var p))
                        {
                            var cubes = p.GetComponent<ObjectMesh>();
                            cubes.DisableCubes();
                            if (p.activeSelf) {
                                UnTellNumber();
                                p.SetActive(false);
                            }
                        }
                        m_Cards.Remove(trackedImage.referenceImage.guid);

                    }
                }
        }
        ControlTexts();
    }


    private void ControlTexts()
    {
        var guids = m_Cards.ToList();
        if (guids.Count == 0)
        {
            CleanMarcadores();
            return;
        }
        else if (guids.Count == 1)
        {
            CleanMarcadores();
            sign.text = m_InstantiatedName[guids[0]].Substring(6);
        }
        else
        {
            if(lastNumb1 == -1 || lastNumb2 == -1)
            {
                ToggleButton(true);
                var numbers = getNumbers();
                firstNumber.text = numbers[0].ToString();
                secondNumber.text = numbers[1].ToString();
                lastNumb1 = numbers[0];
                lastNumb2 = numbers[1];
                sign.text = "+";
                NewOp(lastNumb1, lastNumb2);

            }
        }
    }

    private int[] getNumbers()
    {
        var guids = m_Cards.ToList();
        var numbers = new int[guids.Count];

        var textoIzquierda = m_InstantiatedName[guids[0]];
        var textoDerecha = m_InstantiatedName[guids[1]];
        if (isLeft(guids[1], guids[0]))
        {
            textoIzquierda = m_InstantiatedName[guids[1]];
            textoDerecha = m_InstantiatedName[guids[0]];
        }
        numbers[0] = GetNumber(textoIzquierda);
        numbers[1] = GetNumber(textoDerecha);
        return numbers;
    }

    IEnumerator AudioNumber(int n1, int n2)
    {
        if(!flag)
        {
            flag = true;
            yield return new WaitForSeconds(1.0f);
            speaker.PlayOneShot(numbers[n1 - 1]);
            yield return new WaitForSeconds(0.65f);
            speaker.PlayOneShot(sum);
            yield return new WaitForSeconds(0.65f);
            speaker.PlayOneShot(numbers[n2 - 1]);
        }

    }

    public void TellNumber(int n1)
    {
        speaker.PlayOneShot(numbers[n1 - 1]);
        numberCounter++;
        if (numberCounter == 2)
        {
            var numbers = getNumbers();
            StartCoroutine(AudioNumber(numbers[0], numbers[1]));
        }
    }

    public void UnTellNumber()
    {
        numberCounter--;
    }

    private void CleanMarcadores()
    {
        awnser[0].text = "";
        awnser[1].text = "";
        awnser[2].text = "";
        lastSum = 0;
        firstNumber.text = "";
        secondNumber.text = "";
        sign.text = "";
        endMessage.text = "";
        ToggleButton(false);
        lastNumb1 = lastNumb2 = -1;
        win.SetActive(false);
        lose.SetActive(false);
        flag = false;
    }

    private void ToggleButton(bool active)
    {
        awnser[0].transform.parent.transform.parent.gameObject.SetActive(active);
        awnser[0].transform.parent.gameObject.SetActive(active);
        awnser[1].transform.parent.gameObject.SetActive(active);
        awnser[2].transform.parent.gameObject.SetActive(active);
        awnser[0].gameObject.SetActive(active);
        awnser[1].gameObject.SetActive(active);
        awnser[2].gameObject.SetActive(active);
    }

    bool isLeft(Guid guid0, Guid guid1)
    {
        var position0 = m_Instantiated[guid0].transform.position;
        var position1 = m_Instantiated[guid1].transform.position;
        return position0.x < position1.x;
    }

    private int GetNumber(String name)
    {
        String number = name.Substring(name.Length - 1);
        return Int16.Parse(number);
    }

}
