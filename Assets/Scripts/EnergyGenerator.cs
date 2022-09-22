using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyGenerator : MonoBehaviour
{
    void Start()
    {
        InvokeRepeating("AddGoldsRoutine", 1.0f, 3.0f);
    }


    void AddEnergyRoutine()
    {
        RessourcesManager.Instance.energy += 1;

    }
}
