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
    [SerializeField] private TextMeshProUGUI[] awnser;
    [SerializeField] private TextMeshProUGUI OPT1, OPT2, ENDTEXT;
    [SerializeField] private GameObject prefabBase;
    [SerializeField] private Camera cameraAR;
    [SerializeField] private GameObject[] prefabs;

    Dictionary<Guid, GameObject> m_Instantiated = new Dictionary<Guid, GameObject>();
    Dictionary<Guid, String> m_InstantiatedName = new Dictionary<Guid, String>();
    HashSet<Guid> m_Cards = new HashSet<Guid>();
    List<int> indexPrefabs;

    [SerializeField] ARTrackedImageManager m_TrackedImageManager;

    Operation op;
    int correctID;
    int contador = 0;
    bool firstTime = false;
    int lastSum = 0;

    private void Awake()
    {
        Cubes.cc = this;
        Instance = this;
        indexPrefabs = Utils.RandomListOfInt(0, prefabs.Length, 9);
    }

    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    
    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }
    IEnumerator ClearText()
    {
        yield return new WaitForSeconds(2);
        ENDTEXT.text = "";
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
            ENDTEXT.text = "　　BIEN!!! LO LOGRASTE";
            //ENDTEXT.text = "　　BIEN!!! Lo lograste";
        }
        else
        {
            ENDTEXT.text = "NO PASA NADA, PRUEBA OTRO NUMERO";
            //ENDTEXT.text = "No pasa nada, prueba otro numero";
            StartCoroutine(ClearText());
        }
    }


    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventAR)
    {
        String texto = "";
        contador++;

        bool change = true;
        if (eventAR.added.Count > 0)
        {
            texto += "A:";
            change = true;

            try { 
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
                texto += "-" + trackedImage.referenceImage.name;
            }
            }
            catch (Exception ex)
            {
                texto = "Error: " + ex.Message;
            }
        }

        if (eventAR.updated.Count > 0)
        {
            try
            {
                change = true;
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cameraAR);
                foreach (var trackedImage in eventAR.updated)
                {
                    texto += "--U:";
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
                        texto += "--D:";
                        if (m_Instantiated.TryGetValue(trackedImage.referenceImage.guid, out var p))
                        {
                            var cubes = p.GetComponent<ObjectMesh>();
                            cubes.DisableCubes();
                            p.SetActive(false);
                        }
                        m_Cards.Remove(trackedImage.referenceImage.guid);

                    }
                    texto += "-" + trackedImage.referenceImage.name + "-" + trackedImage.trackingState.ToString();
                    change = true;
                }
            }catch (Exception ex)
            {
                texto = "Error: " + ex.Message;
            }
        }


        ControlTexts(change, contador + "::" + texto);

    }


    private void ControlTexts(bool change, String texto)
    {
        //CleanMarcadores();
        //OPT2.text = texto;
        var guids = m_Cards.ToList();
        String textoIzquierda = "";
        String textoDerecha = "";
        if (guids.Count == 0)
        {
            CleanMarcadores();
            return;
        }
        else if (guids.Count == 1)
        {
            CleanMarcadores();
            textoIzquierda = m_InstantiatedName[guids[0]];
            if (m_Instantiated[guids[0]].transform.position.x < 0)
            {
                OPT1.text = textoIzquierda.Substring(6);
            } else
            {
                OPT2.text = textoIzquierda.Substring(6);
            }
        }
        else
        {
           // ToggleButton(true);
            textoIzquierda = m_InstantiatedName[guids[0]];
            textoDerecha = m_InstantiatedName[guids[1]];
            if (isLeft(guids[1], guids[0]))
            {
                textoIzquierda = m_InstantiatedName[guids[1]];
                textoDerecha = m_InstantiatedName[guids[0]];
            }
            OPT1.text = textoIzquierda.Substring(6);
            OPT2.text = textoDerecha.Substring(6);
            NewOp(GetNumber(textoIzquierda), GetNumber(textoDerecha));
        }
    }

    private void CleanMarcadores()
    {
        awnser[0].text = "";
        awnser[1].text = "";
        awnser[2].text = "";
        lastSum = 0;
        OPT1.text = "";
        OPT2.text = "";
        ENDTEXT.text = "";
        //ToggleButton(false);
    }

    private void ToggleButton(bool active)
    {
        awnser[0].transform.parent.gameObject.SetActive(active);
        awnser[1].transform.parent.gameObject.SetActive(active);
        awnser[2].transform.parent.gameObject.SetActive(active);
    }

    bool isLeft(Guid guid0, Guid guid1)
    {
        var position0 = m_Instantiated[guid0].transform.position;
        var position1 = m_Instantiated[guid1].transform.position;
        return position0.x < position1.x;
    }

    String getCoordenada(Guid guid)
    {
        var position = m_Instantiated[guid].transform.position;
        return "(" + position.x + "," + position.y + "," + position.z + ")";
    }


    private int GetNumber(String name)
    {
        String number = name.Substring(name.Length - 1);
        return Int16.Parse(number);
    }

}
