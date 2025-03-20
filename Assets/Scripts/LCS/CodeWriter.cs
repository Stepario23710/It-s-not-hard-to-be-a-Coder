using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CodeWriter : MonoBehaviour
{
    private List<List<string>> localLastMeaningCodeStrs;
    private TMP_InputField[] ifs;
    private string[] typesOfData = {"string", "bool", "int"}; //добавлять при неоходимости
    void Start()
    {
        localLastMeaningCodeStrs = new List<List<string>>();
        ifs = new TMP_InputField[transform.childCount];
        for (int i = 0; i < ifs.Length; i++){
            ifs[i] = transform.GetChild(i).GetComponent<TMP_InputField>();
        }
        SpawnCodeStrs();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)){
            RememberCodeStrs();
            SceneManager.LoadScene(GlobalVaribles.lastSceneName);
        }
        CodeCheck();
    }
    public void Deleter(){
        int i = 0;
        if (GlobalVaribles.actLifeObjType != null){
            i = 1;
        }
        for (; i < ifs.Length; i++){
            ifs[i].text = "";
        }
    }
    private void CodeCheck(){
        int i = 0;
        if (GlobalVaribles.actLifeObjType != null){
            i = 1;
        }
        localLastMeaningCodeStrs.Clear();
        for (; i < ifs.Length; i++){
            string codeStr = ifs[i].text;
            List<string> meaningStr = new List<string>();
            if (codeStr == ""){
                ifs[i].transform.GetChild(ifs[i].transform.childCount - 1).gameObject.SetActive(false);
                continue;
            }
            if (GlobalVaribles.actLifeObjType == null){
                ifs[i].transform.GetChild(ifs[i].transform.childCount - 1).gameObject.SetActive(true);
                continue;
            }
            string curProp = GlobalVaribles.actLifeObjType.nameOfObj.ToString();
            int lastInd = 0;
            if (codeStr.Length < lastInd + curProp.Length + 1 || curProp + "." != codeStr.Substring(lastInd, curProp.Length + 1)){
                ifs[i].transform.GetChild(ifs[i].transform.childCount - 1).gameObject.SetActive(true);
                continue;
            }
            meaningStr.Add(curProp);
            lastInd += curProp.Length + 1;
            bool ok = false;
            for (int j = 0; j < GlobalVaribles.actLifeObjType.ownProperties.Length; j++){
                curProp = GlobalVaribles.actLifeObjType.ownProperties[j].ToString();
                if (codeStr.Length >= lastInd + curProp.Length && curProp == codeStr.Substring(lastInd, curProp.Length)){
                    ok = true;
                    lastInd += curProp.Length;
                    break;
                }
            }
            if (!ok){
                ifs[i].transform.GetChild(ifs[i].transform.childCount - 1).gameObject.SetActive(true);
                continue;
            }
            meaningStr.Add(curProp);
            while (!typesOfData.Contains(LifeObjsProperties.poperties[curProp][0])){
                ok = false;
                string[] curPropGroup = LifeObjsProperties.poperties[curProp];
                for (int j = 0; j < curPropGroup.Length; j++){
                    curProp = curPropGroup[j];
                    if (codeStr.Length >= lastInd + 1 + curProp.Length && "." + curProp == codeStr.Substring(lastInd, curProp.Length + 1)){
                        ok = true;
                        lastInd += curProp.Length + 1;
                        break;
                    }
                }
                if (!ok){
                    break;
                }
                meaningStr.Add(curProp);
            }
            if (!ok){
                ifs[i].transform.GetChild(ifs[i].transform.childCount - 1).gameObject.SetActive(true);
                continue;
            }
            ok = false;
            string curDataType = LifeObjsProperties.poperties[curProp][0];
            if (curDataType == "bool"){
                if (codeStr.Length >= lastInd + 7 && " = true" == codeStr.Substring(lastInd)){
                    ok = true;
                } else if (codeStr.Length >= lastInd + 8 && " = false" == codeStr.Substring(lastInd)){
                    ok = true;
                }
            } else if (curDataType == "string"){
                if (codeStr.Length >= lastInd + 5 && " = '" == codeStr.Substring(lastInd, 4) && "'" == codeStr[codeStr.Length - 1].ToString()){
                    ok = true;
                } else if (codeStr.Length >= lastInd + 6 && " += '" == codeStr.Substring(lastInd, 5) && "'" == codeStr[codeStr.Length - 1].ToString()){
                    ok = true;
                }
            } else if (curDataType == "int"){
                if (codeStr.Length >= lastInd + 4 && " = " == codeStr.Substring(lastInd, 3) && int.TryParse(codeStr.Substring(lastInd + 3), out int val1) 
                 && codeStr.Substring(lastInd + 3).Replace(" ","") == codeStr.Substring(lastInd + 3)){
                    ok = true;
                } else if (codeStr.Length >= lastInd + 5 && (" += " == codeStr.Substring(lastInd, 4) || " -= " == codeStr.Substring(lastInd, 4) 
                 || " *= " == codeStr.Substring(lastInd, 4) || " /= " == codeStr.Substring(lastInd, 4)) && int.TryParse(codeStr.Substring(lastInd + 4), out int val2) 
                  && codeStr.Substring(lastInd + 4).Replace(" ","") == codeStr.Substring(lastInd + 4)){
                    ok = true;
                } else if (codeStr.Length >= lastInd + 3 && (" ++" == codeStr.Substring(lastInd) || " --" == codeStr.Substring(lastInd))){
                    ok = true;
                }
            } //добавлять при необходимости
            if (!ok){
                ifs[i].transform.GetChild(ifs[i].transform.childCount - 1).gameObject.SetActive(true);
                continue;
            }
            meaningStr.Add(codeStr.Substring(lastInd));
            if(meaningStr.Count != 0){
                localLastMeaningCodeStrs.Add(meaningStr);
            }
            ifs[i].transform.GetChild(ifs[i].transform.childCount - 1).gameObject.SetActive(false);
        }
        GlobalVaribles.lastMeaningCodeStrs = localLastMeaningCodeStrs;
    }
    private void SpawnCodeStrs(){
        if (GlobalVaribles.numOfScene + 1 > GlobalVaribles.codeStrsOfLCSOnScenes.Count){
            GlobalVaribles.codeStrsOfLCSOnScenes.Add(new List<string[]>());
        }
        if (GlobalVaribles.actLCSNum + 1 > GlobalVaribles.codeStrsOfLCSOnScenes[GlobalVaribles.numOfScene].Count){
            GlobalVaribles.codeStrsOfLCSOnScenes[GlobalVaribles.numOfScene].Add(new string[ifs.Length]);
        }
        string[] strs = GlobalVaribles.codeStrsOfLCSOnScenes[GlobalVaribles.numOfScene][GlobalVaribles.actLCSNum];
        if (GlobalVaribles.actLifeObjType != null){
            ifs[0].text = "object " + GlobalVaribles.actLifeObjType.nameOfObj + " = getLOGObject()";
            ifs[0].readOnly = true;
        } else {
            ifs[0].text = strs[0];
        }
        for (int i = 1; i < ifs.Length; i++){
            ifs[i].text = strs[i];
        }
    }
    private void RememberCodeStrs(){
        for (int i = 0; i < ifs.Length; i++){
            GlobalVaribles.codeStrsOfLCSOnScenes[GlobalVaribles.numOfScene][GlobalVaribles.actLCSNum][i] = ifs[i].text;
        }
    }
    public void TryImplementCode(){
        if (GlobalVaribles.lastMeaningCodeStrs.Count == 0){
            return;
        }
        RememberCodeStrs();
        GlobalVaribles.isCodeImplementing = true;
        SceneManager.LoadScene(GlobalVaribles.lastSceneName);
    }
}
