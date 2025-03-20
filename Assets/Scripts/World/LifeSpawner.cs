using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LifeSpawner : MonoBehaviour
{
    [SerializeField] private GameObject LCSs;
    [SerializeField] private GameObject lifeObjs;
    void Start()
    {
        SpawnLCSs();
        SpawnLifeObjs();
    }
    private void SpawnLCSs(){
        for (int i = 0; i < LCSs.transform.childCount; i++){
            LCSs.transform.GetChild(i).gameObject.GetComponent<LCSInicialisation>().numOfLCSOnScene = i;
        }
    }
    public void SpawnLifeObjs(){
        if (GlobalVaribles.numOfScene + 1 > GlobalVaribles.lifeObjsPositions.Count){
            GlobalVaribles.lifeObjsPositions.Add(Enumerable.Repeat(new Vector3(), lifeObjs.transform.childCount).ToArray());
            GlobalVaribles.lifeObjsRotations.Add(Enumerable.Repeat(new Quaternion(), lifeObjs.transform.childCount).ToArray());
            GlobalVaribles.lifeObjsScales.Add(Enumerable.Repeat(new Vector3(), lifeObjs.transform.childCount).ToArray());
            GlobalVaribles.lifeObjsGravities.Add(Enumerable.Repeat(0f, lifeObjs.transform.childCount).ToArray());
            GlobalVaribles.lifeObjsTexts.Add(Enumerable.Repeat("", lifeObjs.transform.childCount).ToArray());
            SaveLifeObjs();
        } else{
            for (int i = 0; i < lifeObjs.transform.childCount; i++){
                GameObject curObj = lifeObjs.transform.GetChild(i).gameObject;
                curObj.GetComponent<LifeObjInicialisation>().numOfLifeObjOnScene = i;
                curObj.transform.localPosition = GlobalVaribles.lifeObjsPositions[GlobalVaribles.numOfScene][i];
                curObj.transform.rotation = GlobalVaribles.lifeObjsRotations[GlobalVaribles.numOfScene][i];
                curObj.transform.localScale = GlobalVaribles.lifeObjsScales[GlobalVaribles.numOfScene][i];
                if (curObj.GetComponent<Rigidbody2D>() != null){
                    curObj.GetComponent<Rigidbody2D>().gravityScale = GlobalVaribles.lifeObjsGravities[GlobalVaribles.numOfScene][i];
                }
                if (curObj.GetComponent<Text>() != null){
                    curObj.GetComponent<Text>().text = GlobalVaribles.lifeObjsTexts[GlobalVaribles.numOfScene][i];
                }
            }
        }
    }
    public void SaveLifeObjs(){
        for (int i = 0; i < lifeObjs.transform.childCount; i++){
            GameObject curObj = lifeObjs.transform.GetChild(i).gameObject;
            GlobalVaribles.lifeObjsPositions[GlobalVaribles.numOfScene][i] = curObj.transform.localPosition;
            GlobalVaribles.lifeObjsRotations[GlobalVaribles.numOfScene][i] = curObj.transform.rotation;
            GlobalVaribles.lifeObjsScales[GlobalVaribles.numOfScene][i] = curObj.transform.localScale;
            if (curObj.GetComponent<Rigidbody2D>() != null){
                GlobalVaribles.lifeObjsGravities[GlobalVaribles.numOfScene][i] = curObj.GetComponent<Rigidbody2D>().gravityScale;
            }
            if (curObj.GetComponent<Text>() != null){
                GlobalVaribles.lifeObjsTexts[GlobalVaribles.numOfScene][i] = curObj.GetComponent<Text>().text;
            }
        }
    } //добавлять при необходимости
}
