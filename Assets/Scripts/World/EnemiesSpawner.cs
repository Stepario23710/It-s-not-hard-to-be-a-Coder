using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemiesSpawner : MonoBehaviour
{
    void Awake()
    {
        SpawnEnemies();
    }
    private void SpawnEnemies(){
        for (int i = 0; i < transform.childCount; i++){
            transform.GetChild(i).gameObject.GetComponent<EnemyInicialisation>().num = i;
        }
        if (GlobalVaribles.numOfScene + 1 > GlobalVaribles.aliveEnemiesOnScenes.Count){
            GlobalVaribles.aliveEnemiesOnScenes.Add(Enumerable.Repeat(true, transform.childCount).ToArray());
        } else {
            List<int> destroyingEnemies = new List<int>();
            bool[] enemiesOnThisScene = GlobalVaribles.aliveEnemiesOnScenes[GlobalVaribles.numOfScene];
            for (int i = 0; i < enemiesOnThisScene.Length; i++){
                if (!enemiesOnThisScene[i]){
                    destroyingEnemies.Add(i);
                }
            }
            for (int i = 0; i < destroyingEnemies.Count; i++){
                Destroy(transform.GetChild(destroyingEnemies[i]).gameObject);
            }
        }
    }
}
