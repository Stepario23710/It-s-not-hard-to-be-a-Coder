using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActsManager : MonoBehaviour
{
    [field: SerializeField] public EnemyBattleHpSystem enemyHpSystem {get; private set;}
    [NonSerialized] public BattleActInicialisation act;
    private Animator anim;
    private Dictionary<string, (ActsBase, bool)> acts;
    void Start()
    {
        anim = GetComponent<Animator>();
        acts = new Dictionary<string, (ActsBase, bool)>(){
            {"DubinkaAtk", (GetComponent<DubinkaAttack>(), true)},
        };//добавлять при необходимости
        ActsBase.ActEnded += DoSomethingWithPlayerOrEnemy;
        enemyHpSystem.EnemyDead += EndBattle;
    }
    private void ActivateAct(){
        if (GlobalVaribles.isPlayerFirstSmash){
            acts["DubinkaAtk"].Item1.Activate();
            anim.SetBool("IsDubinkaAtk", true);
        } else {
            acts[act.typeOfAct].Item1.Activate();
            anim.SetBool("Is" + act.typeOfAct, true);
        }
    }
    private void DoSomethingWithPlayerOrEnemy(){
        if (GlobalVaribles.isPlayerFirstSmash){
            enemyHpSystem.Damage(acts["DubinkaAtk"].Item1.ReturnValue());
            anim.SetBool("IsDubinkaAtk", false);
            GlobalVaribles.isPlayerFirstSmash = false;
        } else {
            int val = acts[act.typeOfAct].Item1.ReturnValue();
            if (acts[act.typeOfAct].Item2){
                enemyHpSystem.Damage(val);
            } else {
                gameObject.GetComponent<PlayerBattleHpSystem>().Heal(val);
            }
            anim.SetBool("Is" + act.typeOfAct, false);
        }
    }
    private void EndBattle(){
        ActsBase.ActEnded -= DoSomethingWithPlayerOrEnemy;
    }
}
