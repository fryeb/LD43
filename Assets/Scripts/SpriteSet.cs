﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Character", menuName = "Sprites/Set", order = 0)]
public class SpriteSet : ScriptableObject
{
    public Sprite idle;
    public Sprite ready;
    public Sprite attack;
}