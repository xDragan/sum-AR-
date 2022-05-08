using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Controller : MonoBehaviour
{
    public struct Operation
    {
        public int x, y, z;
        public Operation Addition(int minResult, int maxResult)
        {
            Operation op = new Operation();
            op.z = Random.Range(minResult, maxResult + 1);
            op.x = Random.Range(minResult, op.z);
            op.y = op.z - op.x;
            return op;
        }
    }
    [SerializeField] private TextMeshProUGUI[] awnser;
    [SerializeField] private TextMeshProUGUI OPT1, OPT2;
    Operation op;
    int correctID;

    private void Awake()
    {
        Cubes.cc = this;
    }

    void Start()
    {
        NewOp();
    }

    public void NewOp()
    {
        op = new Operation();
        op = op.Addition(1,9);

        OPT1.text = op.x.ToString();
        OPT2.text = op.y.ToString();

        correctID = Random.Range(0, 3);
        
        for(int i = 0; i < 3; i++)
        {
            var tmp = Random.Range(2, 19);
            while(tmp == op.z)
                tmp = Random.Range(2, 19);

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

    void Update()
    {
        
    }


}
