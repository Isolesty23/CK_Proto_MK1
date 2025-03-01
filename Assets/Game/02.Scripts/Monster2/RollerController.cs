using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RollerController : MonsterController
{
    #region
    [Serializable]
    public class RollerStatus
    {
        public float jumpPower;
        public float aclrt; // ���ӵ�
        public float maxSpeed;

        [Header("Sub Status")]
        public float moveChangeTime;
        public float changeDelay;
        public int rollingDamage;

        public bool isJustMove;
    }

    [Serializable]
    public class RollerComponents
    {
        //instance
        public ParticleSystem VFX_change;
        public ParticleSystem VFX_rush;

        public ConstantForce constantForce;
        public SphereCollider sphereCollider;
        public CapsuleCollider capsuleCollider;
        public SphereCollider rollingCollider;
        public Collider attackCollider;
    }

    [SerializeField] private RollerStatus rollerStatus = new RollerStatus();
    [SerializeField] private RollerComponents rollerComponents = new RollerComponents();

    public RollerStatus Stat2 => rollerStatus;
    public RollerComponents Com2 => rollerComponents;

    private float movePatternTime;
    private int random;
    [HideInInspector] public Vector3 moveDir;
    private float currentSpeed;
    private float usingAclrt;
    #endregion
    public override void Initialize()
    {
        base.Initialize();
        Com.rigidbody.velocity = Vector3.zero;
        Com2.rollingCollider.gameObject.SetActive(false);
        Com.animator.SetBool("isAttack", false);
        Com.animator.SetBool("isJump", false);
        Com.animator.SetBool("isMove", false);
        Com.animator.SetFloat("AttackSpeed", 0.0f);
        movePatternTime = 10f;
        Com.collider.enabled = true;
        Com2.sphereCollider.enabled = false;
        transform.localEulerAngles = Vector3.zero;
        Com2.attackCollider.gameObject.SetActive(true);
        currentSpeed = 0f;
        usingAclrt = Stat2.aclrt;
        Com2.constantForce.enabled = true;
        Com2.VFX_rush.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        Com2.VFX_change.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        if (Stat2.isJustMove)
            Com2.attackCollider.gameObject.SetActive(false);
    }

    public override void Awake()
    {
        base.Awake();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void State(MonsterState state)
    {
        base.State(state);
    }

    public override void ChangeState(MonsterState state)
    {
        base.ChangeState(state);
    }

    protected override void Idle()
    {
        base.Idle();
        ChangeState(MonsterState.MOVE);
    }

    protected override void Move()
    {
        base.Move();

        movePatternTime += Time.deltaTime;



        if(movePatternTime > Stat2.moveChangeTime)
        {
            if(!Stat2.isJustMove)
                random = UnityEngine.Random.Range(0, 3);
            else
            {
                if (transform.localEulerAngles == Vector3.zero)
                    random = 1;
                else
                    random = 2;
            }
            movePatternTime = 0f;
        }

        if (random == 0f)
        {
            Com.animator.SetBool("isMove", false);
            return;
        }
        else if (random == 1)
        {
            Com.animator.SetBool("isMove", true);
            Com.rigidbody.velocity = new Vector3(-Stat.moveSpeed, Com.rigidbody.velocity.y, 0);
            transform.localEulerAngles = Vector3.zero;
        }
        else if (random == 2)
        {
            Com.animator.SetBool("isMove", true);
            Com.rigidbody.velocity = new Vector3(Stat.moveSpeed, Com.rigidbody.velocity.y, 0);
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }

        if (!Physics.Raycast(transform.position - Vector3.left * Com2.capsuleCollider.height / 2, Vector3.down, 1, LayerMask.GetMask("Ground")))
        {
            if(random == 1)
            {
                random = 2;
                movePatternTime = 0f;
            }
            else if(random == 2)
            {
                random = 1;
                movePatternTime = 0f;
            }
        }
    }

    protected override void Detect()
    {
        base.Detect();
        Com.animator.SetBool("isJump", true);
        Com.rigidbody.velocity = Vector3.zero;

        var changeMode = ChangeMode();
        StartCoroutine(changeMode);

        ChangeState(MonsterState.ATTACK);
    }

    IEnumerator ChangeMode()
    {
        moveDir = new Vector3();

        if (transform.position.x > GameManager.instance.playerController.transform.position.x)
        {
            moveDir = new Vector3(-1, 0, 0);
            transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            moveDir = new Vector3(1, 0, 0);
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }

        Com.rigidbody.AddForce(Vector3.up * Stat2.jumpPower, ForceMode.Impulse);

        yield return new WaitForSeconds(Stat2.changeDelay);

        Com2.VFX_change.Play();

        //instance
        Com.animator.SetBool("isAttack", true);
        //Com2.particle.Play();
        Com2.VFX_rush.Play();

        Com.collider.enabled = false;
        Com2.sphereCollider.enabled = true;
        Com2.rollingCollider.gameObject.SetActive(true);

        if (!Com.audio.isPlaying && Time.timeScale != 0)
        {
            Com.audio.loop = true;
            Com.audio.Play();
        }
    }

    protected override void Attack()
    {
        base.Attack();

        currentSpeed = Mathf.Clamp(currentSpeed += usingAclrt * Time.deltaTime, 0f, Stat2.maxSpeed);
        Com.animator.SetFloat("AttackSpeed", currentSpeed * 0.4f);
        var layDir = new Vector3();

        //Com2.VFX_rush.Play();

        if (moveDir.x < 0)
        {
            Com.rigidbody.velocity = new Vector3(-currentSpeed, Com.rigidbody.velocity.y, 0);
            layDir = Vector3.left;
        }
        else
        {
            Com.rigidbody.velocity = new Vector3(currentSpeed, Com.rigidbody.velocity.y, 0);
            layDir = Vector3.right;
        }



        if (Physics.Raycast(transform.position, layDir, Com2.sphereCollider.radius + 0.1f, LayerMask.GetMask("Ground")))
        {
            if (moveDir.x < 0)
            {
                moveDir = new Vector3(1, 0, 0);
                transform.localEulerAngles = new Vector3(0, 180, 0);
            }
            else
            {
                moveDir = new Vector3(-1, 0, 0);
                transform.localEulerAngles = Vector3.zero;
            }
        }

    }

    public override void Hit(int damage)
    {
        base.Hit(damage);
    }

    protected override void Death()
    {
        Com.animator.SetBool("isAttack", false);
        Com2.constantForce.enabled = false;
        base.Death();
        Com2.VFX_rush.Stop();
        Com2.VFX_change.Stop();
    }

    protected override void HandleAnimation()
    {
        base.HandleAnimation();
    }
}