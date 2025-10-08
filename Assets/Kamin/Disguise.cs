using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Disguise : Wall
{
    void Start()
    {
        Health = 2500;
        Level = 4;
        MaxLevel = 1;
        UpdateLabel();
    }
    public override void Upgraded()
    {
        if (gamemanager.Currency >= UpgradeCost && Level > MaxLevel)
        {
            gamemanager.Currency -= UpgradeCost;
            Level--;
            Health += 500;
            UpgradeCost *= 2;
            Debug.Log($"The {gameObject.name} has been upgraded, Its level is now {Level}");
        }
        else if (Level >= MaxLevel)
        {
            Debug.Log("Max Level");
        }
        else
        {
            int needMore = UpgradeCost - gamemanager.Currency;
            Debug.Log($"Need {needMore}$ more");
        }
    }

    public override void UpdateLabel()
    {
        if (TextLabel != null)
            TextLabel.text = $"Level : {Level} (Fake)\n Health : {Health}";
    }
}
