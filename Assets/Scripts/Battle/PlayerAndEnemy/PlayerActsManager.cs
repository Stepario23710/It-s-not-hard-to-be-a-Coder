using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerActsManager : MonoBehaviour
{
    [field: SerializeField] public EnemyBattleHpSystem enemyHpSystem {get; private set;}
    [SerializeField] private Text playerFPText;
    [SerializeField] private Text playerCPText;
    [NonSerialized] public BattleActInicialisation act;
    private Animator anim;
    private Dictionary<string, (ActsBase, bool)> acts;
    private int maxFp;
    private int maxCp;
    public int fp {get; private set;}
    public int cp {get; private set;}
    void Start()
    {
        anim = GetComponent<Animator>();
        acts = new Dictionary<string, (ActsBase, bool)>(){
            {"DubinkaStandartAtk", (GetComponent<DubinkaStandartAttack>(), true)},
            {"DeerHealing", (GetComponent<DeerHealing>(), false)}
        };//добавлять при необходимости
        ActsBase.ActEnded += DoSomethingWithPlayerOrEnemy;
        maxFp = GlobalVaribles.maxFp;
        fp = GlobalVaribles.fp;
        maxCp = GlobalVaribles.maxCp;
        cp = GlobalVaribles.cp;
        playerFPText.text = fp.ToString() + "/" + maxFp.ToString();
        playerCPText.text = cp.ToString() + "/" + maxCp.ToString();
        enemyHpSystem.EnemyDead += EndBattle;
    }
    void Update()
    {
        playerFPText.text = fp.ToString() + "/" + maxFp.ToString();
        playerCPText.text = cp.ToString() + "/" + maxCp.ToString();
    }
    private void ActivateAct(){
        if (GlobalVaribles.isPlayerFirstSmash){
            acts["DubinkaStandartAtk"].Item1.Activate();
            anim.SetBool("IsDubinkaStandartAtk", true);
        } else {
            acts[act.typeOfAct].Item1.Activate();
            anim.SetBool("Is" + act.typeOfAct, true);
            fp -= act.needFp;
            cp -= act.needCp;
        }
    }
    private void DoSomethingWithPlayerOrEnemy(){
        if (GlobalVaribles.isPlayerFirstSmash){
            enemyHpSystem.Damage(acts["DubinkaStandartAtk"].Item1.ReturnValue());
            anim.SetBool("IsDubinkaStandartAtk", false);
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
        GlobalVaribles.fp = fp;
        GlobalVaribles.cp = cp;
    }
}
