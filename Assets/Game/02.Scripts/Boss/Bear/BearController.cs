﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Linq;

[SelectionBase]
public class BearController : BossController
{
    public Animator animator;
    private BearStateMachine bearStateMachine;
    public BearMapInfo bearMapInfo;

    #region definitions

    [Serializable]
    public class Patterns
    {

        public List<BearPattern> phase_01_List = new List<BearPattern>();
        public List<BearPattern> phase_02_List = new List<BearPattern>();
        public List<BearPattern> phase_03_List = new List<BearPattern>();

        //public Queue<eBossState> phase_01_Queue = new Queue<eBossState>();
        //public Queue<eBossState> phase_02_Queue = new Queue<eBossState>();
        //public Queue<eBossState> phase_03_Queue = new Queue<eBossState>();
    }

    [Serializable]
    public class Colliders
    {
        public BoxCollider headCollider;
        public BoxCollider bodyCollider;

        public Vector3 headColliderSize;
        public Vector3 bodyColliderSize;
    }

    [Serializable]
    public class SkillObjects
    {
        public GameObject strikeCube;
        public GameObject roarEffect;
        public GameObject claw_A_Effect;
        public GameObject claw_B_Effect;
        public Transform clawUnderPosition;
        public Transform headParringPosition;
        public GameObject concentrateSphere;
        public GameObject smashRock;
    }

    [Serializable]
    public class SkillValue
    {
        [Tooltip("할퀴기 추가공격 개수")]
        public int clawCount = 3;

        [Tooltip("할퀴기 추가공격 간격")]
        public float clawDelay = 0.5f;

        [Tooltip("포효 투사체 개수")]
        public int roarRandCount = 7;

        [Tooltip("스매쉬 투사체 개수")]
        public int smashRandCount = 4;

        [Tooltip("집중 시간")]
        public float concentrateTime = 3;

        [Tooltip("무력화 시간")]
        public float powerlessTime = 3;
    }

    #endregion

    #region Test용
    [Tooltip("현재 상태")]
    public StateInfo stateInfo = new StateInfo();

    [Serializable]
    public class TestTextMesh
    {
        public TextMesh stateText;
        public TextMesh phaseText;
        public TextMesh hpText;
    }
    public TestTextMesh testTextMesh;
    #endregion


    public Colliders colliders;
    public SkillObjects skillObjects;

    [Header("현재 체력")]
    [Range(0, 100)]
    public float hp = 100f;
    [Header("페이즈 전환 체력")]
    public BossPhaseValue bossPhaseValue;


    [Header("스킬 세부 값")]
    public SkillValue skillValue;


    [Header("패턴 관련")]
    public Patterns patterns;

    [Tooltip("애니메이터 파라미터")]
    public Dictionary<string, int> aniHash = new Dictionary<string, int>();

    private Transform myTransform;

    private List<List<BearPattern>> phaseList = new List<List<BearPattern>>();
    private BearPattern currentPattern;

    public CustomPool<RoarProjectile> roarProjectilePool = new CustomPool<RoarProjectile>();
    public CustomPool<ClawProjectile> clawProjectilePool = new CustomPool<ClawProjectile>();
    public CustomPool<SmashProjectile> smashProjectilePool = new CustomPool<SmashProjectile>();

    private IEnumerator ProcessChangeStateTestCoroutine;

    /// <summary>
    /// 스킬 액션
    /// </summary>
    private Action skillAction = null;

    private void Awake()
    {
        Init();
    }
    private void Init()
    {
        phaseList.Add(patterns.phase_01_List);
        phaseList.Add(patterns.phase_02_List);
        phaseList.Add(patterns.phase_03_List);
        ProcessChangeStateTestCoroutine = ProcessChangeStateTest();
        Init_Animator();


        bearMapInfo.exclusionRange = 3;
        bearMapInfo.Init();

        myTransform = transform;

        //int layerMask = 1 << LayerMask.NameToLayer(str_Arrow);
        bearMapInfo.SetPhase3Position(myTransform.position);
    }
    private void Init_Animator()
    {
        BearStateMachineBehaviour[] behaviours = animator.GetBehaviours<BearStateMachineBehaviour>();

        for (int i = 0; i < behaviours.Length; i++)
        {
            behaviours[i].bearController = this;
        }

        AddAnimatorHash("Start_Idle");
        AddAnimatorHash("Start_Rush");
        AddAnimatorHash("Start_Roar");
        AddAnimatorHash("Start_Claw");
        AddAnimatorHash("Start_Strike");
        AddAnimatorHash("Phase");
        AddAnimatorHash("Start_Stamp");
        AddAnimatorHash("Start_Smash");
        AddAnimatorHash("Start_Powerless");
        AddAnimatorHash("Start_Die");
    }

    private void Init_Collider()
    {
        //충돌하지 않게 
        Physics.IgnoreCollision(colliders.headCollider, GameManager.instance.playerController.Com.collider, false);
        Physics.IgnoreCollision(colliders.bodyCollider, GameManager.instance.playerController.Com.collider, false);

        colliders.headColliderSize = colliders.headCollider.size;
        colliders.bodyColliderSize = colliders.bodyCollider.size;
    }
    private void Init_Pool()
    {
        roarProjectilePool = CustomPoolManager.Instance.CreateCustomPool<RoarProjectile>();
        clawProjectilePool = CustomPoolManager.Instance.CreateCustomPool<ClawProjectile>();
        smashProjectilePool = CustomPoolManager.Instance.CreateCustomPool<SmashProjectile>();
    }
    private void Start()
    {
        bearStateMachine = new BearStateMachine(this);
        bearStateMachine.isDebugMode = false;
        bearStateMachine.StartState(eBossState.BearState_Idle);

        Init_Pool();
        Init_Collider();
        StartCoroutine(ProcessChangeStateTestCoroutine);
    }
    private void Update()
    {
        testTextMesh.stateText.text = stateInfo.state;
        testTextMesh.hpText.text = hp.ToString();
        testTextMesh.phaseText.text = stateInfo.phase.ToString();
    }
    private bool CanChangeState()
    {
        return bearStateMachine.CanExit();
    }
    private bool ChangeState(eBossState _state)
    {
        //if (_state == eBossState.BearState_Random)
        //{
        //    bearStateMachine.ChangeState(GetRandomState(stateInfo.phase));
        //}
        //else
        //{
        bearStateMachine.ChangeState(_state);

        //}
        return false;
    }
    private void Init_ChangePhase(ePhase _phase)
    {
        switch (_phase)
        {
            case ePhase.Phase_1:
                myTransform.SetPositionAndRotation(bearMapInfo.phase2Position.position, Quaternion.Euler(Vector3.zero));

                SetStretchColliderSize();

                //투사체 위치 다시 계산
                bearMapInfo.exclusionRange = 0;
                bearMapInfo.UpdateProjectilePositions();
                bearMapInfo.InitProjectileRandArray();
                bearMapInfo.UpdateProjectileRandArray();

                break;

            case ePhase.Phase_2:
                myTransform.SetPositionAndRotation(bearMapInfo.phase3Position.position, Quaternion.Euler(new Vector3(0, 90, 0)));

                SetOriginalColliderSize();
                //투사체 위치 다시 계산
                bearMapInfo.exclusionRange = 3;
                bearMapInfo.UpdateProjectilePositions();
                bearMapInfo.InitProjectileRandArray();
                bearMapInfo.UpdateProjectileRandArray();

                break;

            //case ePhase.Phase_3:
            //    break;

            default:
                break;
        }
    }

    /// <summary>
    /// 다음 페이즈로 가기 위한 체력을 반환해줍니다.
    /// </summary>
    private float GetNextPhaseHP(ePhase _currentPhase)
    {
        switch (_currentPhase)
        {
            case ePhase.Phase_1:
                return bossPhaseValue.phase2;

            case ePhase.Phase_2:
                return bossPhaseValue.phase3;

            case ePhase.Phase_3:
                return 0;

            default:
                return 0;
        }
    }

    WaitForSecondsRealtime waitOneSec = new WaitForSecondsRealtime(1f);
    private int currentIndex = 0;
    private IEnumerator ProcessChangeStateTest()
    {
        //해야함 : 반복되는 부분 정리하고, List 3개를 Queue로 만들어서 페이즈가 지날 때마다 디큐 시켜서 자동화하기
        stateInfo.phase = ePhase.Phase_1;
        currentIndex = 0;
        int length = phaseList[stateInfo].Count;
        currentPattern = new BearPattern();

        while (true)
        {
            if (CanChangeState()) //패턴을 바꿀 수 있는 상태라면
            {
                //페이즈 전환 체크
                if (hp <= GetNextPhaseHP(stateInfo.phase))
                {
                    //체력이 0이하면 break;
                    if (hp <= 0)
                    {
                        break;
                    }

                    Init_ChangePhase(stateInfo.phase);

                    stateInfo.phase += 1;
                    currentIndex = 0;

                    length = phaseList[stateInfo].Count;
                }

                //대기 시간동안 기다림
                yield return new WaitForSeconds(currentPattern.waitTime);

                //다음 패턴 가져오기
                currentPattern = phaseList[stateInfo][currentIndex];

                if (currentPattern.state == eBossState.BearState_Random)
                {
                    currentPattern.state = GetRandomState(stateInfo.phase);

                }

                //스테이트 변경
                SetStateInfo(currentPattern.state);
                ChangeState(currentPattern.state);

                currentIndex += 1;
                currentIndex = currentIndex % length;

                yield return null;
            }
            yield return YieldInstructionCache.WaitForFixedUpdate;
        }

        SetStateInfo(eBossState.BearState_Die);
        ChangeState(eBossState.BearState_Die);

    }
    public void SetStateInfo(eBossState _state)
    {
        stateInfo.stateE = _state;
        stateInfo.state = _state.ToString();
    }
    public void SetTrigger(string _paramName)
    {
        animator.SetTrigger(aniHash[_paramName]);
    }
    /// <summary>
    /// 현재 상태의 canExit를 설정합니다.
    /// </summary>
    public void SetCanExit(bool _canExit)
    {
        bearStateMachine.currentState.canExit = _canExit;
    }

    //랜덤 범위------------
    private readonly eBossState[] patterns_phase_1
        = { eBossState.BearState_Stamp, eBossState.BearState_Strike_A, eBossState.BearState_Claw_A };
    private readonly eBossState[] patterns_phase_2
        = { eBossState.BearState_Roar_A, eBossState.BearState_Claw_B, eBossState.BearState_Strike_B };//,eBossState.BearState_Roar_B};
    private readonly eBossState[] patterns_phase_3
        = { eBossState.BearState_Strike_A, eBossState.BearState_Strike_C };
    private eBossState GetRandomState(ePhase _phase)
    {
        switch (_phase)
        {
            case ePhase.Phase_1:
                return patterns_phase_1[UnityEngine.Random.Range(0, patterns_phase_1.Length)];

            case ePhase.Phase_2:
                return patterns_phase_2[UnityEngine.Random.Range(0, patterns_phase_2.Length)];

            case ePhase.Phase_3:
                return patterns_phase_3[UnityEngine.Random.Range(0, patterns_phase_3.Length)];

            default:
                Debug.LogError("GetRandomState Error");
                return eBossState.None;
        }
    }
    public void SetSkillAction(Action _action)
    {
        skillAction = null;
        skillAction += () => Debug.Log("SkillAction!");
        skillAction += _action;
    }
    public void SkillAction()
    {
        skillAction();
    }

    #region Collider 관련
    private void SetStretchColliderSize()
    {
        colliders.headCollider.size = new Vector3(colliders.headColliderSize.x, colliders.headColliderSize.y, 10f);
        colliders.bodyCollider.size = new Vector3(colliders.bodyColliderSize.x, colliders.bodyColliderSize.y, 10f);
    }
    private void SetOriginalColliderSize()
    {
        colliders.headCollider.size = new Vector3(colliders.headColliderSize.x, colliders.headColliderSize.y, colliders.headColliderSize.z);
        colliders.bodyCollider.size = new Vector3(colliders.bodyColliderSize.x, colliders.bodyColliderSize.y, colliders.bodyColliderSize.z);
    }
    #endregion

    #region Animation 관련
    private void AddAnimatorHash(string _paramName)
    {
        aniHash.Add(_paramName, Animator.StringToHash(_paramName));
    }
    public void OnAnimStateExit()
    {
        bearStateMachine.currentState.canExit = true;
    }
    #endregion

    private readonly string str_Arrow = "Arrow";

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(str_Arrow))
        {
            hp -= 1f;
        }
    }
}

[Serializable]
public struct BearPattern
{
    [Tooltip("실행할 패턴")]
    public eBossState state;

    [Tooltip("실행 후 대기 시간")]
    public float waitTime;
}