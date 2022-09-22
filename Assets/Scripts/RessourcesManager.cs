using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RessourcesManager : MonoBehaviour
{
   public static RessourcesManager Instance { get;private set; }

    public int gold;
    public int wood;
    public int rock;
    public int energy;
    public int energyConsumption;
    public int stockage;
    public TMP_Text goldUI;
    public TMP_Text woodUI;
    public TMP_Text rockUI;
    public TMP_Text foodUI;


    private void Awake()
    {
        if (!Instance)
            Instance = this;
        
        else
            Destroy(gameObject);


        gold = 50;
        wood = 50;
        rock = 50;
        energy = 50;
        energyConsumption = 1;
        stockage = 100;
        InvokeRepeating("FoodConsumed", 10.0f, 10.0f);
    }
    private void Update()
    {
        goldUI.text = "Or: " + gold.ToString();
        woodUI.text = "Bois: " + wood.ToString();
        rockUI.text = "Pierre: " + rock.ToString();
        foodUI.text = "Nourriture: " + energy.ToString();

        if (rock > stockage)
            rock = stockage;
        if (wood > stockage)
            wood = stockage;
        if (energy > stockage)
            energy = stockage;
    }
    void FoodConsumed()
    {
        energy -= energyConsumption;
    }

}
