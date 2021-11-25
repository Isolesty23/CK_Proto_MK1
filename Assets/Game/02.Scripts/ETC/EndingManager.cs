using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    private UIMovieScreen movieScreen;
    private UIEndingCredit endingCredit;
    private IEnumerator Start()
    {
        //로딩창이 사라질때까지 대기
       yield return new WaitWhile(() => SceneChanger.Instance.isLoading);

        yield return null;
        //무비스크린 데려오기
        movieScreen = UIManager.Instance.GetUI("UIMovieScreen") as UIMovieScreen;
        //if (AudioManager.Instance.Audios.audioSource_BGM.isPlaying)
        //    AudioManager.Instance.Audios.audioSource_BGM.Stop();
        movieScreen.onMovieEnded += MovieScreen_onMovieEnded;
        StartCoroutine(movieScreen.playMovie);
    }

    private void MovieScreen_onMovieEnded()
    {
        movieScreen.onMovieEnded -= MovieScreen_onMovieEnded;
        movieScreen.waitingPanel.SetActive(false);
        //꺼버리기
        movieScreen.Close();

        endingCredit = UIManager.Instance.GetUI("UIEndingCredit") as UIEndingCredit;
        endingCredit.StartCoroutine(endingCredit.Up());

        endingCredit.onCreditMoveEnded += EndingCredit_onCreditMoveEnded;
    }


    private IEnumerator OnCreditMoveEnded()
    {
        yield return new WaitForSeconds(1f);
        SceneChanger.Instance.LoadThisScene(SceneNames.mainMenu);
    }
    private void EndingCredit_onCreditMoveEnded()
    {
        endingCredit.onCreditMoveEnded -= EndingCredit_onCreditMoveEnded;
        StartCoroutine(OnCreditMoveEnded());

    }
}
