using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Scr Objs/Enemy")]
public class Enemies : ScriptableObject
{
    public int minHp;
    public int maxHp;
    public Sprite sprite;
}