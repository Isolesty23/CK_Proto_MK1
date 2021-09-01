﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
/// <summary>
/// Windows OS에서의 FileBase입니다.
/// </summary>
public class FileBaseWindows : FileBase
{

//#if UNITY_EDITOR || UNITY_STANDALONE_WIN
    public override IEnumerator WriteText(string _dataName, string _data, string _path)
    {
        string path = _path + _dataName;

        File.WriteAllText(path, _data);
        yield break;
    }
    /// <summary>
    /// 텍스트 관련 파일을 읽어옵니다. 결과는 readText_Result에 저장됩니다.
    /// </summary>
    /// <param name="_dataName">파일의 이름. 예시 : testFile.png</param>
    public override IEnumerator ReadText(string _dataName, string _path)
    {
        readText_Result = string.Empty;

        string path = _path + _dataName;

        FileInfo fileInfo = new FileInfo(path);
        if (fileInfo.Exists == true)
        {
            readText_Result = File.ReadAllText(path);
        }
        else
        {
            Debug.LogWarning("파일이 존재하지 않습니다." + " [" + _dataName + "]");
        }

        yield break;
    }


    /// <summary>
    /// Application.dataPath : 실행파일/실행파일_Data/
    /// </summary>
    public override string GetDataLocation_DataPath()
    {
        return Application.dataPath + "/";
        //윈도우의 경우에는 파일 경로 뒤에 / 붙여줘야 ㄹㅇ 경로처럼 되어버리기 때문에...ㅇㅋ? 
    }


    /// <summary>
    /// 파일이 존재하는지 확인힙니다. 결과는 isExit_Result에 저장됩니다.
    /// </summary>
    /// <param name="dataName">파일의 이름. 예시 : testFile.png</param>
    public override IEnumerator IsExist(string dataName, string dataPath)
    {
        string path = dataPath + dataName;
        isExist_result = File.Exists(path);
        yield break;
    }

    /// <summary>
    /// 파일을 생성합니다.
    /// </summary>
    /// <param name="_path">파일, 파일 확장자를 포함한 전체 경로</param>
    public override void CreateFile(string _path)
    {
        File.Create(_path);
    }    
//#endif
}
