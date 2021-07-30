using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum StateType
{
    Idle,   //闲置
    Patrol, //巡逻
    Chase,  //追击
    React,  //发现敌人
    Attack, //攻击
    Injured,    //被攻击
    Death   //死亡
}

/// <summary>
/// 状态机对象
/// </summary>
public class FSM : MonoBehaviour
{
    private IState currentState;    //当前状态
    private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();

    public Parameter parameter;

    // Start is called before the first frame update
    void Start()
    {
        states.Add(StateType.Idle, new IdleState(this));
        states.Add(StateType.Patrol, new PatrolState(this));
        states.Add(StateType.React, new ReactState(this));
        states.Add(StateType.Chase, new ChaseState(this));
        states.Add(StateType.Attack, new AttackState(this));
        states.Add(StateType.Injured, new InjuredState(this));
        states.Add(StateType.Death, new DeathState(this));
        
        parameter.animator = transform.GetComponent<Animator>();
        
        
        TransitionState(StateType.Patrol);

       

    }

    // Update is called once per frame
    void Update()
    {
        currentState.OnUpdate();
    }

    /// <summary>
    /// 更换状态
    /// </summary>
    /// <param name="type"></param>
    public void TransitionState(StateType type)
    {
        if(currentState != null)
        {
            currentState.OnExit();
        }
        currentState = states[type];
        currentState.OnEnter();

    }

    /// <summary>
    /// 更改方向   敌人初始方向向右
    /// </summary>
    /// <param name="target"></param>
    public void FilpTo(Transform target)
    {
        if(target != null)
        {
           
            if(transform.position.x > target.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else if(transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            parameter.target = other.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            parameter.target = null;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(parameter.attakPoint.position, parameter.attakArea);
    }

}
