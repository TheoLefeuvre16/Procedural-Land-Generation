using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodGenerator : MonoBehaviour
{
    void Start()
    {
        InvokeRepeating("AddWoodRoutine", 1.0f, 3.0f);
    }


    void AddWoodRoutine()
    {
        RessourcesManager.Instance.wood += 1;

    }

}
