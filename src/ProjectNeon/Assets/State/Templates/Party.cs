﻿using UnityEngine;

public class Party : ScriptableObject
{
    // @todo #1:10min change this to a dynamic size array

    [SerializeField]
    public Character characterOne;

    [SerializeField]
    public Character characterTwo;

    [SerializeField]
    public Character characterThree;

    [SerializeField] private IntVariable totalPowerLevel;

}