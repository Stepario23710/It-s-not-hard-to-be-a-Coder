using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ButtonShaper : MonoBehaviour
{
    private Image im;
    void Start()
    {
        im = gameObject.GetComponent<Image>();
        im.alphaHitTestMinimumThreshold = 1;
    }
}
