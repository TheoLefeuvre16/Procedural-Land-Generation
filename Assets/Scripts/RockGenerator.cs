using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockGenerator : MonoBehaviour
{
    void Start()
    {
        InvokeRepeating("AddRockRoutine", 1.0f, 3.0f);
    }

    void AddRockRoutine()
    {
        RessourcesManager.Instance.rock += 1;

    }
}
