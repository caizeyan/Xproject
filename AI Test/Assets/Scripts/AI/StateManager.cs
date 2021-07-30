using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    /// <summary>
    /// 闲置状态
    /// </summary>
    public class IdleState : IState
    {
        private FSM manager;    //状态机管理器
        private Parameter parameter;    //对象属性


        private float timer;    //闲置时间

        public IdleState(FSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            parameter.animator.Play("Idle");
        }

        public void OnExit()
        {
            timer = 0;
            return;
        }

        public void OnUpdate()
        {
            timer += Time.deltaTime;
            //被攻击优先级最高
            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Injured);
                return;
            }

            //发现敌人  进入警戒状态
            if(parameter.target != null && 
                parameter.target.position.x >= parameter.chasePoints[0].position.x &&
                parameter.target.position.x <= parameter.chasePoints[1].position.x)
            {
                manager.TransitionState(StateType.React);
                return;
            }

            //闲置时间到 进入巡逻
            if(timer >= parameter.idleTime)
            {
                manager.TransitionState(StateType.Patrol);
                return;
            }


        }
    }



    /// <summary>
    /// 巡逻状态
    /// </summary>
    public class PatrolState : IState
    {
        private FSM manager;    //状态机管理器
        private Parameter parameter;    //对象属性

        private int patrolIndex; //目标巡逻点


        public PatrolState(FSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            parameter.animator.Play("Walk");
        }

    
        public void OnExit()
        {
            patrolIndex++;
            if(patrolIndex >= parameter.patrolPoints.Length)
            {
                patrolIndex = 0;
            }
        }

        public void OnUpdate()
        {
            //走向巡逻点
            manager.FilpTo(parameter.patrolPoints[patrolIndex]);

            manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                parameter.patrolPoints[patrolIndex].position, parameter.moveSpeed * Time.deltaTime
                );

            //被攻击优先级最高
            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Injured);
                return;
            }

            //发现敌人  进入警戒状态
            if (parameter.target != null &&
                parameter.target.position.x >= parameter.chasePoints[0].position.x &&
                parameter.target.position.x <= parameter.chasePoints[1].position.x)
            {
                manager.TransitionState(StateType.React);
                return;
            }

            //到达巡逻点 进入闲置状
            if(Vector2.Distance(manager.transform.position,parameter.patrolPoints[patrolIndex].position) < 0.1f)
            {
                manager.TransitionState(StateType.Patrol);
            }

        }

    }


    /// <summary>
    /// 追击状态
    /// </summary>
    public class ChaseState : IState
    {
        private FSM manager;    //状态机管理器
        private Parameter parameter;    //对象属性

        public ChaseState(FSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            parameter.animator.Play("Walk");
           
        }

        public void OnExit()
        {
            
        }

        public void OnUpdate()
        {
            manager.FilpTo(parameter.target);

            if (parameter.target)
            {
                manager.transform.position = Vector2.MoveTowards(manager.transform.position,
                    parameter.target.position, parameter.chaseSpeed * Time.deltaTime
                    );
            }

            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Injured);
                return;
            }

            //判断是否进入攻击范围
            if(Physics2D.OverlapCircle(parameter.attakPoint.position,parameter.attakArea,parameter.targetLayer))
            {
                manager.TransitionState(StateType.Attack);
            }

            //判断是否出界 出界进入巡逻状态
            if(parameter.target == null ||
               manager.transform.position.x < parameter.chasePoints[0].position.x ||
               manager.transform.position.x > parameter.chasePoints[1].position.x
                )
            {
                manager.TransitionState(StateType.Patrol);
            }

            
        }
    }

    /// <summary>
    /// 发现敌人状态
    /// </summary>
    public class ReactState : IState
    {
        private FSM manager;    //状态机管理器
        private Parameter parameter;    //对象属性

        private AnimatorStateInfo info;

        public ReactState(FSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            parameter.animator.Play("React");
        }

        public void OnExit()
        {
           
        }

        public void OnUpdate()
        {
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);

            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Injured);
                return;
            }

            //动画播完进入追击状态
            if(info.normalizedTime >= 0.95f)
            {
                manager.TransitionState(StateType.Chase);
            }


        }
    }


    /// <summary>
    /// 受伤状态
    /// </summary>
    public class InjuredState : IState
    {
        private FSM manager;    //状态机管理器
        private Parameter parameter;    //对象属性

        private AnimatorStateInfo info; 
        public InjuredState(FSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            parameter.animator.Play("Hit");
            //生命值减少
            parameter.health--;
        }

        public void OnExit()
        {
            parameter.getHit = false;
        }

        public void OnUpdate()
        {
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);

            if(parameter.health <= 0)
            {
                manager.TransitionState(StateType.Death);
            }
            if(info.normalizedTime >= 0.95)
            {
                parameter.target = GameObject.FindWithTag("Player").transform;
                manager.TransitionState(StateType.Chase);
            }

        }

      

    }


    /// <summary>
    /// 死亡状态
    /// </summary>
    public class DeathState : IState
    {
        private FSM manager;    //状态机管理器
        private Parameter parameter;    //对象属性

        public DeathState(FSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
            parameter.animator.Play("Dead");
        }

        public void OnExit()
        {
            throw new System.NotImplementedException();
        }

        public void OnUpdate()
        {
            throw new System.NotImplementedException();
        }
    }


    /// <summary>
    /// 攻击状态
    /// </summary>
    public class AttackState : IState
    {
        private FSM manager;    //状态机管理器
        private Parameter parameter;    //对象属性

        private AnimatorStateInfo info;
        public AttackState(FSM manager)
        {
            this.manager = manager;
            this.parameter = manager.parameter;
        }

        public void OnEnter()
        {
        parameter.animator.Play("Attack");
        }

        public void OnExit()
        {
          
        }

        public void OnUpdate()
        {
            info = parameter.animator.GetCurrentAnimatorStateInfo(0);
            if (parameter.getHit)
            {
                manager.TransitionState(StateType.Injured);
                return;    
            }
            if(info.normalizedTime >= .95f)
            {
                manager.TransitionState(StateType.Chase);
            }
        }
    }



