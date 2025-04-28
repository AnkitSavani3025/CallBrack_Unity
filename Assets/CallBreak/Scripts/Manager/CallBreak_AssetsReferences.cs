using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallBreak_AssetsReferences : MonoBehaviour
{
    public static CallBreak_AssetsReferences Inst;
    [Tooltip("all 52 cards, we will use at runtime")]
    public List<Sprite> AllCardSprite = new List<Sprite>();
    [Tooltip("use for assign button sprite in scoreboard button at runtime")]
    public Sprite btn_Common_disable, btn_Common_enable;

    private void Awake()
    {
        Inst = this;
    }
}
