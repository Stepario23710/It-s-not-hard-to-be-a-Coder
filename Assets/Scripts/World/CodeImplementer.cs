using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class CodeImplementer : MonoBehaviour
{
    [SerializeField] private PostProcessProfile ppp;
    [SerializeField] private CinemachineVirtualCamera virCam;
    [SerializeField] private GameObject player;
    private GameObject lifeObj;
    private Queue<string> keysForOps;
    private Dictionary<string, (object, Action<object>)> opsWithLifeObj;
    void Start()
    {
        if(GlobalVaribles.isCodeImplementing == true){
            keysForOps = new Queue<string>();
            lifeObj = transform.GetChild(GlobalVaribles.actLifeObjNum).gameObject;
            opsWithLifeObj = new Dictionary<string, (object, Action<object>)>(){
                {"position.x", (lifeObj.transform.localPosition.x, OpPositionX)},
                {"position.y", (lifeObj.transform.localPosition.y, OpPositionY)},
                {"rotation.x", (lifeObj.transform.rotation.x, OpRotationX)},
                {"rotation.y", (lifeObj.transform.rotation.y, OpRotationY)},
                {"rotation.z", (lifeObj.transform.rotation.z, OpRotationZ)},
                {"scale.x", (lifeObj.transform.localScale.x, OpScaleX)},
                {"scale.y", (lifeObj.transform.localScale.y, OpScaleY)},
                //{"value", (null, null)},
                //{"isOn", (null, null)},
            };//добавлять при необходимости
            if (lifeObj.GetComponent<Rigidbody2D>() != null){
                opsWithLifeObj.Add("gravity", (lifeObj.GetComponent<Rigidbody2D>().gravityScale, OpGravity));
            }
            if (lifeObj.GetComponent<Text>() != null){
                opsWithLifeObj.Add("text", (lifeObj.GetComponent<Text>().text, OpText));
            }
            lifeObj.GetComponent<LifeObjBoundsSystem>().IsBoundsCrossed += StopCodeImplementing;
            Invoke(nameof(SetCamOnLifeObj), 1f);
            Invoke(nameof(CodeImplementing), 3f);
        }
    }
    private void CodeImplementing(){ 
        if (lifeObj.GetComponent<Rigidbody2D>() != null){
            lifeObj.GetComponent<Rigidbody2D>().isKinematic = true;
        }
        List<List<string>> lifeCodeStrs = GlobalVaribles.lastMeaningCodeStrs;
        for (int i = 0; i < lifeCodeStrs.Count; i++){
            string key = lifeCodeStrs[i][1];
            for (int j = 2; j < lifeCodeStrs[i].Count - 1; j++){
                key += ".";
                key += lifeCodeStrs[i][j];
            }
            string op = lifeCodeStrs[i][lifeCodeStrs[i].Count - 1];
            string opTypeOfData = LifeObjsProperties.poperties[lifeCodeStrs[i][lifeCodeStrs[i].Count - 2]][0];
            if (op.Substring(0, 2) == " ="){
                if (opTypeOfData == "int"){
                    opsWithLifeObj[key] = (Convert.ToSingle(op.Substring(3)), opsWithLifeObj[key].Item2);
                } else if (opTypeOfData == "bool"){
                    opsWithLifeObj[key] = (Convert.ToBoolean(op.Substring(3)), opsWithLifeObj[key].Item2);
                } else if (opTypeOfData == "string"){
                    opsWithLifeObj[key] = (op.Substring(4, op.Length - 5), opsWithLifeObj[key].Item2);
                }
            } else if (op.Substring(0, 3) == " +="){
                if (opTypeOfData == "int"){
                    opsWithLifeObj[key] = (Convert.ToSingle(opsWithLifeObj[key].Item1) + Convert.ToSingle(op.Substring(4)), opsWithLifeObj[key].Item2);
                } else if (opTypeOfData == "string"){
                    opsWithLifeObj[key] = (Convert.ToString(opsWithLifeObj[key].Item1) + op.Substring(4, op.Length - 5), opsWithLifeObj[key].Item2);
                }
            } else if (op.Substring(0, 3) == " -="){
                opsWithLifeObj[key] = (Convert.ToSingle(opsWithLifeObj[key].Item1) - Convert.ToSingle(op.Substring(4)), opsWithLifeObj[key].Item2);
            } else if (op.Substring(0, 3) == " --"){
                opsWithLifeObj[key] = (Convert.ToSingle(opsWithLifeObj[key].Item1) - 1, opsWithLifeObj[key].Item2);
            } else if (op.Substring(0, 3) == " ++"){
                opsWithLifeObj[key] = (Convert.ToSingle(opsWithLifeObj[key].Item1) + 1, opsWithLifeObj[key].Item2);
            } else if (op.Substring(0, 3) == " *="){
                opsWithLifeObj[key] = (Convert.ToSingle(opsWithLifeObj[key].Item1) * Convert.ToSingle(op.Substring(4)), opsWithLifeObj[key].Item2);
            } else if (op.Substring(0, 3) == " /="){
                opsWithLifeObj[key] = (Convert.ToSingle(opsWithLifeObj[key].Item1) / Convert.ToSingle(op.Substring(4)), opsWithLifeObj[key].Item2);
            }//добавлять при необходимости
            keysForOps.Enqueue(key);
            Invoke(nameof(LaunchOp), i);
            if (i == lifeCodeStrs.Count - 1){
                Invoke(nameof(EndCodeImplementing), i + 1f);
            }
        }
    }
    private void SetCamOnLifeObj(){
        virCam.Follow = lifeObj.transform;
    }
    private void SetCamOnPlayer(){
        virCam.Follow = player.transform;
        GlobalVaribles.isCodeImplementing = false;
    }
    private void LaunchOp(){
        string key = keysForOps.Dequeue();
        opsWithLifeObj[key].Item2(opsWithLifeObj[key].Item1);
    }
    private void EndCodeImplementing(){
        if (lifeObj.GetComponent<Rigidbody2D>() != null){
            lifeObj.GetComponent<Rigidbody2D>().isKinematic = false;
        }
        Invoke(nameof(SetCamOnPlayer), 1f);
    }
    private void CodeImplementingError(){
        transform.parent.GetComponent<LifeSpawner>().SpawnLifeObjs();
        ppp.GetSetting<ChromaticAberration>().intensity.value = 1f;
        StartCoroutine(ErrorDisappear());
    }
    private IEnumerator ErrorDisappear(){
        for (int i = 0; i < 100; i++){
            ppp.GetSetting<ChromaticAberration>().intensity.value -= 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
    }
    private void StopCodeImplementing(){
        CancelInvoke(nameof(LaunchOp));
        CancelInvoke(nameof(EndCodeImplementing));
        Invoke(nameof(CodeImplementingError), 1f);
        Invoke(nameof(EndCodeImplementing), 1f);
        lifeObj.GetComponent<LifeObjBoundsSystem>().IsBoundsCrossed -= StopCodeImplementing;
    }
    private void OpPositionX(object val){
        lifeObj.transform.localPosition = new Vector2(Convert.ToSingle(val), lifeObj.transform.localPosition.y);
    }
    private void OpPositionY(object val){
        lifeObj.transform.localPosition = new Vector2(lifeObj.transform.localPosition.x, Convert.ToSingle(val));
    }
    private void OpRotationX(object val){
        lifeObj.transform.rotation = Quaternion.Euler(Convert.ToSingle(val), 0, 0);
    }
    private void OpRotationY(object val){
        lifeObj.transform.rotation = Quaternion.Euler(0, Convert.ToSingle(val), 0);
    }
    private void OpRotationZ(object val){
        lifeObj.transform.rotation = Quaternion.Euler(0, 0, Convert.ToSingle(val));
    }
    private void OpScaleX(object val){
        lifeObj.transform.localScale = new Vector2(Convert.ToSingle(val), lifeObj.transform.localScale.y);
    }
    private void OpScaleY(object val){
        lifeObj.transform.localScale = new Vector2(lifeObj.transform.localScale.x, Convert.ToSingle(val));
    }
    private void OpGravity(object val){
        lifeObj.GetComponent<Rigidbody2D>().gravityScale = Convert.ToSingle(val);
    }
    private void OpText(object val){
        if (lifeObj.GetComponent<Text>() != null){
            lifeObj.GetComponent<Text>().text = Convert.ToString(val);
        }
    }//добавлять при необходимости
}