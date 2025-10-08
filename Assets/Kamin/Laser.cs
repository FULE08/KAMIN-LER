using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Wall
{
    void Start()
    {
        Health = 750;
        Level = 5;
        UpdateLabel();
    }
}
