using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using CallBreak_Socketmanager;
using System.Threading.Tasks;
using System.Linq;

namespace ThreePlusGamesCallBreak
{
    public class CallBreak_CardDeal : MonoBehaviour
    {
        #region variables
        public static CallBreak_CardDeal Inst;
        internal Transform MainUIBGObj;
        internal List<GameObject> DisCardedCardsList = new List<GameObject>();

        [SerializeField]
        internal CallBreak_Users[] PlayerList;
        internal bool CardDealAnimOnGoing = false, cardCollectAnimation = false;

        public GameObject PlayingUIBackground, Player1, TableMain, discardedCardContainer;

        [Tooltip("to stop card deal animation IEnumerator")]

        internal IEnumerator DealAnimEnumeator;

        [SerializeField]
        List<GameObject> tempGeneratedCardList = new List<GameObject>();
        [Tooltip("Prefabs")]
        public GameObject BootCollect, CardPREFAB, CardP1, CardRejoinP1, CardRejoinP2, CardRejoinP3, CardRejoinP4;
        [Tooltip("Sprite")]
        public Sprite Player2_1, Player2_2, Player2_3;
        #endregion

        #region Unity callbacks
        private void Awake() =>
            Inst = this;

        private void OnEnable() => CallBreak_EventManager.ResetAll += ResetAllAfterNewGame;

        private void OnDisable() => CallBreak_EventManager.ResetAll -= ResetAllAfterNewGame;
        #endregion


        #region Method for Card deal animation

        internal IEnumerator StartCardDeal(int si)
        {
            CardDealAnimOnGoing = true;
            tempGeneratedCardList.Clear();

            yield return new WaitForSeconds(0.05f);

            Debug.Log(" StartCardDeal || START card Deal Animation ");
            CallBreak_UIManager.Inst.MyCardsParent.GetComponent<HorizontalLayoutGroup>().enabled = false;
            int childCount = CallBreak_UIManager.Inst.MyCardsParent.transform.childCount;
            for (int n = 0; n < childCount; n++)
            {
                DestroyImmediate(CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(0).gameObject);
            }
            CallBreak_UIManager.Inst.MyCardsParent.transform.DOScaleX(1, 0.1f);
            yield return new WaitForSeconds(0.05f);

            for (int i = 0; i < 13; i++)
            {
                GameObject cloneCard = Instantiate(CallBreak_UIManager.Inst.CardMain, CallBreak_UIManager.Inst.CardGeneratePoint.transform);
                cloneCard.transform.DOScaleX(1, 0);
                cloneCard.transform.SetParent(CallBreak_UIManager.Inst.MyCardsParent.transform);
                cloneCard.SetActive(false);
                tempGeneratedCardList.Add(cloneCard);
            }
            CallBreak_SocketEventReceiver.Inst.SetUserHandCards(); // Set All Dummy Card Data whoes Send from Backend           

            yield return new WaitForSeconds(0.5f);

            Debug.Log(" Card Animation start ----" + CallBreak_UIManager.Inst.MyCardsParent.transform.childCount + "  TEMP card List Count " + tempGeneratedCardList.Count);

            for (int i = 0; i < tempGeneratedCardList.Count; i++)
            {
                RectTransform cardRectTransform = tempGeneratedCardList[i].GetComponent<RectTransform>();
                cardRectTransform.gameObject.SetActive(true);
                cardRectTransform.DOAnchorPos(CallBreak_UIManager.Inst.FaekCardHolder.transform.GetChild(i).GetComponent<RectTransform>().localPosition, 3, true).SetEase(Ease.OutElastic, 1, 1.3f).OnComplete(() =>
                {
                    if (cardRectTransform.name == tempGeneratedCardList[i - 1].name)
                        CallBreak_UIManager.Inst.MyCardsParent.GetComponent<HorizontalLayoutGroup>().enabled = true;
                });
                CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.CardDealing);
                yield return new WaitForSeconds(0.1f);
            }
            CardDealAnimOnGoing = false;
            yield return new WaitForSeconds(0.25f);

        }
        #endregion

        public float cardThrowSpeed = 0.3f;
        #region Player 1 card thow animation method on rejoin
        internal void Player1CardThrowRejoin(string CardName)
        {
            Debug.Log("<color=red>Player1CardThrowRejoin </color>");
            GameObject Card = Instantiate(CardRejoinP1, discardedCardContainer.transform);
            Card.name = "Player1";
            Card.AddComponent<Canvas>().overrideSorting = true;
            Card.transform.GetComponent<Canvas>().sortingOrder = CallBreak_SocketEventReceiver.Inst.CardOrder;
            Debug.Log("<color=red> Self Player Player1 Card Throw Rejoin|| CallBreak_SocketEventReceiver.Inst.CardOrder </color>" + CallBreak_SocketEventReceiver.Inst.CardOrder);
            Card.transform.localPosition = CallBreak_UIManager.Inst.DiscardPositions[0].transform.localPosition;
            Card.transform.DORotate(new Vector3(0, 0, 0), 0.1f);
            List<Sprite> temp = new List<Sprite>();
            temp = CallBreak_AssetsReferences.Inst.AllCardSprite.FindAll(x => x.name == CardName);
            Card.GetComponent<Image>().sprite = temp[0];
            DisCardedCardsList.Add(Card);
        }
        #endregion
        #region Player 1 card thow animation method
        internal void Player1CardThrow(GameObject Card)
        {
            Debug.Log("<color=red> Self Player Player1 Card Throw </color>" + Card.name);
            CallBreak_GS.Inst.isMyTurn = false;

            if (Card.GetComponent<Canvas>() == null)
                Card.AddComponent<Canvas>();

            Card.GetComponent<Canvas>().overrideSorting = true;
            Card.GetComponent<Canvas>().sortingOrder = CallBreak_SocketEventReceiver.Inst.CardOrder;
            Debug.Log("<color=red> Self Player Player1 Card Throw || CallBreak_SocketEventReceiver.Inst.CardOrder </color>" + CallBreak_SocketEventReceiver.Inst.CardOrder);
            Card.name = "Player1";

            CallBreak_UIManager.Inst.MyCardsParent.GetComponent<HorizontalLayoutGroup>().enabled = false;
            Card.transform.DOKill();
            Card.GetComponent<RectTransform>().DOSizeDelta(CallBreak_UIManager.Inst.DiscardPositions[0].GetComponent<RectTransform>().sizeDelta, cardThrowSpeed);
            Debug.Log("<color> Card  Moveble Place To Player 1 </color>" + CallBreak_UIManager.Inst.DiscardPositions[0].transform.position);


            this.DisCardedCardsList.Add(Card);
            Card.transform.DOMove(CallBreak_UIManager.Inst.DiscardPositions[0].transform.position, cardThrowSpeed).OnComplete((TweenCallback)(() =>
            {
                CallBreak_UIManager.Inst.MyCardsParent.GetComponent<HorizontalLayoutGroup>().enabled = true;
                Card.transform.DOKill();
                Card.transform.SetParent(discardedCardContainer.transform);

            }));


            CallBreak_PlayerController.Inst._myCardList.Remove(Card.GetComponent<CallBreak_MyCard>());

            CallBreak_Users p = CallBreak_GameManager.Inst.GetPlayerSeatIndex(CallBreak_GS.Inst.userinfo.Player_Seat);

            p.StopTurnTimer();

            for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
            {
                CallBreak_PlayerController.Inst.playerList[i].TimerBG.transform.localScale = Vector3.zero;
                CallBreak_PlayerController.Inst.playerList[i].darkImageForBetterView.enabled = false;
            }
            Destroy(Card.GetComponent<CallBreak_UIManager>());
        }
        #endregion

        #region Player 2 card thow animation method
        internal void Player2CardThrow(string CardName)
        {
            Debug.Log("<color=red> Player2 CardThrow </color>" + CardName);

            for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
            {
                CallBreak_PlayerController.Inst.playerList[i].TimerBG.transform.localScale = Vector3.zero;
                CallBreak_PlayerController.Inst.playerList[i].darkImageForBetterView.enabled = false;

            }
            GameObject CardPrefab = CardPREFAB;
            GameObject Card = Instantiate(CardPrefab, discardedCardContainer.transform);
            Card.name = "Player2";
            if (Card.GetComponent<Canvas>() == null)
                Card.AddComponent<Canvas>();
            Card.GetComponent<Canvas>().overrideSorting = true;
            Card.transform.GetComponent<Canvas>().sortingOrder = CallBreak_SocketEventReceiver.Inst.CardOrder;
            Debug.Log("<color=red> Self Player Player2 Card Throw || CallBreak_SocketEventReceiver.Inst.CardOrder </color>" + CallBreak_SocketEventReceiver.Inst.CardOrder);

            List<Sprite> temp = new List<Sprite>();
            temp = CallBreak_AssetsReferences.Inst.AllCardSprite.FindAll(x => x.name == CardName);
            Card.GetComponent<Image>().sprite = temp[0];
            Vector3 GenPos = CallBreak_UIManager.Inst.P2DisGenPos.transform.localPosition;
            Card.transform.localPosition = new Vector3(GenPos.x + 100, GenPos.y, GenPos.z);
            Card.transform.DORotate(new Vector3(0, 0, -30), 0f);
            Card.transform.DORotate(new Vector3(0, 0, 90), cardThrowSpeed);
            Card.GetComponent<RectTransform>().DOSizeDelta(CallBreak_UIManager.Inst.DiscardPositions[1].GetComponent<RectTransform>().sizeDelta, cardThrowSpeed);
            this.DisCardedCardsList.Add(Card);
            Card.transform.DOLocalMove(CallBreak_UIManager.Inst.DiscardPositions[1].transform.localPosition, cardThrowSpeed).OnComplete((TweenCallback)(() =>
            {
            }));
        }
        #endregion

        #region Player 2 card thow animation method
        internal void Player2RejoinCardThrow(string CardName)
        {
            
            Debug.Log("===" + CardName);
            GameObject Card = Instantiate(CardRejoinP2, discardedCardContainer.transform);
            Card.name = "Player2";
            Card.AddComponent<Canvas>();
            Card.GetComponent<Canvas>().overrideSorting = true;
            Card.transform.GetComponent<Canvas>().sortingOrder = CallBreak_SocketEventReceiver.Inst.CardOrder;
            Debug.Log("<color=red> Self Player Player2 Card Throw Rejoin|| CallBreak_SocketEventReceiver.Inst.CardOrder </color>" + CallBreak_SocketEventReceiver.Inst.CardOrder);

            Card.transform.localPosition = CallBreak_UIManager.Inst.DiscardPositions[1].transform.localPosition;
            Card.transform.DORotate(new Vector3(0, 0, -30), 0f);
            Card.transform.DORotate(new Vector3(0, 0, 90), 0.1f);
            List<Sprite> temp = new List<Sprite>();
            temp = CallBreak_AssetsReferences.Inst.AllCardSprite.FindAll(x => x.name == CardName);
            Card.GetComponent<Image>().sprite = temp[0];
            DisCardedCardsList.Add(Card);

        }
        #endregion


        #region Player 3 card thow animation method
        internal void Player3CardThrow(string CardName)
        {
            for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
            {
                CallBreak_PlayerController.Inst.playerList[i].TimerBG.transform.localScale = Vector3.zero;
                CallBreak_PlayerController.Inst.playerList[i].darkImageForBetterView.enabled = false;

            }
            GameObject CardPrefab = CardPREFAB;
            GameObject Card = Instantiate(CardPrefab, discardedCardContainer.transform);
            Card.name = "Player3";
            Card.AddComponent<Canvas>().overrideSorting = true;
            Card.transform.GetComponent<Canvas>().sortingOrder = CallBreak_SocketEventReceiver.Inst.CardOrder;
            Debug.Log("<color=red> Self Player Player3 Card Throw|| CallBreak_SocketEventReceiver.Inst.CardOrder </color>" + CallBreak_SocketEventReceiver.Inst.CardOrder);

            List<Sprite> temp = new List<Sprite>();
            temp = CallBreak_AssetsReferences.Inst.AllCardSprite.FindAll(x => x.name == CardName);
            Card.GetComponent<Image>().sprite = temp[0];
            Vector3 GenPos = CallBreak_UIManager.Inst.P3DisGenPos.transform.localPosition;
            Card.transform.localPosition = GenPos;
            Card.transform.DORotate(new Vector3(0, 0, -90), 0f);
            Card.transform.DORotate(new Vector3(0, 0, 0), cardThrowSpeed);
            Card.GetComponent<RectTransform>().DOSizeDelta(CallBreak_UIManager.Inst.DiscardPositions[2].GetComponent<RectTransform>().sizeDelta, cardThrowSpeed);
            this.DisCardedCardsList.Add(Card);
            Card.transform.DOLocalMove(CallBreak_UIManager.Inst.DiscardPositions[2].transform.localPosition, cardThrowSpeed).OnComplete((TweenCallback)(() =>
            {
            }));
        }
        #endregion

        #region Player 3 card thow animation method
        internal void Player3RejoinCardThrow(string CardName)
        {
            GameObject Card = Instantiate(CardRejoinP3, discardedCardContainer.transform);
            Card.name = "Player3";
            Card.AddComponent<Canvas>().overrideSorting = true;
            Card.transform.GetComponent<Canvas>().sortingOrder = CallBreak_SocketEventReceiver.Inst.CardOrder;
            Debug.Log("<color=red> Self Player Player3 Card Throw rejoin|| CallBreak_SocketEventReceiver.Inst.CardOrder </color>" + CallBreak_SocketEventReceiver.Inst.CardOrder);

            Card.transform.localPosition = CallBreak_UIManager.Inst.DiscardPositions[2].transform.localPosition;
            Card.transform.DORotate(new Vector3(0, 0, -90), 0f);
            Card.transform.DORotate(new Vector3(0, 0, 0), 0.1f);
            List<Sprite> temp = new List<Sprite>();
            temp = CallBreak_AssetsReferences.Inst.AllCardSprite.FindAll(x => x.name == CardName);
            Card.GetComponent<Image>().sprite = temp[0];
            DisCardedCardsList.Add(Card);
        }
        #endregion

        #region Player 4 card thow animation method
        internal void Player4CardThrow(string CardName)
        {
            for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
            {
                CallBreak_PlayerController.Inst.playerList[i].TimerBG.transform.localScale = Vector3.zero;
                CallBreak_PlayerController.Inst.playerList[i].darkImageForBetterView.enabled = false;
            }
            GameObject CardPrefab = CardPREFAB;
            GameObject Card = Instantiate(CardPrefab, discardedCardContainer.transform);
            Card.name = "Player4";
            Card.AddComponent<Canvas>().overrideSorting = true;
            Card.transform.GetComponent<Canvas>().sortingOrder = CallBreak_SocketEventReceiver.Inst.CardOrder;
            Debug.Log("<color=red> Self Player Player4 Card Throw|| CallBreak_SocketEventReceiver.Inst.CardOrder </color>" + CallBreak_SocketEventReceiver.Inst.CardOrder);

            List<Sprite> temp = new List<Sprite>();
            temp = CallBreak_AssetsReferences.Inst.AllCardSprite.FindAll(x => x.name == CardName);
            Card.GetComponent<Image>().sprite = temp[0];
            Vector3 GenPos = CallBreak_UIManager.Inst.P4DisGenPos.transform.localPosition;
            Card.transform.localPosition = GenPos;
            Card.transform.DORotate(new Vector3(0, 0, -60), 0f);
            Card.transform.DORotate(new Vector3(0, 0, -90), cardThrowSpeed);
            Card.GetComponent<RectTransform>().DOSizeDelta(CallBreak_UIManager.Inst.DiscardPositions[3].GetComponent<RectTransform>().sizeDelta, cardThrowSpeed);
            this.DisCardedCardsList.Add(Card);
            Card.transform.DOLocalMove(CallBreak_UIManager.Inst.DiscardPositions[3].transform.localPosition, cardThrowSpeed).OnComplete((TweenCallback)(() =>
            {
            }));
        }
        #endregion
        #region Player 4 card thow animation method
        internal void Player4RejoinCardThrow(string CardName)
        {
            GameObject Card = Instantiate(CardRejoinP4, discardedCardContainer.transform);
            Card.name = "Player4";
            Card.AddComponent<Canvas>().overrideSorting = true;
            Card.transform.GetComponent<Canvas>().sortingOrder = CallBreak_SocketEventReceiver.Inst.CardOrder;
            Debug.Log("<color=red> Self Player Player4 Card Throw Rejoin|| CallBreak_SocketEventReceiver.Inst.CardOrder </color>" + CallBreak_SocketEventReceiver.Inst.CardOrder);

            Card.transform.localPosition = CallBreak_UIManager.Inst.DiscardPositions[3].transform.localPosition;
            Card.transform.DORotate(new Vector3(0, 0, -60), 0f);
            Card.transform.DORotate(new Vector3(0, 0, -90), 0.1f);
            List<Sprite> temp = new List<Sprite>();
            temp = CallBreak_AssetsReferences.Inst.AllCardSprite.FindAll(x => x.name == CardName);
            Card.GetComponent<Image>().sprite = temp[0];
            DisCardedCardsList.Add(Card);
        }
        #endregion

        #region Call Card throw method
        internal void UserCardThrow(string cardName, int seatIndex, bool isRejoinThrow)
        {
            discardedCardContainer.transform.DOScaleX(1, 0f);

            if (isRejoinThrow)    //check if its rejoin card throw then not play card throw animation and direct display on table
            {
                CallBreak_Users p = CallBreak_GameManager.Inst.GetPlayerSeatIndex(seatIndex);
                if (p.gameObject.name == "Player2")
                {
                    Player2RejoinCardThrow(cardName);
                }
                else if (p.gameObject.name == "Player3")
                {
                    Player3RejoinCardThrow(cardName);
                }
                else if (p.gameObject.name == "Player4")
                {
                    Player4RejoinCardThrow(cardName);
                }
            }
            else
            {
                CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.HandCollect);
                CallBreak_Users p = CallBreak_GameManager.Inst.GetPlayerSeatIndex(seatIndex);
                if (p.gameObject.name == "Player1")
                {
                    try
                    {
                        GameObject card = CallBreak_UIManager.Inst.MyCardsParent.transform.Find(cardName).gameObject;
                        if (card != null)
                            Player1CardThrow(card);
                        else
                            Debug.Log(" Player 1 Card Thrwod Before Animation ");
                    }
                    catch (System.Exception e) { Debug.LogError("Lelf Player:" + e.ToString()); }
                }
                else if (p.gameObject.name == "Player2")
                {
                    Player2CardThrow(cardName);
                }
                else if (p.gameObject.name == "Player3")
                {
                    Player3CardThrow(cardName);
                }
                else if (p.gameObject.name == "Player4")
                {
                    Player4CardThrow(cardName);
                }
            }
        }
        #endregion

        internal List<GameObject> DisCardedCardsListNEW = new List<GameObject>();

        #region Hand collect animation method
        internal async void HandCollectAnim(GameObject TargetPlayer, int seatIndex)
        {
            cardCollectAnimation = true;
            DisCardedCardsListNEW.Clear();
            GameObject cardWon = DisCardedCardsList.Find(x => x.name == TargetPlayer.name);
            DisCardedCardsListNEW = DisCardedCardsList.ToList();
            DisCardedCardsList.Clear();

            Debug.Log(Time.time + " Target Player::" + TargetPlayer.name);
            Debug.Log(Time.time + " DisCardedCardsListNEW Count:: " + DisCardedCardsListNEW.Count);//+ "Temp Discard count " + tempDisCardedCardsList.Count);
            Debug.Log(Time.time + " Card Won Name From  tempDisCardedCardsList ::" + cardWon.name);

            if (cardWon.GetComponent<Canvas>() == null)
            {
                cardWon.AddComponent<Canvas>().overrideSorting = true;
            }
            cardWon.GetComponent<Canvas>().sortingOrder = 3;

            cardWon.transform.DOPunchScale(new Vector2(0.8f, 0.3f), 0.2f, 0, 1).SetEase(Ease.InBounce);
            await Task.Delay(400);
            CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.HandCollect);
            Debug.Log(Time.time + " After Punch Scale DisCardedCardsList Count:: " + DisCardedCardsListNEW.Count);// + "Temp Discard count " + tempDisCardedCardsList.Count);

            try
            {
                for (int i = 0; i < DisCardedCardsListNEW.Count; i++)
                {
                    if (DisCardedCardsListNEW[i].gameObject.name != cardWon.name)
                    {
                        try
                        {
                            if (DisCardedCardsListNEW[i].GetComponent<Canvas>() == null)
                            {
                                DisCardedCardsListNEW[i].AddComponent<Canvas>().overrideSorting = true;
                            }
                            DisCardedCardsListNEW[i].GetComponent<Canvas>().sortingOrder = 1;
                        }
                        catch (System.Exception e) { Debug.LogError(e.ToString()); }
                        Debug.Log(" Discarded Card Without Card Won Name " + DisCardedCardsListNEW[i].name);
                        DisCardedCardsListNEW[i].transform.DORotate(cardWon.transform.eulerAngles, 0.15f);
                        DisCardedCardsListNEW[i].transform.DOLocalMove(cardWon.transform.position, 0.15f);
                        DisCardedCardsListNEW[i].transform.SetParent(cardWon.transform);
                    }                   
                    Debug.Log(" Discarded Card With Card Won Name " + DisCardedCardsListNEW[i].name);

                }
                DisCardedCardsListNEW.Clear();
            }
            catch (System.Exception e) { Debug.LogError(e.ToString()); }
            await Task.Delay(300);
            cardWon.GetComponent<Canvas>().sortingOrder = 1;

            MoveCardToWinner(seatIndex, cardWon);
        }

        void MoveCardToWinner(int seatIndex, GameObject cardWon)
        {
            Debug.Log("Local Position seat Index : " + seatIndex + CallBreak_GameManager.Inst.GetPlayerSeatIndex(seatIndex).transform.localPosition);
            cardWon.transform.DOLocalMove(CallBreak_GameManager.Inst.GetPlayerSeatIndex(seatIndex).transform.localPosition, 0.25f).OnComplete(() =>
            {
                Debug.Log(" Called From Move Card To Winner  ");
                Destroy(cardWon);
                cardCollectAnimation = false;
                if (CallBreak_UIManager.Inst.MyCardsParent.transform.childCount != 1)
                {
                    CallBreak_GS.Inst.Invoke(nameof(CallBreak_GS.Inst.EnableAllMyCardTrigger), 0.7f);
                    Debug.Log(" This is my turn  EnableAllMyCardTrigger ");
                }
            });
        }
        internal void DisCardedCardDestroyImmediate()
        {
            int childcount = discardedCardContainer.transform.childCount;
            for (int n = 0; n < childcount; n++)
            {
                Debug.Log(" DisCarded Cards  Destroy Immediate " + discardedCardContainer.transform.GetChild(0).name);
                DestroyImmediate(discardedCardContainer.transform.GetChild(0).gameObject);
            }
            DisCardedCardsListNEW.Clear();
            DisCardedCardsList.Clear();
            cardCollectAnimation = false;

            if (CallBreak_UIManager.Inst.MyCardsParent.transform.childCount != 1)
            {
                CallBreak_GS.Inst.Invoke(nameof(CallBreak_GS.Inst.EnableAllMyCardTrigger), 0.7f);
                Debug.Log(" This is my turn  Enable All My Card Trigger ");
            }
        }
        #endregion





        #region Boot value collect animation
        List<GameObject> BootValueObjList = new List<GameObject>();
        internal void BootCollectAnimation()
        {
            float bootValue = CallBreak_GS.Inst.bootData.bootValue;
            CallBreak_GS.Inst.collectBootAnimation = true;
            if (bootValue != 0)
            {
                BootValueObjList.Clear();

                for (int i = 0; i < 4; i++)
                {
                    if (PlayerList[i].transform.localScale != Vector3.zero)
                    {
                        GameObject BootCollectObj = Instantiate(BootCollect, PlayerList[i].transform);
                        BootCollectObj.transform.SetParent(PlayingUIBackground.transform);
                        BootValueObjList.Add(BootCollectObj);
                        BootCollectObj.transform.DOPunchScale(new Vector3(1.1f, 1.1f, 1.1f), 0.2f, 5, 1f);
                    }
                }
                DoMoveUpBootValue();
            }
            else
            {
                Debug.Log(" CallBreak_CardDeal ||  BootCollectAnimation || Animation Now Show Boot Value" + bootValue);
            }
        }


        void DoMoveUpBootValue()
        {
            for (int i = 0; i < BootValueObjList.Count; i++)
            {
                Transform BootValueObject = BootValueObjList[i].transform;

                BootValueObject.DOLocalMove(Vector2.zero, 0.3f).SetDelay(0.3f).OnComplete(() =>
                {
                    float x = CallBreak_UIManager.Inst.PotAmount.transform.position.x - BootValueObject.transform.position.x;
                    float y = CallBreak_UIManager.Inst.PotAmount.transform.position.y - BootValueObject.transform.position.y;
                    BootValueObject.transform.DOBlendableMoveBy(new Vector3(-x * 5f, y * 1.5f, 0), 0.5f).SetDelay(0.25f);

                    BootValueObject.transform.DOBlendableMoveBy(new Vector3(x * 6f, -(y / 2f), 0), 0.5f).SetDelay(0.25f).OnComplete(() =>
                    {
                        KillBootValueAnimation();

                    });
                });
            }

        }
        #endregion
        internal void KillBootValueAnimation()
        {
            Debug.Log(" Kill Coin COllect ANimation ");
            for (int i = 0; i < BootValueObjList.Count; i++)
            {
                Destroy(BootValueObjList[i].gameObject);
            }
            CallBreak_GS.Inst.collectBootAnimation = false;
            BootValueObjList.Clear();

        }
        #region Kill Onging card deal animation for rejoin
        internal void KillCardDealAnimation()
        {

            tempGeneratedCardList.Clear();
            if (DealAnimEnumeator != null)
            {
                Debug.Log(" Stop Corutine ");
                DealAnimEnumeator = StartCardDeal(0);

                StopCoroutine(DealAnimEnumeator);

                StopCoroutine(StartCardDeal(0));
                StopAllCoroutines();
                DealAnimEnumeator = null;
                this.DOKill();
            }

            CallBreak_UIManager.Inst.MyCardsParent.transform.DOScaleX(0, 0f);
            int count = CallBreak_UIManager.Inst.MyCardsParent.transform.childCount;

            for (int i = 0; i < count; i++)
            {
                CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(0).transform.DOKill(complete: true);
                tempGeneratedCardList.Remove(CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(0).gameObject);
                Debug.Log(" Card Name " + CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(0).name);
                DestroyImmediate(CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(0).gameObject);
            }
            for (int i = 0; i < tempGeneratedCardList.Count; i++)
            {
                tempGeneratedCardList[i].transform.DOKill();
                Debug.Log(tempGeneratedCardList[i].name + " Count " + tempGeneratedCardList.Count);
                DestroyImmediate(tempGeneratedCardList[i]);
            }
            CallBreak_UIManager.Inst.MyCardsParent.transform.DOScaleX(1, 0.2f);
        }
        #endregion

        void ResetAllAfterNewGame()
        {
            for (int i = 0; i < DisCardedCardsList.Count; i++)
            {
                DestroyImmediate(DisCardedCardsList[i]);
            }
            Debug.Log(" Called From Card Deal  ");

            DisCardedCardDestroyImmediate();

            KillBootValueAnimation();


            DisCardedCardsList = new List<GameObject>();
            DisCardedCardsList.Clear();            
            cardCollectAnimation = false;
            transform.DOKill();
        }

        internal void ResetPreviousThrowsCards()
        {
            try
            {
                for (int i = 0; i < DisCardedCardsList.Count; i++)
                {
                    DestroyImmediate(DisCardedCardsList[i]);
                }
                Debug.Log(" Called From ResetPreviousThrowsCards ");

                DisCardedCardDestroyImmediate();
                DisCardedCardsList.Clear();

                //stop timer
                for (int i = 0; i < CallBreak_PlayerController.Inst.playerList.Count; i++)
                {
                    CallBreak_PlayerController.Inst.playerList[i].TimerBG.transform.localScale = Vector3.zero;
                    CallBreak_PlayerController.Inst.playerList[i].darkImageForBetterView.enabled = false;

                }
                //hide hand card
                //  CallBreak_UIManager.Inst.MyCardsParent.transform.DOScaleX(0, 0);

            }
            catch (System.Exception ex)
            {
                Debug.Log("ResetPreviousThrowsCards Exception " + ex.ToString());
            }
        }
    }

}