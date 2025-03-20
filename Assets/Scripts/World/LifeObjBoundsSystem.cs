using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeObjBoundsSystem : MonoBehaviour
{
    [SerializeField] private Transform LWall;
    [SerializeField] private Transform RWall;
    [SerializeField] private Transform UWall;
    [SerializeField] private Transform DWall;
    public event Action IsBoundsCrossed;
    void Update()
    {
        if (transform.position.x < LWall.position.x || transform.position.x > RWall.position.x || 
         transform.position.y < DWall.position.y || transform.position.y > UWall.position.y){
            IsBoundsCrossed?.Invoke();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject != gameObject && collision.gameObject != transform.parent.parent.gameObject){
            IsBoundsCrossed?.Invoke();
        }
    }
}