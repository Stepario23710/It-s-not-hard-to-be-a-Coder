using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerHealing : ActsBase
{
    [SerializeField] private GameObject deerOrig;
    [SerializeField] private float deerHealingTime;
    private GameObject actDeer;
    public override void Activate()
    {
        actDeer = Instantiate(deerOrig);
        actDeer.GetComponent<Animator>().Play("DeerHealing");
        Invoke(nameof(EndAct), deerHealingTime);
    }
}
