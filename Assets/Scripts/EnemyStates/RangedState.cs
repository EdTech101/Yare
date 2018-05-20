using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedState : IEnemyState
{
    private Enemy enemy;

    public void Enter(Enemy enemy)
    {
        SoundManager.Instance.PlaySfx("goblin");
        this.enemy = enemy;

    }

    public void Excute()
    {
       
        if (enemy.InMeleeRange)
        {
            enemy.ChangeState(new MeleeState());
        }
        if (enemy.Target != null)
        {          
            enemy.Move();
        }
        else
        {
            enemy.ChangeState(new IdleState());
        }
    }

    public void Exit()
    {

    }

    public string GetStateName()
    {
        return "Ranged";
    }

    public void OnTriggerEnter(Collider2D other)
    {     
    }
}
