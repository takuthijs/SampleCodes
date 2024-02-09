using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BattStatusScriptable : ScriptableObject
{
    public int battNumber;
    public string battName;
    public GameObject battObj;
    public Sprite battImage;
    public int maxLevel;
    public int battlevel;
    public int power;
    public int meet;
    public float speed;
    public int value;
}
