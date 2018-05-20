using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IEnemyState
{
    private Enemy enemy;
    private float patrolTimmer;
    private float patrolDuration;

    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;

    }

    public void Excute()
    {
        patrolDuration = UnityEngine.Random.Range(5, 10);
        Patrol();
        enemy.Move();
        if (enemy.Target != null)
        {
            enemy.ChangeState(new RangedState());
        }
    }

    public void Exit()
    {

    }

    public void OnTriggerEnter(Collider2D other)
    {
    
    }

    private void Patrol()
    {   
        patrolTimmer += Time.deltaTime;
        if (patrolTimmer >= patrolDuration)
        {
            enemy.ChangeState(new IdleState());
        }
    }

    public string GetStateName()
    {
        return "Patrol";
    }
}

