using System.Collections;
using UnityEngine;

using UnityEngine.UI;

public class CallBreak_LoadingScreenAnimation : MonoBehaviour
{
    [SerializeField] Text loadingText;
    int repeatRate;
    [SerializeField] string connectingText;
    Coroutine loadingAnim;
    private void OnEnable() => StartLoadingAnim();
    private void OnDisable() => StopLoadingAnimation();

    private void StartLoadingAnim()
    {
        if (loadingAnim != null)
            StopCoroutine(loadingAnim);
        loadingAnim = StartCoroutine(AnimationOnText());

    }
    void StopLoadingAnimation()
    {
        if (loadingAnim != null)
            StopCoroutine(loadingAnim);
        loadingAnim = null;
    }
    IEnumerator AnimationOnText()
    {
    testAgain:
        yield return new WaitForSeconds(0.4f);

        repeatRate++;
        string dotText = loadingText.text;
        loadingText.text = dotText + ".";
        if (repeatRate == 4)
        {
            repeatRate = 0;
            loadingText.text = connectingText;
        }
        if (loadingAnim != null)
            goto testAgain;
    }
}