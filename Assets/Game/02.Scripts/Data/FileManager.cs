﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    private FileBase fileBase = new FileBaseWindows();

    #region legacy
//    private void Awake()
//    {

//#if UNITY_EDITOR || UNITY_STANDALONE_WIN

//        fileBase = new FileBaseWindows();
//#endif

//#if UNITY_PS4
//                fileBase = new FileBasePS4();
//#endif

//    }
    #endregion
    public IEnumerator WriteText(string _dataName, string _data, string _path)
    {
        yield return StartCoroutine(fileBase.WriteText(_dataName, _data, _path)); // 이 코루틴이 끝날 때 까지.

    }

    [HideInInspector] public string readText_Result;

    /// <summary>
    /// 파일을 읽은 후, ,readText_Result에 저장합니다.
    /// </summary>
    /// <param name="_dataName">파일의 이름을 적습니다. 예시 : "testFile.png"  </param>
    /// <param name="_path">파일의 이름을 제외한 경로를 적습니다. 예시 : "Assets/data/" </param>
    public IEnumerator ReadText(string _dataName, string _path)
    {

        readText_Result = string.Empty;
        yield return StartCoroutine(fileBase.ReadText(_dataName, _path));
        readText_Result = fileBase.readText_Result;
    }

    [HideInInspector] public bool isExist_Result;


    /// <summary>
    /// 해당 파일이 존재하는지 확인합니다. 결과는 isExist_Result에 저장됩니다.
    /// </summary>
    /// <param name="_dataName">파일의 이름을 적습니다. 예시 : "testFile.png"  </param>
    /// <param name="_dataPath">파일의 이름을 제외한 경로를 적습니다. 예시 : "Assets/data/" </param>
    public IEnumerator IsExist(string _dataName, string _dataPath)
    {
        yield return StartCoroutine(fileBase.IsExist(_dataName, _dataPath));
        isExist_Result = fileBase.isExist_result;
    }

    /// <summary>
    /// 파일을 생성합니다.
    /// </summary>
    /// <param name="_path">파일, 파일 확장자를 포함한 전체 경로</param>
    public void CreateFile(string _path)
    {
        fileBase.CreateFile(_path);
    }
}
