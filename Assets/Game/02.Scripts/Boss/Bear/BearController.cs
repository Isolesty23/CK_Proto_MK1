﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
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
    public class SkillObjects
    {
        public GameObject strikeCube;
        public GameObject roarCube;
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

    public SkillObjects skillObjects;
    [Header("현재 체력")]
    [Range(0, 100)]
    public float hp = 100f;
    [Header("페이즈 전환 체력")]
    public BossPhaseValue bossPhaseValue;

    [Header("패턴 목록")]
    public Patterns patterns;

    [Tooltip("애니메이터 파라미터")]
    public Dictionary<string, int> aniHash = new Dictionary<string, int>();

    /// <summary>
    /// 스킬 액션
    /// </summary>
    private Action skillAction;

    private void Awake()
    {
        Init();
        Init_Animator();
        bearMapInfo.Init();
    }
    private void Init()
    {
        //int length = patterns.phase_01_List.Count;
        //for (int i = 0; i < length; i++)
        //{
        //    patterns.phase_01_Queue.Enqueue(patterns.phase_01_List[i]);
        //}

        //length = patterns.phase_02_List.Count;
        //for (int i = 0; i < length; i++)
        //{
        //    patterns.phase_02_Queue.Enqueue(patterns.phase_02_List[i]);
        //}

        //length = patterns.phase_03_List.Count;
        //for (int i = 0; i < length; i++)
        //{
        //    patterns.phase_03_Queue.Enqueue(patterns.phase_03_List[i]);
        //}
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
    }
    private void Start()
    {
        bearStateMachine = new BearStateMachine(this);
        bearStateMachine.isDebugMode = true;
        bearStateMachine.StartState(eBossState.BearState_Idle);
        StartCoroutine(ProcessChangeStateTest());
    }
    private void Update()
    {
        testTextMesh.stateText.text = stateInfo.state;
        testTextMesh.hpText.text = hp.ToString();
        testTextMesh.phaseText.text = stateInfo.phase.ToString();
    }
    private bool ChangeState(eBossState _state)
    {
        if (_state == eBossState.BearState_Random)
        {
            bearStateMachine.ChangeState((eBossState)UnityEngine.Random.Range(0, (int)eBossState.BearState_Random));
        }
        else
        {
            bearStateMachine.ChangeState(_state);

        }
        return false;
    }

    private bool CanChangeState()
    {
        return bearStateMachine.CanExit();
    }

    private void NextPhase()
    {
        stateInfo.phase = stateInfo.phase + 1;
    }
    WaitForSecondsRealtime waitOneSec = new WaitForSecondsRealtime(1f);
    private IEnumerator ProcessChangeStateTest()
    {
        //해야함 : 반복되는 부분 정리하고, List 3개를 Queue로 만들어서 페이즈가 지날 때마다 디큐 시켜서 자동화하기
        bool thisPhase = true;
        int i = 0;
        int length = patterns.phase_01_List.Count;
        stateInfo.phase = ePhase.Phase_1;
        while (thisPhase)
        {
            i = i % length;
            // if (ChangeState(patterns.phase_01_List[i].state))
            if (CanChangeState()) //패턴을 바꿀 수 있는 상태라면
            {
                //페이즈 전환 체크
                if (hp <= bossPhaseValue.phase2)
                {
                    break;
                }

                //아직 현재 페이즈에 머무를수 있다면
                //대기 시간동안 기다림
                yield return new WaitForSeconds(patterns.phase_01_List[i].waitTime);

                //스테이트 변경
                ChangeState(patterns.phase_01_List[i].state);
                stateInfo.state = bearStateMachine.GetCurrentStateName();

                //다음 패턴 불러오기
                i += 1;

                yield return null;
            }
            yield return YieldInstructionCache.WaitForFixedUpdate;
        }
        //while (thisPhase)
        //{
        //    i = i % length;
        //    if (ChangeState(patterns.phase_01_List[i].state))
        //    {
        //        stateInfo.state = patterns.phase_01_List[i].ToString();
        //        i += 1;
        //        if (hp <= bossPhaseValue.phase2)
        //        {
        //            thisPhase = false;
        //        }
        //        yield return waitOneSec;
        //    }
        //    yield return YieldInstructionCache.WaitForFixedUpdate;
        //}

        thisPhase = true;
        i = 0;
        length = patterns.phase_02_List.Count;
        stateInfo.phase = ePhase.Phase_2;
        while (thisPhase)
        {
            i = i % length;
            if (ChangeState(patterns.phase_02_List[i].state))
            {
                stateInfo.state = patterns.phase_02_List[i].ToString();
                Debug.Log("현재 인덱스 " + (i));
                i += 1;
                if (hp <= bossPhaseValue.phase3)
                {
                    thisPhase = false;
                }
                yield return waitOneSec;
            }
            yield return YieldInstructionCache.WaitForFixedUpdate;
        }

        thisPhase = true;
        i = 0;
        length = patterns.phase_03_List.Count;
        stateInfo.phase = ePhase.Phase_3;
        while (thisPhase)
        {
            i = i % length;
            if (ChangeState(patterns.phase_03_List[i].state))
            {
                stateInfo.state = patterns.phase_03_List[i].ToString();
                Debug.Log("현재 인덱스 " + (i));
                i += 1;
                yield return waitOneSec;
            }
            yield return YieldInstructionCache.WaitForFixedUpdate;
        }
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

    #region Animation 관련
    private void AddAnimatorHash(string _paramName)
    {
        aniHash.Add(_paramName, Animator.StringToHash(_paramName));
    }
    public void AnimatorPlay(string _pathAndName)
    {
        animator.Play(_pathAndName, 0, 0f);
    }
    public void OnAnimStateExit()
    {
        bearStateMachine.currentState.canExit = true;
    }
    #endregion
}
[Serializable]
public struct BearPattern
{
    [Tooltip("대기 시간")]
    public float waitTime;

    [Tooltip("실행할 패턴")]
    public eBossState state;
}