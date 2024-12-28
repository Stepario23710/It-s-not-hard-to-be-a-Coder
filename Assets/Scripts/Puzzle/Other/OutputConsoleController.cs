using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutputConsoleController : MonoBehaviour
{
    public static event Action ConsoleDestroyed;
    void Start()
    {
        gameObject.SetActive(false);
        Invoke(nameof(Appear), 0.1f);
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return)){
            Disappear();
        }
    }
    public void DestroyObj(){
        ConsoleDestroyed?.Invoke();
        Destroy(gameObject);
    }
    protected void Disappear(){
        GetComponent<Animator>().SetBool("Disappearing", true);
    }
    private void Appear(){
        gameObject.SetActive(true);
    }
}
