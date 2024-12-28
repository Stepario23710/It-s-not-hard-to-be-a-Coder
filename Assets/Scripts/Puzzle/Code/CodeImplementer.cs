using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;
using UnityEngine.UI;

public class CodeImplementer : MonoBehaviour
{
    [SerializeField] private PlayerBattleHpSystem playerHpSystem;
    [SerializeField] private EnemyBattleHpSystem enemyHpSystem;
    [SerializeField] private CardManager cards;
    [SerializeField] private GameObject outputConsoleOrig;
    [SerializeField] private GameObject inputConsoleOrig;
    private OutputConsoleController actOutputConsole;
    private InputConsoleController actInputConsole;
    private Action<int>[] funcs;
    private Action<int, int>[] funcsWithPar;
    private Action<int, int> funcWithInput;
    private int funcWithInputVar;
    private Dictionary<string, int> codeFuncs;
    private int[] varsMeanings;
    private bool isChange;
    private int actStr;
    private int endedAnimationCount;
    public static event Action LastCodeStrImplemented;
    void Start()
    {
        funcWithInput = null;
        funcWithInputVar = 0;
        isChange = false;
        endedAnimationCount = 0;
        varsMeanings = new int[GetComponent<CodeStrsController>().vars.Length];
        actStr = -1;
        actOutputConsole = null;
        actInputConsole = null;
        funcs = new Action<int>[]{
            PlPl, MinMin, Print
        }; //добавлять типы при необходимости
        funcsWithPar = new Action<int, int>[]{
            PlEq, MinEq, Eq
        }; //добавлять типы при необходимости
        codeFuncs = new Dictionary<string, int>(){
            {"++", 0},
            {"--", 1},
            {"print()", 2},
            {"+=", 0},
            {"-=", 1},
            {"=", 2}
        }; //добавлять типы при необходимости
        InputConsoleController.InputDone += InputConsoleEnd;
        OutputConsoleController.ConsoleDestroyed += OutputConsoleEnd;
        playerHpSystem.AnimationEnded += OneMoreAnimEnded;
        enemyHpSystem.AnimationEnded += OneMoreAnimEnded;
    }
    public void Implement(){
        if (isChange){
            actStr--;
            isChange = false;
        }
        if(actStr != -1){
            transform.GetChild(actStr).gameObject.GetComponent<Text>().fontStyle = FontStyle.Normal;
            if (gameObject.GetComponent<CodeStrsController>().TimersUpdate(actStr)){
                isChange = true;
            }
        }
        actStr++;
        if (actStr >= transform.childCount){
            actStr = -1;
            isChange = false;
            cards.CardsUpdate(new int[0]);
            LastCodeStrImplemented?.Invoke();
        } else {
            Action<int> act = (int i) => {};
            Action<int, int> actWithPar = (int i, int j) => {};
            StrInicialisation str;
            bool isInputPar = false;
            str = transform.GetChild(actStr).GetComponent<StrInicialisation>();
            str.gameObject.GetComponent<Text>().fontStyle = FontStyle.BoldAndItalic;
            if(str.par == null){
                act = funcs[codeFuncs[str.type]];
                act(str.numOfVar);
            } else if(str.par != null){
                actWithPar = funcsWithPar[codeFuncs[str.type]];
                if (Int32.TryParse(str.par, out int j)){
                    actWithPar(str.numOfVar, j);
                } else {
                    if (str.par.Length >= 3 && str.par.Substring(0, 3) == "var"){
                        actWithPar(str.numOfVar, varsMeanings[Int32.Parse(str.par.Substring(3, 4))]);
                    } else if (str.par.Length == 7 && str.par.Substring(0, 7) == "input()"){
                        isInputPar = true;
                    }
                }
            }
            if (isInputPar){
                funcWithInput = actWithPar;
                funcWithInputVar = str.numOfVar;
                actInputConsole = Instantiate(inputConsoleOrig, transform.parent).GetComponent<InputConsoleController>();
            } else if (str.type != "print()") {
                Invoke(nameof(StartAction), 1f);
            }
        }
    }
    void Update()
    {
        if (endedAnimationCount == 2){
            endedAnimationCount = 0;
            Implement();
        }
    }
    private void PlPl(int var){
        varsMeanings[var]++;
    }
    private void MinMin(int var){
        varsMeanings[var]--;
    }
    private void PlEq(int var, int ch){
        varsMeanings[var] += ch;
    }
    private void MinEq(int var, int ch){
        varsMeanings[var] -= ch;
    }
    private void Eq(int var, int ch){
        varsMeanings[var] = ch;
    }
    private void Print(int var){
        actOutputConsole = Instantiate(outputConsoleOrig, transform.parent).GetComponent<OutputConsoleController>();
        actOutputConsole.gameObject.transform.GetChild(0).GetComponent<Text>().text = varsMeanings[var].ToString();
    }
    private void StartAction(){
        
    }
    private void FuncWithInput(Action<int, int> act, int var, int input){
        act(var, input);
    }
    private void OutputConsoleEnd(){
        if (actOutputConsole != null){
            actOutputConsole = null;
            StartAction();
        }
    }
    private void InputConsoleEnd(){
        FuncWithInput(funcWithInput, funcWithInputVar, actInputConsole.ReturnInput());
        funcWithInput = null;
        funcWithInputVar = 0;
        actInputConsole = null;
        Invoke(nameof(StartAction), 1f);
    }
    private void OneMoreAnimEnded(){
        endedAnimationCount++;
    }
    private void PuzzleClear(){
        cards.gameObject.GetComponent<CardsSpawner>().AddRemainingCardsToList();
    }
}