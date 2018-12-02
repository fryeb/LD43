using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Character", menuName = "Set", order = 0)]
public class SpriteSet : ScriptableObject
{
    public Sprite idle;
    public Sprite ready;
    public Sprite attack;
    public Sprite dead;
}
