using System.Collections.Generic;
using UnityEngine;

public static class GlobalVaribles
{
    public static int hp;
    public static int maxHp;
    public static int fp;
    public static int maxFp;
    public static int cp;
    public static int maxCp;
    public static List<(string, int)> cards;
    public static List<bool[]> aliveEnemiesOnScenes = new List<bool[]>();
    public static int actEnemyNum = -1;
    public static Enemies actEnemy;
    public static int numOfScene;
    public static string sceneName;
    public static string lastSceneNameBeforeBattle;
    public static Vector2 lastPos;
    public static Vector2 lastPosBeforeBattle;
    public static bool isPlayerWin;
    public static bool isPlayerFirstSmash = false;
    public static bool isBaseConfigDone = false;
}
