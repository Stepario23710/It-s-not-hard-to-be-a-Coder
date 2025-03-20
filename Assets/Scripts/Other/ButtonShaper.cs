using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ButtonShaper : MonoBehaviour
{
    [Range(0f, 1f)] public float alphaLevel;
    private Image im;
    void Start()
    {
        im = gameObject.GetComponent<Image>();
        im.alphaHitTestMinimumThreshold = alphaLevel;
    }
}
