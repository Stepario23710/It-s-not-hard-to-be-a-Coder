using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    [SerializeField] private GameObject arrowOrig;
    [SerializeField] private CodeStrsController code;
    private CardsSpawner classWithCardPos;
    private static string[] typeWithNeedOfNums;
    private List<CardInicialisation> actCards;
    public List<Button> cards;
    private ArrowController actArrow;
    void Start()
    {
        classWithCardPos = gameObject.GetComponent<CardsSpawner>();
        typeWithNeedOfNums = new string[] {
            "+=",
            "-="
        }; //добавлять типы при необходимости
        actArrow = null;
        actCards = new List<CardInicialisation>();
        CardsUpdate(new int[0]);
    }
    void Update()
    {
        if (actCards.Count > 0 && Input.GetKeyUp(KeyCode.Escape)){
            if (actArrow != null){
                Destroy(actArrow.gameObject);
            }
            actArrow = null;
            for (int i = 0; i < actCards.Count; i++){
                actCards[i].gameObject.GetComponent<CardChoosingController>().UnChosen();
            }
            actCards.Clear();
            CardsUpdate(new int[0]);
        }
    }
    public void CardActivate(CardInicialisation obj){
        string type = obj.typeOfCard;
        int timer = obj.timer;
        bool isNeedOfNums = false;
        if (typeWithNeedOfNums.Contains(type)){
            isNeedOfNums = true;
        }
        for (int i = 0; i < transform.childCount; i++){
            if (isNeedOfNums && cards[i].gameObject.GetComponent<CardInicialisation>().timer == 0){
                cards[i].interactable = true;
            } else {
                cards[i].interactable = false;
            }
        }
        actCards.Add(obj);
        if (!isNeedOfNums){
            actArrow = Instantiate(arrowOrig, transform.parent).GetComponent<ArrowController>();
            actArrow.InicialEndlessStrs(code.vars.Length);
            actArrow.StrSelected += CardUsing;
        }
    }
    public void CardsUpdate(int[] badNums){
        cards = new List<Button>();
        for (int i = 0, j = 0; i < transform.childCount; i++){
            if (!badNums.Contains(i)){
                cards.Add(transform.GetChild(i).GetComponent<Button>());
                cards[j].gameObject.GetComponent<RectTransform>().localPosition = 
                 new Vector2(classWithCardPos.cardDefaultPos.x + j * classWithCardPos.DistBeetweenCards, 
                  classWithCardPos.cardDefaultPos.y);
                cards[j].gameObject.GetComponent<CardInicialisation>().num = j;
                if (cards[j].gameObject.GetComponent<CardInicialisation>().timer == 0){
                    cards[j].interactable = false;
                } else {
                    cards[j].interactable = true;
                }
                j++;
            }
        }
    }
    private void CardUsing(int numOfVar){
        code.AddCodeStrWithOnePar(actCards[0], numOfVar, actCards.Count > 1 ? actCards[1].typeOfCard : null);
        int[] badNums = new int[actCards.Count];
        for (int i = 0; i < actCards.Count; i++){
            Destroy(actCards[i].gameObject);
            badNums[i] = actCards[i].num;
        }
        Destroy(actArrow.gameObject);
        actArrow = null;
        CardsUpdate(badNums);
        actCards.Clear();
    }
    public void CardsStop(){
        for (int i = 0; i < transform.childCount; i++){
            cards[i].interactable = false;
        }
    }
}
