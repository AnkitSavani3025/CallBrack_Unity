using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallBreak_ButtonClickAnimation : MonoBehaviour
{
    public void ButtonClickAnim(GameObject obj)
    {
        obj.transform.DOScale(0.8f, 0.15f).OnComplete(() =>
        {
            obj.transform.DOScale(1f, 0.15f);
        });
    }
}
