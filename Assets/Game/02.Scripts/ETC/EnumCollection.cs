﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePhase
{
    Phase_1,
    Phase_2,
    //  Phase_3,
    //  Phase_Finish
}

public enum eBearState
{
    None,
    [InspectorName("대기")]
    Idle,

    [InspectorName("발구르기")]
    Stamp,

    [InspectorName("포효_A")]
    Roar_A,
    [InspectorName("포효_B")]
    Roar_B,

    [InspectorName("내려치기_A")]
    Strike_A,
    [InspectorName("내려치기_B")]
    Strike_B,

    [InspectorName("할퀴기_A")]
    Claw_A,
    [InspectorName("할퀴기_B")]
    Claw_B,

    [InspectorName("스매쉬")]
    Smash,

    [InspectorName("집중")]
    Concentrate,


    [InspectorName("랜덤 1번")]
    Random_1,

    [InspectorName("랜덤 2번")]
    Random_2,

    [InspectorName("랜덤 3번")]
    Random_3,

    [InspectorName("랜덤 4번")]
    Random_4,


    Powerless,

    [InspectorName("페이즈2 전환 연출(돌진)")]
    Rush,

    [InspectorName("죽음")]
    Die
}

public enum eGloomState
{
    [InspectorName("대기/(None)")]
    None,

    [InspectorName("대기/(Idle_사용금지)")]
    Idle,

    //[InspectorName("추격")]
    Chase,

    //[InspectorName("도약")]
    Leap,

    [InspectorName("위협")]
    Threat,

    [InspectorName("가시숲")]
    ThornForest,

    [InspectorName("방해")]
    Obstruct,

    [InspectorName("가시밭길")]
    ThornPath,

    //[InspectorName("소환")]
    Summon,

    [InspectorName("광폭화(사용금지)")]
    Berserk,

    [InspectorName("죽음(사용금지)")]
    Die

}
public enum eUIText
{
    StartNewGame,
    NoPlayerData,
    DataDelete,
    DataSave,
    Exit,


}

public enum eResolutionType
{
    /// <summary>
    /// 1280x720
    /// </summary>
    HD,

    /// <summary>
    /// 1920x1080
    /// </summary>
    FHD,

    /// <summary>
    /// 3840x2160
    /// </summary>
    UHD,
}

public enum eDataManagerState
{

    DEFAULT,

    /// <summary>
    /// 데이터 파일 검사 중
    /// </summary>
    CHECK,

    /// <summary>
    /// 데이터 파일을 생성하는 중
    /// </summary>
    CREATE,

    /// <summary>
    /// 데이터 파일을 읽는 중
    /// </summary>
    LOAD,

    /// <summary>
    /// 데이터 파일을 적용하는 중
    /// </summary>
    SAVE,

    /// <summary>
    /// 작업 완료
    /// </summary>
    FINISH,

    /// <summary>
    /// 무...무슨일이지?
    /// </summary>
    ERROR,

}


public enum eDiretion
{
    Left,
    Right,
    Up,
    Down
}


[System.Serializable]
public class BoxColliderInfo
{
    public Vector3 center;
    public Vector3 size;
}

[System.Serializable]
public class MapBlock
{
    [System.Serializable]
    public class Position
    {
        [ReadOnly]
        public Vector3 min;

        [ReadOnly]
        public Vector3 max;

        [ReadOnly]
        public Vector3 groundCenter;

        [ReadOnly]
        public Vector3 topCenter;
    }
    public enum eType
    {
        None,
        Empty,
        Used,
    }
    private eType originType;
    [ReadOnly]
    public eType currentType;

    public Position position = new Position();

    public void SetOriginType(eType _type)
    {
        originType = _type;
    }
    public void SetMinMax(Vector3 _min, Vector3 _max)
    {
        position.min = _min;
        position.max = _max;
    }

    public void SetGroundCenter(Vector3 _groundCenter)
    {
        position.groundCenter = _groundCenter;
    }
    public void SetTopCenter(Vector3 _topCenter)
    {
        position.topCenter = _topCenter;
    }

    public void SetCurrentType(eType _type)
    {
        currentType = _type;
    }
    public void SetCurrentTypeToOrigin()
    {
        currentType = originType;
    }

}

/// <summary>
/// 데미지를 받을 수 있는 오브젝트에게 상속합니다.
/// </summary>
public interface IDamageable
{
    public void OnHit();
    public void ReceiveDamage();
}
public class EnumCollection : MonoBehaviour
{

}
