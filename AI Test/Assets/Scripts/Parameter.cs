using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 角色属性
/// </summary>
/// 

[Serializable]
public class Parameter 
{
    public int health;  //血量
    public float moveSpeed; //移动速度
    public float chaseSpeed; //追击速度
    public float idleTime; //观察时间
    public Transform[] patrolPoints;//巡逻店
    public Transform[] chasePoints; //追击点
    public Transform target;    //目标
    public LayerMask targetLayer;   //目标图层
    public Transform attakPoint;    //攻击点
    public float attakArea; //攻击范围
    public Animator animator;   //动画管理器
    public bool getHit; //是否被攻击

}
