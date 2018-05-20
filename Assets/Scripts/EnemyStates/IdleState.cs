using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IEnemyState
{
    private Enemy enemy;
    private float idleTimmer;
    private float idleDuration;

    public void Enter(Enemy enemy)
    {
        idleDuration = UnityEngine.Random.Range(1,3);
        this.enemy = enemy;
        int randomFacing = UnityEngine.Random.Range(0, 1);
        if (randomFacing == 1)
        {
            enemy.ChangeDirection();
        }

    }

    public void Excute()
    {
        Iddle();
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

    private void Iddle()
    {
        enemy.MyAnimator.SetFloat("speed", 0);
        idleTimmer += Time.deltaTime;
        if (idleTimmer >= idleDuration)
        {
            enemy.ChangeState(new PatrolState());
        }       
    }

    public string GetStateName()
    {
        return "Idle";
    }
}