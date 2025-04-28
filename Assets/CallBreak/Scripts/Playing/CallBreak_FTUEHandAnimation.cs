using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ThreePlusGamesCallBreak;

public class CallBreak_FTUEHandAnimation : MonoBehaviour
{
    #region Variables
    public static CallBreak_FTUEHandAnimation Inst;
    [SerializeField] Image image_Hand;
    Sequence sequence;
    int count = 0;
    #endregion

    #region Unity Callbacks
    private void Awake()
    {
        Inst = this;
    }
    private void Start()
    {
        CardThrowAnimHand();
    }

    #endregion

    private void CardThrowAnimHand()
    {
        count++;
        sequence = DOTween.Sequence();

        transform.position = transform.parent.position;

        image_Hand.DOFade(0f, 0f);
        if (count > 2)
            transform.localScale = new Vector3(2f, 2f, 2f);
        if (count > 2)
            transform.DOScale(1f, .5f);
        if (count > 2)
            sequence.Append(image_Hand.DOFade(1f, .4f));
        sequence.Append(transform.DOLocalMove(new Vector3(transform.parent.position.x, transform.parent.localPosition.y + 15, transform.parent.position.z), .7f));
        sequence.Append(image_Hand.DOFade(0f, .4f)).OnComplete(() =>
        {
            if (transform.parent.gameObject.transform.parent.gameObject.name != "DiscardedCardGroup")
                CardThrowAnimHand();
        });
    }

}
