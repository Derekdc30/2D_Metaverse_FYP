using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public static SpawnPoint instance;

    private void Awake()
    {
        instance = this;
    }
}
