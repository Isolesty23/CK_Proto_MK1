using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LarvaController : MonsterController
{
    #region
    [SerializeField] private Components components = new Components();
    [SerializeField] private MonsterStatus monsterStatus = new MonsterStatus();

    public Components Com => components;
    public MonsterStatus Stat => monsterStatus;
    public MonsterState state = MonsterState.Search;

    public float upDownSpeed;
    public float upDelay;
    public float downRange;
    public bool isRunninCo;
    #endregion

    private Vector3 pos;
    private Vector3 destPos;

    public enum AttackState
    {
        Wait,
        Down,
        Up
    }

    public AttackState attackState = AttackState.Wait;

    void Start()
    {
        pos = gameObject.transform.position;
        destPos = gameObject.transform.position - new Vector3(0, downRange, 0);
    }

    void Update()
    {
        State(state);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Bullet"))
        {
            other.gameObject.SetActive(false);
            if (Stat.hp > 1)
                Stat.hp--;
            else
                ChangeState("Dead");
        }
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
        if(functionName == "Search")
        {
            state = MonsterState.Search;
        }
        else if(functionName == "Chase")
        {
            state = MonsterState.Chase;
        }
        else if(functionName == "Attack")
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
        if(gameObject.transform.position.y == pos.y)
        {
            if(isRunninCo == false)
                StartCoroutine(UpDownDelay(0));
        }
        else if(gameObject.transform.position.y == destPos.y)
        {
            if (isRunninCo == false)
                StartCoroutine(UpDownDelay(1));
        }

        switch (attackState)
        {
            case AttackState.Wait:
                break;
            case AttackState.Down:
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, destPos, upDownSpeed);
                isRunninCo = false;
                break;
            case AttackState.Up:
                gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, pos, upDownSpeed);
                isRunninCo = false;
                break;
            default:
                break;
        }
    }
    protected override void Dead()
    {
        base.Dead();
        //gameObject.SetActive(false);
    }

    private IEnumerator UpDownDelay(int n)
    {
        isRunninCo = true;
        attackState = AttackState.Wait;
        yield return new WaitForSeconds(upDelay);
        if (n == 0)
            attackState = AttackState.Down;
        else
            attackState = AttackState.Up;
    }

}
