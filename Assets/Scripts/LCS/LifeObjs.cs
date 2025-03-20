using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LifeObj", menuName = "Scr Objs/LifeObj")]
public class LifeObjs : ScriptableObject
{
    public string nameOfObj;
    public typesOfLifeObjsProperties[] ownProperties;
}
