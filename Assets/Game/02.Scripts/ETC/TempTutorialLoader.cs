using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempTutorialLoader : MonoBehaviour
{
    private static TempTutorialLoader instance;
    public static TempTutorialLoader Instance;


    private PlayerController player;


    private UIPlayerHP uiPlayer;
    public string currentCoName { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            instance = this;
            Instance = instance;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Debug.Log("이미 instance가 존재합니다." + this);
            if (Instance != this) //나 자신이 인스턴스가 아닐 경우
            {
                Debug.Log(this + " : 더 이상, 이 세계선에서는 존재할 수 없을 것 같아... 안녕.");
                Destroy(this.gameObject);
            }
        }
    }

    private IEnumerator Start()
    {
        yield return StartCoroutine(DataManager.Instance.LoadData_Talk("Stage_00"));
        DataManager.Instance.UpdateTalkData();

        yield return null;
        player = GameManager.instance.playerController;
        uiPlayer = UIManager.Instance.GetUI("UIPlayerHP") as UIPlayerHP;
        StartCoroutine(CoBeginTutorial());

    }

    //------------------------------------------------------

    public KeyOption key
    {
        get
        {
            return DataManager.Instance.currentData_settings.keySetting;
        }
    }


    private Dictionary<string, TutorialCollider> tcDict = new Dictionary<string, TutorialCollider>();
    private Dictionary<string, IEnumerator> coDict = new Dictionary<string, IEnumerator>();

    private const float talkTime = 3f;
    private WaitForSeconds wait3sec = new WaitForSeconds(3f);
    private WaitForSeconds waitSsec = new WaitForSeconds(0.1f);
    private UIGameMessage gameMessage;

    public void AddDict(string _name, TutorialCollider _tc)
    {
        tcDict.Add(_name, _tc);
    }

    public void CloseCollider(string _name)
    {
        tcDict[_name].Close();
    }

    public void StartCoPrac(string _coName)
    {
        StopCoroutine(currentCoName);
        currentCoName = _coName;
        StartCoroutine(_coName);
    }
    private void CanMove(bool _b)
    {
        if (_b)
        {
            uiPlayer.Open();
        }
        else
        {
            uiPlayer.Close();
        }

        player.State.isCrouching = false;
        player.InputVal.movementInput = 0f;
        player.State.moveSystem = !_b;
    }
    /// <summary>
    /// 튜토리얼에 처음 입장했을 때 뜨는 대사
    /// </summary>
    private IEnumerator CoBeginTutorial()
    {
        //CloseCollider("CoBeginTutorial");
        yield return YieldInstructionCache.WaitForEndOfFrame;

        gameMessage = UIManager.Instance.GetUI("UIGameMessage") as UIGameMessage;
        gameMessage.SetWaitTime(100f);

        MessageOpen("[스페이스 바]키로 대화를 스킵할 수 있습니다.");

        CanMove(false);
        Talk("아차, 자기소개를 깜빡했네! 내 이름은 루미에야.");
        yield return StartCoroutine(CoWaitTalkEnd());

        Talk("숲을 되돌리기 위해선, 정화의 힘이 필요해.");
        yield return StartCoroutine(CoWaitTalkEnd());

        Talk("걱정마! 너에게 내 정화의 힘을 조금 나눠줄게.");
        yield return StartCoroutine(CoWaitTalkEnd());

        Talk("아무래도, 처음부터 힘을 완벽히 다루기는 힘들겠지?");
        yield return StartCoroutine(CoWaitTalkEnd());


        Talk("차근차근 하나씩 연습해보자.");
        yield return new WaitForSeconds(1f);

        StartCoroutine(CoPrac_Move());

    }


    private IEnumerator CoPrac_Move()
    {
        //CloseCollider("CoPrac_Move");
        GameManager.instance.playerController.Com.pixy.getPixy = true;
        Talk("일단, 몸을 좀 움직여볼까?");
        MessageOpen("화살표 [←],[→] 키로 이동할 수 있습니다.");
        CanMove(true);

        while (true)
        {

            if (GetKey(key.moveRight) || GetKey(key.moveLeft))
            {
                break;
            }
            yield return null;
        }
        //TalkEnd();
        Talk("좋아! 이대로 저어~기 통나무 있는 곳 까지 가보자!");
    }

    private IEnumerator CoPrac_Jump()
    {
        CloseCollider("CoPrac_Jump");

        MessageOpen("[X]키로 점프할 수 있습니다.");

        Talk("이 정도라면 뛰어넘을 수 있을거야. 한 번 뛰어볼까?");

        while (!GetKey(key.jump))
        {
            yield return null;
        }

        Talk("흠, 사지는 멀쩡한가보네….\n비실비실해보여서 불안했는데 말이지….");
        yield return StartCoroutine(CoWaitTalkEnd());

        Talk("앗? 아니야! 아무 말도 안했어^_^!");
        yield return StartCoroutine(CoWaitTalkEnd());

        TalkInfinity("좀 더 앞으로 가볼까? ^_^");
        
        MessageClose();
    }

    private IEnumerator CoPrac_Crouch()
    {
        CloseCollider("CoPrac_Crouch");

        MessageClose();
        CanMove(false);
        Talk("여긴 길이 좀 낮네….");
        yield return StartCoroutine(CoWaitTalkEnd());

        MessageOpen("[↓]키로 웅크릴 수 있습니다.");
        CanMove(true);

        Talk("이피아는 나보다 훨씬 덩치가 크니까, \n몸을 웅크려서 지나가야할거야.");
        yield return StartCoroutine(CoWaitTalkEnd());

        while (!GetKey(key.crouch))
        {
            yield return null;
        }
        TalkInfinity("좋아, 천천히 지나가자.");
    }

    private IEnumerator CoPrac_Attack_Default()
    {
        MessageClose();
        CloseCollider("CoPrac_Attack_Default");
        Talk("우와앗, 토끼다!");
        yield return StartCoroutine(CoWaitTalkEnd());

        Talk("겉모습은 좀 멀쩡해보여도,\n정신은 이미 어둠에 물들어버렸어….");
        yield return StartCoroutine(CoWaitTalkEnd());

        Talk("이피아, 정화의 힘을 사용할 때야.");

        MessageOpen("[Z]키로 공격할 수 있습니다.");

        while (!GetKey(key.attack))
        {
            yield return null;
        }

        //Talk("정화의 화살이라니, 멋있는걸!");
    }

    private IEnumerator CoPrac_Attack_Up()
    {
        CloseCollider("CoPrac_Attack_Up");
        MessageOpen("[↑]키로 위를 조준할 수 있습니다. \n[Z]키를 함께 사용하면 위를 향해 공격할 수 있습니다.");

        while (!GetKey(key.lookUp))
        {
            yield return null;
        }


        //Talk("이피아는 석궁을 잘 다루는구나? 능숙해보여.");
    }

    private IEnumerator CoPrac_Parrying()
    {
        CloseCollider("CoPrac_Parrying");

        Talk("이건, 한 번 뛴다고 지나갈 수 있는 높이가 아니네….");
        yield return StartCoroutine(CoWaitTalkEnd());

        Talk("저 무시무시한 가시공이 보이니? \n저걸 딛고 뛴다면 넘어갈 수 있을거야.");
        yield return StartCoroutine(CoWaitTalkEnd());

        Talk("걱정마! 정화의 힘을 사용한다면\n상처없이 지나갈 수 있을거야.");
        yield return StartCoroutine(CoWaitTalkEnd());

        MessageOpen("점프 중, 적과 닿았을 때 [X]키를 사용하면 \n'패링'을 하여 연속 점프를 할 수 있습니다. ");

        while (!GameManager.instance.playerController.State.isParrying)
        {
            yield return null;
        }

        Talk("좋았어! 이대로 지나가보자.");

        MessageOpen("'패링'은 땅에 닿기 전까지\n몇 번이고 연속해서 사용할 수 있습니다. ");
    }

    private IEnumerator CoPrac_Attack_Power()
    {
        CloseCollider("CoPrac_Attack_Power");

        Talk("숲 속에서 사용한 정화의 힘은 나에게 다시 돌아오게 돼. \n몰랐지?");
        yield return StartCoroutine(CoWaitTalkEnd());

        Talk("반환된 힘은 차곡차곡 모아둘테니, \n이피아가 필요할 때 말하면 도움이 될만한 일을 해볼게.");
        yield return StartCoroutine(CoWaitTalkEnd());

        Talk("시험 삼아서 한 번 해볼까?");
        yield return StartCoroutine(CoWaitTalkEnd());

        Talk("앞의 통나무를 때려서 정화의 힘을 반환시켜봐.");
        yield return StartCoroutine(CoWaitTalkEnd());

        MessageOpen("몬스터에게 공격을 적중시키거나 '패링'을 성공시키면\n좌측 상단 UI의 정화 게이지를 획득할 수 있습니다.");
        yield return StartCoroutine(CoWaitTalkEnd());
        MessageOpen("게이지를 일정량 획득하여 꽃이 한 송이씩 필 때마다, \n루미에의 '강공격'을 사용할 수 있습니다.");
        while (!GameManager.instance.playerController.Com.pixy.isAttack)
        {
            yield return null;
        }
        MessageOpen("강공격은 몬스터나 지형을 관통할 수 있습니다. ");

        Talk("봤지? 난 그냥 따라다니기만 하는 마스코트가 아니야!");
    }

    private IEnumerator CoPrac_Attack_Ult()
    {

        CloseCollider("CoPrac_Attack_Ult");

        Talk("힘을 조금만 더 모으면,\n 방금 것보다 대단한 걸 할 수 있어.");
        yield return StartCoroutine(CoWaitTalkEnd());

        yield return null;

        MessageOpen("정화 게이지를 끝까지 채워 꽃이 주황색으로 물들면,\n 루미에의 '궁극기'를 사용할 수 있습니다.");

        while (!GameManager.instance.playerController.Com.pixy.isUlt)
        {
            yield return null;
        }

        MessageOpen("궁극기를 사용하고 있을 때에는 정화 게이지를 획득할 수 없습니다.");

        Talk("우오오! 다 정화시켜버리겠어!");
    }

    private void Talk(string _str)
    {
        UIManager.Instance.Talk(_str, talkTime);
    }
    private void TalkInfinity(string _str)
    {
        UIManager.Instance.TalkInfinity(_str);
    }
    private void TalkEnd()
    {
        UIManager.Instance.TalkEnd();
    }


    private IEnumerator CoWaitTalkEnd()
    {
        float timer = 0f;

        while (timer < talkTime)
        {
            timer += Time.deltaTime;
            if (IsInputSkipKey())
            {
                yield return waitSsec;
                yield break;
            }
            yield return null;
        }
    }


    private void MessageOpen(string _str)
    {
        gameMessage.Open(_str);
    }

    private void MessageClose()
    {
        gameMessage.Close();
    }
    private bool GetKey(KeyCode _keyCode) => Input.GetKey(_keyCode);
    private bool GetKeyDown(KeyCode _keyCode) => Input.GetKey(_keyCode);

    /// <summary>
    /// 대화를 스킵할 수 있는 키가 눌렸는가?
    /// </summary>
    private bool IsInputSkipKey()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void OnDestroy()
    {
        coDict = null;
        tcDict = null;
    }
}
