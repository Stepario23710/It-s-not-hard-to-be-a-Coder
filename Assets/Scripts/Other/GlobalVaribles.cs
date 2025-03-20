using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public static class GlobalVaribles
{
    public static int hp;
    public static int maxHp;
    public static List<bool[]> aliveEnemiesOnScenes = new List<bool[]>();
    public static int actEnemyNum = -1;
    public static Enemies actEnemy;
    public static int numOfScene;
    public static string lastSceneName;
    public static Vector2 lastPos;
    public static Vector2 lastPosBeforeBattle;
    public static bool isPlayerWin;
    public static bool isPlayerFirstSmash = false;
    public static bool isBaseConfigDone = false;
    public static LifeObjs actLifeObjType;
    public static int actLifeObjNum = -1;
    public static int actLCSNum = -1;
    public static List<List<string[]>> codeStrsOfLCSOnScenes = new List<List<string[]>>();
    public static List<List<string>> lastMeaningCodeStrs = new List<List<string>>();
    public static bool isCodeImplementing = false;

    public static List<Vector3[]> lifeObjsPositions = new List<Vector3[]>();
    public static List<Quaternion[]> lifeObjsRotations = new List<Quaternion[]>();
    public static List<Vector3[]> lifeObjsScales = new List<Vector3[]>();
    public static List<float[]> lifeObjsGravities = new List<float[]>();
    public static List<string[]> lifeObjsTexts = new List<string[]>();//добавлять при необходимости
}
