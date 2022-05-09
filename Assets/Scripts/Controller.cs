using System;
using System.Collections;
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

    Dictionary<Guid, GameObject> m_Instantiated = new Dictionary<Guid, GameObject>();
    Dictionary<Guid, String> m_InstantiatedName = new Dictionary<Guid, String>();
    List<ARTrackedImage> m_Cards = new List<ARTrackedImage>();

    [SerializeField] ARTrackedImageManager m_TrackedImageManager;

    Operation op;
    int correctID;

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
        bool change = false;
        foreach (var trackedImage in eventAR.added)
        {
            
            if (!m_Instantiated.TryGetValue(trackedImage.referenceImage.guid, out var p))
            {
                var prefab = Instantiate(prefabBase, trackedImage.transform);
                var cubes = prefab.GetComponent<Cubes>();
                cubes.EnableCubes(GetNumber(trackedImage.referenceImage.name));
                m_Instantiated[trackedImage.referenceImage.guid] = cubes.gameObject;
            }
            m_InstantiatedName[trackedImage.referenceImage.guid] = trackedImage.referenceImage.name;
            m_Cards.Add(trackedImage);
            change = true;
        }

        foreach (var trackedImage in eventAR.removed)
        {

            if (m_Instantiated.TryGetValue(trackedImage.referenceImage.guid, out var prefab))
            {
                m_Cards.Remove(trackedImage);
                m_Instantiated[trackedImage.referenceImage.guid] = null;
                Destroy(prefab);
            }
            change = true;
        }

        ControlTexts(change);
    }

    private void ControlTexts(bool change)
    {
        if (m_Cards.Count != 2)
        {
            CleanMarcadores();
            OPT1.text = m_Cards[1].referenceImage.name.Substring(6) + " - " + m_Cards[1].trackingState.ToString();
        }
        else
        {
            OPT1.text = m_Cards[0].referenceImage.name.Substring(6) + " - " + m_Cards[0].trackingState.ToString();
            OPT2.text = m_Cards[1].referenceImage.name.Substring(6) + " - " + m_Cards[1].trackingState.ToString();
            if (change)
            {
                NewOp(GetNumber(m_Cards[0].referenceImage.name.Substring(6)), GetNumber(m_Cards[1].referenceImage.name.Substring(6)));
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
