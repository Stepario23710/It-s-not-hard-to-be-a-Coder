using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DubinkaStandartAttack : ActsBase
{
    [SerializeField] private GameObject keyFOrig;
    [SerializeField] private float timeWhenYouCanPress;
    [SerializeField] private float timeOfPenalty;
    [SerializeField] private float timeBeforePressing;
    [SerializeField] private float timeBeforePressing2;
    [SerializeField] private float timeBeforeDamage;
    [SerializeField] private int damageIncrease;
    private GameObject keyF;  
    private bool isWaitingOfPressingF = false;
    private bool youCanPressF = false;
    private bool youMissPressingF = false;  
    private int strikesCount = 0;
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.F)){
            if(youCanPressF && !isWaitingOfPressingF && !youMissPressingF){
                youMissPressingF = true;
                actCorrectOrWrong = Instantiate(correctOrWrongOrig);
                actCorrectOrWrong.GetComponent<SpriteRenderer>().sprite = actCorrectOrWrong.GetComponent<KeyInicialisation>().notPressedSpr;
            }
            if (isWaitingOfPressingF && !youMissPressingF){
                isWaitingOfPressingF = false;
                youCanPressF = false;
                value += damageIncrease;
                actCorrectOrWrong = Instantiate(correctOrWrongOrig);
                actCorrectOrWrong.GetComponent<SpriteRenderer>().sprite = actCorrectOrWrong.GetComponent<KeyInicialisation>().pressedSpr;
            }
        }
    }
    public override void Activate(){
        keyF = Instantiate(keyFOrig);
        Invoke(nameof(NowYouCanPressF), timeBeforePressing - timeOfPenalty);
        Invoke(nameof(KeyFMustPressed), timeBeforePressing);
    }
    private void KeyFMustPressed(){
        strikesCount++;
        isWaitingOfPressingF = true;
        keyF.GetComponent<SpriteRenderer>().sprite = keyF.GetComponent<KeyInicialisation>().pressedSpr;
        Invoke(nameof(NowYouCantPressF), timeWhenYouCanPress + timeOfPenalty);
        Invoke(nameof(KeyFMustNotPressed), timeWhenYouCanPress);
    }
    private void KeyFMustNotPressed(){
        isWaitingOfPressingF = false;
        keyF.GetComponent<SpriteRenderer>().sprite = keyF.GetComponent<KeyInicialisation>().notPressedSpr;
        if(strikesCount == 1){
            Invoke(nameof(NowYouCanPressF), timeBeforePressing2 - timeOfPenalty);
            Invoke(nameof(KeyFMustPressed), timeBeforePressing2);
        } else if(strikesCount == 2){
            strikesCount = 0;
            Invoke(nameof(EndAct), timeBeforeDamage);
            Invoke(nameof(OwnEndAct), timeBeforeDamage);
        }
    }
    private void NowYouCanPressF(){
        youCanPressF = true;
    }
    private void NowYouCantPressF(){
        youCanPressF = false;
        youMissPressingF = false;
        Destroy(actCorrectOrWrong);
        actCorrectOrWrong = null;
    }
    protected override void OwnEndAct(){
        Destroy(keyF);
        keyF = null;
    }
}
