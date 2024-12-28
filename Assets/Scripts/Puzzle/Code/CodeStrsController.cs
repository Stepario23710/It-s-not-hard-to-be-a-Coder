using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CodeStrsController : MonoBehaviour
{
    [SerializeField] private GameObject strOrig;
    [SerializeField] private int distBeetweenStrs;
    [SerializeField] private CardsSpawner classWithTimersSprites;
    [SerializeField] private GameObject varsParent;
    private Sprite[] timersSprites;
    public EndlessStrInicialistion[] vars {get; private set;}
    void Start()
    {
        timersSprites = classWithTimersSprites.TimersSprites;
        vars = new EndlessStrInicialistion[varsParent.transform.childCount];
        for (int i = 0; i < varsParent.transform.childCount; i++){
            vars[i] = varsParent.transform.GetChild(i).gameObject.GetComponent<EndlessStrInicialistion>();
        }
    }
    public void AddCodeStrWithOnePar(CardInicialisation obj, int var, string par){
        StrInicialisation actStr;
        actStr = Instantiate(strOrig, transform).GetComponent<StrInicialisation>();
        actStr.gameObject.GetComponent<RectTransform>().localPosition =  new Vector3(strOrig.GetComponent<RectTransform>().localPosition.x, 
         strOrig.GetComponent<RectTransform>().localPosition.y - ((transform.childCount - 1 + varsParent.transform.childCount) * 
          distBeetweenStrs), strOrig.GetComponent<RectTransform>().localPosition.z);
        actStr.type = obj.typeOfCard;
        actStr.par = par;
        actStr.timer = obj.timer;
        actStr.numOfVar = var;
        actStr.gameObject.GetComponent<Text>().text = TextOfCodeStrwithOnePar(actStr.type, actStr.numOfVar, actStr.par);
        actStr.gameObject.transform.GetChild(0).GetComponent<Image>().sprite = timersSprites[actStr.timer - 1];
    }
    private string TextOfCodeStrwithOnePar(string type, int varNum, string par){
        string str = "";
        str = vars[varNum].nameOfVar;
        if (type == "print()"){
            str = "print(" + str + ")";
        } else {
            str += " " + type;
        }
        if (par != null) {
            if (par.Length >= 3 && par.Substring(0, 2) == "var"){
                par = vars[Int32.Parse(par.Substring(3))].nameOfVar;
            }
            str += " " + par;
        }
        str += ";";
        return str;
    }
    public bool TimersUpdate(int ch){
        StrInicialisation child = transform.GetChild(ch).GetComponent<StrInicialisation>();
        child.timer -= 1;
        int childs = transform.childCount;
        if (child.timer == 0){
            Destroy(child.gameObject);
            for (int j = ch + 1; j < childs; j++){
                child = transform.GetChild(j).gameObject.transform.GetComponent<StrInicialisation>();
                child.gameObject.GetComponent<RectTransform>().localPosition = 
                 new Vector3(child.gameObject.GetComponent<RectTransform>().localPosition.x, 
                  child.gameObject.GetComponent<RectTransform>().localPosition.y + distBeetweenStrs, 
                   child.gameObject.GetComponent<RectTransform>().localPosition.z);
            }
            return true;
        } else {
            child.gameObject.transform.GetChild(0).GetComponent<Image>().sprite = timersSprites[child.timer - 1];
            return false;
        }
    }
}
