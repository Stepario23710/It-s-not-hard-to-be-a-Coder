using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardChoosingController : MonoBehaviour
{
    private bool chosen = false;
    void OnMouseOver()
    {
        if (!chosen){
            if (GetComponent<Button>().interactable) {
                GetComponent<Animator>().SetInteger("Choose", 1);
            } else {
                GetComponent<Animator>().SetInteger("Choose", 0);
            }
        }
    }
    void OnMouseExit()
    {
        if (!chosen){
            GetComponent<Animator>().SetInteger("Choose", 0);
        }
    }
    public void Chosen(){
        chosen = true;
        GetComponent<Animator>().SetInteger("Choose", 2);
    }
    public void UnChosen(){
        chosen = false;
        GetComponent<Animator>().SetInteger("Choose", 0);
    }
}
