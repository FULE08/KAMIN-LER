using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Switch : Wall
{
    void Start()
    {
        Health = 250;
        Level = 5;
        UpdateLabel();
    }
}
