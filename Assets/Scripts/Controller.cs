using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(ARTrackedImageManager))]
public class Controller : MonoBehaviour
{
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
    [SerializeField] private TextMeshProUGUI OPT1, OPT2;
    [SerializeField] private GameObject prefabBase;
    [SerializeField] private Camera cameraAR;

    Dictionary<Guid, GameObject> m_Instantiated = new Dictionary<Guid, GameObject>();
    Dictionary<Guid, String> m_InstantiatedName = new Dictionary<Guid, String>();
    HashSet<String> m_Cards = new HashSet<String>();

    [SerializeField] ARTrackedImageManager m_TrackedImageManager;

    Operation op;
    int correctID;
    int contador = 0;
    bool firstTime = false;

    private void Awake()
    {
        Cubes.cc = this;
    }

    void Start()
    {
    }

    void OnEnable()
    {
        m_TrackedImageManager.trackedImagesChanged += OnTrackedImagesChanged;
    }

    
    void OnDisable()
    {
        m_TrackedImageManager.trackedImagesChanged -= OnTrackedImagesChanged;
    }

    public void NewOp(int x, int y)
    {
        op = new Operation();
        op = op.Addition(x,y);


        correctID = UnityEngine.Random.Range(0, 3);
        
        for(int i = 0; i < 3; i++)
        {
            var tmp = UnityEngine.Random.Range(2, 19);
            while(tmp == op.z)
                tmp = UnityEngine.Random.Range(2, 19);

            awnser[i].text = tmp.ToString();
        }
        awnser[correctID].text = op.z.ToString();
    }

    public void CheckAwnser(int id)
    {
        if (id == correctID)
            Debug.Log("CORRECT AWNSER!!!");
        else
            Debug.Log("FAILED AWNSER!!!");

        
    }

    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventAR)
    {
        contador++;
        String texto = "";

        bool change = true;
        if (eventAR.added.Count > 0)
        {
            texto += "A:";
            change = true;

            foreach (var trackedImage in eventAR.added)
            {
                if (!m_Instantiated.TryGetValue(trackedImage.referenceImage.guid, out var p))
                {
                    var prefab = Instantiate(prefabBase, trackedImage.transform);
                    var cubes = prefab.GetComponent<Cubes>();
                    cubes.EnableCubes(GetNumber(trackedImage.referenceImage.name));
                    m_Instantiated[trackedImage.referenceImage.guid] = cubes.gameObject;
                    prefab.SetActive(true);
                }
                m_InstantiatedName[trackedImage.referenceImage.guid] = trackedImage.referenceImage.name;
                m_Cards.Add(trackedImage.referenceImage.name);
                texto += "-" + trackedImage.referenceImage.name;
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
                    if (IsVisible(planes, trackedImage))
                    {
                        texto += "--U:";
                        if (trackedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                        {
                            if (m_Instantiated.TryGetValue(trackedImage.referenceImage.guid, out var p))
                            {
                                p.SetActive(true);
                            }
                            else
                            {
                                var prefab = Instantiate(prefabBase, trackedImage.transform);
                                var cubes = prefab.GetComponent<Cubes>();
                                cubes.EnableCubes(GetNumber(trackedImage.referenceImage.name));
                                m_Instantiated[trackedImage.referenceImage.guid] = cubes.gameObject;
                                prefab.SetActive(true);

                            }
                            m_Cards.Add(trackedImage.referenceImage.name);
                        } else
                        {
                            texto += "--D:";
                            if (m_Instantiated.TryGetValue(trackedImage.referenceImage.guid, out var p))
                            {
                                p.SetActive(false);
                            }
                            m_Cards.Remove(trackedImage.referenceImage.name);

                        }
                    }
                    else
                    {
                        texto += "--D:";
                        if (m_Instantiated.TryGetValue(trackedImage.referenceImage.guid, out var p))
                        {
                            p.SetActive(false);
                        }
                        m_Cards.Remove(trackedImage.referenceImage.name);

                    }
                    texto += "-" + trackedImage.referenceImage.name + "-" + trackedImage.trackingState.ToString();
                    change = true;
                }
            }catch (Exception ex)
            {
                texto = "Error: " + ex.Message;
            }
        }


        change = true;
        if (change)
        {
            ControlTexts(change, contador + "::" + texto);
        }

    }

    private bool IsVisible(Plane[] planes, ARTrackedImage trackedImage)
    {
        return GeometryUtility.TestPlanesAABB(planes, GetBoundsFromImage(trackedImage));
    }

    private Bounds GetBoundsFromImage(ARTrackedImage trackedImage)
    {

        Bounds bounds = new Bounds(trackedImage.transform.position, new Vector3(trackedImage.size.x, 0, trackedImage.size.y));
        return bounds;
    }

    private void ControlTexts(bool change, String texto)
    {
        //CleanMarcadores();
        //OPT1.text = texto;
        var textos = m_Cards.ToList(); 

        if (m_Cards.Count < 2)
        {
            firstTime = false;
            CleanMarcadores();
            if (m_Cards.Count > 0)
            {
                OPT1.text = textos[0].Substring(6);
            }
        }
        else
        {
            OPT1.text = textos[0].Substring(6);
            OPT2.text = textos[1].Substring(6);
            if (change && !firstTime)
            {
                firstTime = true;
                NewOp(GetNumber(textos[0]), GetNumber(textos[1]));
            }
        }
    }

    private void CleanMarcadores()
    {
        awnser[0].text = "";
        awnser[1].text = "";
        awnser[2].text = "";

        OPT1.text = "";
        OPT2.text = "";
    }

    private int GetNumber(String name)
    {
        String number = name.Substring(6);
        return Int16.Parse(number);
    }

}
