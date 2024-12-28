using System;
using UnityEngine;

public abstract class ActsBase : MonoBehaviour
{
    [SerializeField] protected GameObject correctOrWrongOrig;
    [SerializeField] protected int baseValue;
    protected GameObject actCorrectOrWrong;
    protected int value;
    public static event Action ActEnded;
    public abstract void Activate();
    protected virtual void OwnEndAct(){}
    protected void EndAct(){
        ActEnded?.Invoke();
    }
    protected void Start()
    {
        value = baseValue;
    }
    public int ReturnValue(){
        int trueValue = value;
        value = baseValue;
        return trueValue;
    }
}
