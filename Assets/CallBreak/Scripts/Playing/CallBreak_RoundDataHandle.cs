using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using ThreePlusGamesCallBreak;

public class CallBreak_RoundDataHandle : MonoBehaviour
{
    #region Variables
    public static CallBreak_RoundDataHandle Inst;
    [Tooltip("OpenToolTipList contain the currently open tool tip in scoreboard current open round")]
    List<GameObject> OpenToolTipList = new List<GameObject>();
    private static GameObject descendant = null;
    public GameObject[] Players;
    public GameObject[] TotalBagHighlight, TotalPointsHighlight;
    #endregion

    #region Unity callbacks
    void Start()
    {
        Inst = this;
    }
    #endregion


    #region round header title click method 
    public void DisplayToolTip(string BtnName)
    {
        GameObject RoundHeader = this.gameObject.transform.Find("RoundHeader").gameObject;
        Debug.Log("Display Tool Tip");

        if (OpenToolTipList.Count != 0)
        {
            OpenToolTipList[0].transform.DOScale(0, 0);
            OpenToolTipList.Clear();
        }
        //Debug.Log(ReturnDecendantOfParent(RoundHeader, BtnName));
        ToolTipEnable(ReturnDecendantOfParent(RoundHeader, BtnName).transform.GetChild(0).gameObject);

    }
    #endregion

    #region enable specific tooltip on round header title click
    void ToolTipEnable(GameObject ToolTip)
    {
        OpenToolTipList.Add(ToolTip);
        ToolTip.transform.DOScale(1, 0.1f);
    }
    #endregion

    #region hide other open tooltip when click on another tooltip
    public void HideAllToolTip()
    {
        for (int i = 0; i < OpenToolTipList.Count; i++)
        {
            OpenToolTipList[i].transform.DOScale(0, 0);
        }
    }
    #endregion

    #region method for get child object, pass parent object and name of object you want to find in child's object
    public static GameObject ReturnDecendantOfParent(GameObject parent, string descendantName)
    {

        foreach (Transform child in parent.transform)
        {
            if (child.name == descendantName)
            {
                descendant = child.gameObject;
                break;
            }
            else
            {
                ReturnDecendantOfParent(child.gameObject, descendantName);
            }
        }
        return descendant;
    }
    #endregion


    #region FTUE HighLight Column
    
    internal void TotalPointsHighlightFTUE()
    {
        for (int i = 0; i < TotalBagHighlight.Length; i++)
        {
            TotalBagHighlight[i].gameObject.SetActive(false);
        }
        for (int i = 0; i < TotalPointsHighlight.Length; i++)
        {
            TotalPointsHighlight[i].gameObject.SetActive(true);
        }
    }
    #endregion
}
