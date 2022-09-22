using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldsGenerator : MonoBehaviour
{
    void Start()
    {
        InvokeRepeating("AddGoldsRoutine",1.0f,3.0f);
    }


    void AddGoldsRoutine()
    {
        RessourcesManager.Instance.gold += 1;

    }
}
