using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using UnityEngine.UI;
using CallBreak_Socketmanager;
using System;

namespace ThreePlusGamesCallBreak
{
    public class CallBreak_MyCard : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IPointerClickHandler
    {
        #region variables
        public static CallBreak_MyCard Inst;
        [Tooltip("cardName variable contain the name of card")]
        public string cardName;
        [Tooltip("cardValue variable contain card number in numeric formate")]
        public int cardValue;
        [Tooltip("type variable contain suit latter of card")]
        public string type;
        public int number;
        internal bool MyCardThrowd = false;
        internal bool isCardThrowed = false;
        [Tooltip("for use in check single and double tap")]
        float clicked = 0;
        float clicktime = 0;
        float clickdelay = 0.5f;
        float oldLocalYPosition = 0;
        //public Canvas myCardCanvas;
        #endregion

        #region Unity callbacks
        private void Awake() => Inst = this;
        #endregion

        #region This is A card Throw handler
        public void OnPointerDown(PointerEventData eventData)
        {
        }

        // when user click  on single card two times card will be throwd than this method will work
        // first click card move to +20 in y and after some time card Automatically Set to Its Y position by invoke ResetCardPosition
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!isMovePossible())
                return;

            for (int i = 0; i < CallBreak_UIManager.Inst.MyCardsParent.transform.childCount; i++)
            {
                if (CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(i).name != transform.name)
                    CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(i).GetComponent<RectTransform>().DOAnchorPosY(0, 0);
            }

            Debug.Log(clicked + " Clicked Card Name " + this.transform.name);
            clicked++;
            if (clicked == 1)
            {
                clicktime = Time.time;
                this.isCardThrowed = true;
                transform.DOLocalMoveY(oldLocalYPosition + 20, 0.1f);
            }
            CancelInvoke(nameof(ResetSingleCardPosition));
            Invoke(nameof(ResetSingleCardPosition), clickdelay);
            if (clicked > 1 && Time.time - clicktime < clickdelay)
            {
                Debug.Log(" Clicked count " + clicked + "Time " + (Time.time - clicktime) + "delay " + clickdelay);
                clicked = 0;
                clicktime = 0;
                Debug.Log("Check Disable User Cards Input::" + MyCardThrowd);
                Debug.Log("Check Is my turn true or false" + CallBreak_GS.Inst.isMyTurn);
                Debug.Log(" Self Player Card Throwed now ");

                for (int i = 0; i < transform.parent.childCount; i++)
                    transform.parent.GetChild(i).transform.DOLocalMoveY(oldLocalYPosition, 0f);

                CallBreak_GS.Inst.DisableAllMyCardTrigger();
                CancelInvoke(nameof(ResetSingleCardPosition));
                this.isCardThrowed = false;

                CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.ThrowCard(this.transform.GetComponent<Image>().sprite.name), ThrowCardAcknowledgement, CallBreak_CustomEvents.USER_THROW_CARD);

            }
        }

        //if card drag and drop portion start here 
        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!isMovePossible())
                return;
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (!isMovePossible())
                return;
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
            if (GetDragDirection(dragVectorDirection) == DraggedDirection.Up)
            {
                CallBreak_GS.Inst.DisableAllMyCardTrigger();
                CancelInvoke(nameof(ResetSingleCardPosition));
                this.isCardThrowed = false;
                CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.ThrowCard(this.transform.GetComponent<Image>().sprite.name), ThrowCardAcknowledgement, CallBreak_CustomEvents.USER_THROW_CARD);
            }
        }
        //public void OnEndDrag(PointerEventData eventData)
        //{
        //    if (!isMovePossible())
        //        return;
        //    Destroy(myCardCanvas);
        //    myCardCanvas = null;
        //    Vector3 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
        //    if (oldLocalYPosition + 80 < transform.localPosition.y)
        //    {
        //        if (GetDragDirection(dragVectorDirection) == DraggedDirection.Up && CallBreak_GS.Inst.isMyTurn && !MyCardThrowd)
        //        {
        //            try
        //            {
        //                DestroyImmediate(CallBreak_FTUEHandler.Inst.HandAnimObject);
        //            }
        //            catch { }
        //            //CallBreak_UIManager.Inst.myCardsParentHorizontalLayoutGroup.enabled = true;
        //            CallBreak_GS.Inst.DisableAllMyCardTrigger();
        //            CallBreak_GS.Inst.isMyCardThrowResDone = false;
        //            CallBreak_SocketConnection.intance.SendData(CallBreak_SocketEventManager.ThrowCard(this.transform.GetComponent<Image>().sprite.name), ThrowCardAcknowledgement, CallBreak_CustomEvents.USER_THROW_CARD);
        //        }
        //    }
        //    else
        //        CallBreak_UIManager.Inst.myCardsParentHorizontalLayoutGroup.enabled = true;
        //}

        // Set card Y position to old position
        // reset CLick count
        // reset click time
        private void ResetSingleCardPosition()
        {
            if (isCardThrowed)
            {
                Debug.Log(" Reset Card position Called ");
                if (transform.parent.name == CallBreak_UIManager.Inst.MyCardsParent.name)
                {

                    Debug.Log(" Reset Card position Called ");
                    transform.DOLocalMoveY(oldLocalYPosition, 0.1f).OnComplete(() =>
                  {
                      clicked = 0;
                      clicktime = 0;
                  });
                }
                else
                {
                    Debug.Log(" Change Card parent no need to reset position " + transform.parent.name);
                }
                isCardThrowed = false;
            }
            else
            {
                clicked = 0;
                clicktime = 0;
            }
        }



        // this is a bool for check condition is Move Possible or not portion

        bool isMovePossible()
        {
            if (!CallBreak_GS.Inst.isMyTurn)
                return false;
            if (this.transform.GetComponent<Image>().color != CallBreak_UIManager.Inst.EnableCardColor)
                return false;
            if (MyCardThrowd)
                return false;

            return true;
        }
        #endregion

        #region to find in which direction user dragging finger
        private DraggedDirection GetDragDirection(Vector3 dragVector)
        {
            float positiveX = Mathf.Abs(dragVector.x);
            float positiveY = Mathf.Abs(dragVector.y);

            DraggedDirection draggedDir;

            if (positiveX > positiveY)
                draggedDir = (dragVector.x > 0) ? DraggedDirection.Right : DraggedDirection.Left;
            else
                draggedDir = (dragVector.y > 0.98 && dragVector.y != 1) ? DraggedDirection.Up : DraggedDirection.Down;

            return draggedDir;
        }
        #endregion

        private enum DraggedDirection
        {
            Up,
            Down,
            Right,
            Left
        }

        #region ACKNOWLEDGEMENT_CALLBACKS
        internal void ThrowCardAcknowledgement(string ackData)
        {
            Debug.Log("CallBreak_MyCard->ThrowCard Acknowledged || ackData : " + ackData);
        }
        #endregion
    }
}
