using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
namespace ThreePlusGamesCallBreak
{
    public class CallBreak_FTUECard : MonoBehaviour, IDragHandler
    {
        bool cardThrown;
        public void OnDrag(PointerEventData eventData)
        {
            Vector3 dragVectorDirection = (eventData.position - eventData.pressPosition).normalized;
            if (!cardThrown && GetDragDirection(dragVectorDirection) == DraggedDirection.Up)
            {
                Debug.LogError("Player1CardThrow");
                cardThrown = true;
                transform.GetChild(0).gameObject.SetActive(false);
                transform.GetChild(1).gameObject.SetActive(false);
                gameObject.transform.SetParent(CallBreak_CardDeal.Inst.PlayingUIBackground.transform);
                gameObject.AddComponent<Canvas>().overrideSorting = true;
                gameObject.GetComponent<Canvas>().sortingOrder = 2;
                gameObject.transform.SetParent(CallBreak_CardDeal.Inst.discardedCardContainer.transform);
                gameObject.transform.DORotate(new Vector3(0, 0, -90), 0f);
                gameObject.transform.DORotate(new Vector3(0, 0, 0), 0.4f);
                gameObject.GetComponent<RectTransform>().DOSizeDelta(CallBreak_UIManager.Inst.DiscardPositions[0].GetComponent<RectTransform>().sizeDelta, 0.2f);
                CallBreak_FTUEManager.instance.tempDisCardedCardsList.Add(gameObject);
                gameObject.transform.DOLocalMove(CallBreak_UIManager.Inst.DiscardPositions[0].transform.localPosition, 0.2f).OnComplete(() => { CallBreak_FTUEManager.instance.ClosePopupAndThrowAnotherCard(); });

            }
        }
        #region to find in which direction user dragging finger
        private DraggedDirection GetDragDirection(Vector3 dragVector)
        {
            float positiveX = Mathf.Abs(dragVector.x);
            float positiveY = Mathf.Abs(dragVector.y);
            DraggedDirection draggedDir;
            //Debug.Log(dragVector.y);
            if (positiveX > positiveY)
                draggedDir = (dragVector.x > 0) ? DraggedDirection.Right : DraggedDirection.Left;
            else
                draggedDir = (dragVector.y > 0.98 && dragVector.y != 1) ? DraggedDirection.Up : DraggedDirection.Down;
            return draggedDir;
        }
        #endregion
        private enum DraggedDirection
        {
            Up, Down, Right, Left
        }
    }
}