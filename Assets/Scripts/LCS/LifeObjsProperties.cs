using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LifeObjsProperties
{
    public static Dictionary<string, string[]> poperties = new Dictionary<string, string[]>(){
        {"position", new string[]{"x", "y"}},
        {"rotation", new string[]{"x", "y", "z"}},
        {"scale", new string[]{"x", "y"}},
        {"gravity", new string[]{"int"}},
        {"isOn", new string[]{"bool"}},
        {"text", new string[]{"string"}},
        {"value", new string[]{"int"}},
        {"x", new string[]{"int"}},
        {"y", new string[]{"int"}},
        {"z", new string[]{"int"}},
    };//добалять при необходимости
}
public enum typesOfLifeObjsProperties{
    position,
    scale,
    rotation,
    gravity,
    isOn,
    text,
    value
}
