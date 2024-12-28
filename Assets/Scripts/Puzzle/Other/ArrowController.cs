using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ArrowController : MonoBehaviour
{
    [SerializeField] private int distBetweenArrows;
    private float[] yPos;
    private int actStr = 0;
    private int endlessStrsQuantity;
    public event Action<int> StrSelected;
    void Start()
    {
        yPos = new float[endlessStrsQuantity];
        for (int i = 0; i < yPos.Length; i++){
            yPos[i] = gameObject.GetComponent<RectTransform>().localPosition.y - i * distBetweenArrows;
        }
    }
    public void InicialEndlessStrs(int endlessStrsQuantity) => this.endlessStrsQuantity = endlessStrsQuantity;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && actStr != 0){
            ArrowMove(-1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow) && actStr != (endlessStrsQuantity - 1)){
            ArrowMove(1);
        }
        if (Input.GetKeyDown(KeyCode.Return)){
            StrSelected?.Invoke(actStr);
        }
    }
    private void ArrowMove(int next){
        actStr += next;
        gameObject.GetComponent<RectTransform>().localPosition = new Vector3(gameObject.GetComponent<RectTransform>().localPosition.x, 
         yPos[actStr], gameObject.GetComponent<RectTransform>().localPosition.z);
    }
}
