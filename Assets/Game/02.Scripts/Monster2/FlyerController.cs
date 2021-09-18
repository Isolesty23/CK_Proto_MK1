using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class FlyerController : MonsterController
{
    #region
    [Serializable]
    public class FlyerStatus
    {
        public float patrolRange;
        public float patrolTime = 2f;

        public float shotSpeed;
        public float featherRange;
        public float attackCoolTime;
        public float tripleShotDelay;

        public float attackTime;

        //[Header("Sub Status")]
        [HideInInspector] public bool isAttack;
        [HideInInspector] public Vector3 patrolPos1;
        [HideInInspector] public Vector3 patrolPos2;
    }

    [Serializable]
    public class FlyerComponents
    {


    }

    [SerializeField] private FlyerStatus flyerStatus = new FlyerStatus();
    [SerializeField] private FlyerComponents flyerComponents = new FlyerComponents();
    public FlyerStatus Stat2 => flyerStatus;
    public FlyerComponents Com2 => flyerComponents;

    public Tween tween;
    private float cooltime;
    private bool moveTrigger;

    #endregion

    public override void Awake()
    {
        base.Awake();
    }

    public override void Start()
    {
    }

    public override void Initialize()
    {
        base.Initialize();
        Stat2.patrolPos1 = transform.position;
        Stat2.patrolPos2 = transform.position + Vector3.left * Stat2.patrolRange;
        cooltime = 10f;
        moveTrigger = true;
    }

    public override void Update()
    {
        base.Update();
    }

    public override void State(MonsterState state)
    {
        base.State(state);
    }

    public override void ChangeState(MonsterState functionName)
    {
        base.ChangeState(functionName);
    }

    protected override void Idle()
    {
        base.Idle();

        moveTrigger = true;

        ChangeState(MonsterState.MOVE);
    }

    protected override void Move()
    {
        base.Move();

        if (Stat2.isAttack)
            return;

        if (transform.position.x == Stat2.patrolPos2.x)
        {
            Utility.KillTween(tween);
            tween = transform.DOMove(Stat2.patrolPos1, Stat2.patrolTime).SetEase(Ease.InOutCubic);
            tween.Play();
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else if (transform.position.x == Stat2.patrolPos1.x)
        {
            Utility.KillTween(tween);
            tween = transform.DOMove(Stat2.patrolPos2, Stat2.patrolTime).SetEase(Ease.InOutCubic);
            tween.Play();
            transform.eulerAngles = Vector3.zero;
        }
        else
        {
            if (moveTrigger)
            {
                var pos1 = Mathf.Abs(transform.position.x - Stat2.patrolPos1.x);
                var pos2 = Mathf.Abs(transform.position.x - Stat2.patrolPos2.x);
                if (pos1 > pos2)
                {
                    Utility.KillTween(tween);
                    tween = transform.DOMove(Stat2.patrolPos1, Stat2.patrolTime).SetEase(Ease.InOutCubic);
                    tween.Play();
                    transform.eulerAngles = new Vector3(0, 180, 0);
                }
                else if (pos2 > pos1)
                {
                    Utility.KillTween(tween);
                    tween = transform.DOMove(Stat2.patrolPos2, Stat2.patrolTime).SetEase(Ease.InOutCubic);
                    tween.Play();
                    transform.eulerAngles = Vector3.zero;
                }
                moveTrigger = false;
            }
        }
    }

    protected override void Detect()
    {
        base.Detect();

        //ready to attack
        Utility.KillTween(tween);
        var targetDir = transform.position.x - GameManager.instance.playerController.transform.position.x;
        if (targetDir >= 0)
            targetDir = 1f;
        else
            targetDir = -1f;

        var targetPos = transform.position - new Vector3(targetDir, 0, 0) * 0.5f;
        tween = transform.DOMove(targetPos, 0.5f).SetEase(Ease.Unset);

        cooltime = Stat2.attackCoolTime -0.5f;

        ChangeState(MonsterState.ATTACK);
    }

    protected override void Attack()
    {
        base.Attack();

        //rotate
        if (transform.position.x > GameManager.instance.playerController.transform.position.x)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }

        //attack
        cooltime += Time.deltaTime;

        if (cooltime > Stat2.attackCoolTime)
        {

            //animation time
            Stat2.isAttack = true;
            var checkAttack = CheckAttack();
            StartCoroutine(checkAttack);

            int attackType = UnityEngine.Random.Range(0, 10);

            var targetPos = GameManager.instance.playerController.transform.position;

            if (attackType >= 0 && attackType < 7)
            {
                // Normal Attack
                NormalShot(targetPos);
            }
            else
            {
                // Triple Attack
                var tripleShot = TripleShot(targetPos);
                StartCoroutine(tripleShot);
            }

            cooltime = 0f;
        }
    }

    public void NormalShot(Vector3 targetPos)
    {
        var feather = CustomPoolManager.Instance.featherPool.SpawnThis(transform.position, Vector3.zero, null);
        feather.transform.LookAt(targetPos);
        feather.isActive = true;
        var normalShot = feather.Shot(transform.position, Stat2.shotSpeed, Stat2.featherRange);
        StartCoroutine(normalShot);
    }

    public IEnumerator TripleShot(Vector3 targetPos)
    {
        NormalShot(targetPos);
        yield return new WaitForSeconds(Stat2.tripleShotDelay);
        NormalShot(targetPos);
        yield return new WaitForSeconds(Stat2.tripleShotDelay);
        NormalShot(targetPos);
    }

    public IEnumerator CheckAttack()
    {
        yield return new WaitForSeconds(Stat2.attackTime);

        Stat2.isAttack = false;
    }

    public override void Hit(int damage)
    {
        base.Hit(damage);
    }

    protected override void Death()
    {
        base.Death();
    }

    protected override void HandleAnimation()
    {
        base.HandleAnimation();
    }
}
