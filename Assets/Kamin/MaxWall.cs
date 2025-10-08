using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxWall : Wall
{
    void Start()
    {
        Level = 6;
        Health = 2500;
        UpdateLabel();
    }
}
