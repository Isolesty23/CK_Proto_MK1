using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonsterController
{
    public float moveSpeed;

    void Start()
    {
        
    }
    
    void Update()
    {
        State(state);
    }

    public void State(MonsterState state)
    {
        switch (state)
        {
            case MonsterState.Search:
                Search();
                break;

            case MonsterState.Chase:
                Chase();
                break;

            case MonsterState.Attack:
                Attack();
                break;

            case MonsterState.Dead:
                Dead();
                break;

            default:
                break;
        }
    }

    public void ChangeState(string functionName)
    {
        if (functionName == "Search")
        {
            state = MonsterState.Search;
        }
        else if (functionName == "Chase")
        {
            state = MonsterState.Chase;
        }
        else if (functionName == "Attack")
        {
            state = MonsterState.Attack;
        }
        else if (functionName == "Dead")
        {
            state = MonsterState.Dead;
        }
    }
    protected override void Search()
    {
        base.Search();
    }

    protected override void Chase()
    {
        base.Chase();
    }

    protected override void Attack()
    {
        base.Attack();
        transform.Translate(Vector3.left * Time.deltaTime * moveSpeed);
    }
    protected override void Dead()
    {
        base.Dead();
    }

    public void Hitted()
    {
        if (Stat.hp > 1)
            Stat.hp--;
        else
            ChangeState("Dead");
    }
}
