using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using CallBreak_Socketmanager;
using System.Threading.Tasks;
using System;

namespace ThreePlusGamesCallBreak
{
    public class CallBreak_FTUEManager : MonoBehaviour
    {
        public static CallBreak_FTUEManager instance;
        [SerializeField]
        private List<CallBreak_Users> FTUEplayerList;
        [SerializeField]
        private List<Sprite> ftueCardSpriteList;[SerializeField]
        private List<GameObject> ftueCardList;
        [SerializeField]
        internal GameObject FTUEStepHolder, ftueBidPopup, selectBidHand, step1, step2, StartCardDealPopup, bidHintStep, throwCardPopup, winHandPopup, letsPlayPopup, handForCardThrow, ftueHighLightCard, profileHighLight;
        [SerializeField]
        Button skipOneStepBtn, skipAllBtn;
        [SerializeField]
        public float popupShowSpeed;
        public float textWriteAnimationSpeed;
        int charIndex, currentPopupNo, selectedBid;
        Text textContainerText;
        string textValue, colorString;
        public List<GameObject> tempDisCardedCardsList;
        bool FTUERunning = true;


        private void Awake()
        {
            if (instance == null)
                instance = this;

        }

        internal void StartFUTE()
        {
            CallBreak_UIManager.Inst.headerMainParent.SetActive(false);
            FTUEStepHolder.GetComponent<Canvas>().enabled = true;
            skipAllBtn.gameObject.transform.DOScale(1, 0.5f);
            OpenStep1();
        }

        void OpenStep1()
        {
            step1.SetActive(true);
            step1.transform.DOLocalMoveX(0, 0.8f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                if (FTUERunning)
                    Invoke(nameof(CloseStep1), 3);
            });
        }
        void CloseStep1()
        {
            step1.transform.DOLocalMoveX(-2000, 0.5f).SetEase(Ease.InBounce).OnComplete(() =>
            {
                if (FTUERunning)
                {
                    step1.SetActive(false);
                    Invoke(nameof(OpenStep2), 1);
                }
            });
        }



        void OpenStep2()
        {
            currentPopupNo++;
            WritePopUpAnimation(step2);
        }
        void CloseStep2()
        {
            step2.transform.DOScale(0, popupShowSpeed / 2).SetEase(Ease.InBounce).OnComplete(() =>
            {

                if (FTUERunning)
                {
                    step2.SetActive(false);
                    Invoke(nameof(OpenStartCardDealPopup), 1);
                }
            });


        }



        void OpenStartCardDealPopup()
        {
            currentPopupNo++;
            WritePopUpAnimation(StartCardDealPopup);
        }
        void CloseStartCardDealPopup()
        {
            StartCardDealPopup.transform.DOScale(0, popupShowSpeed / 2).SetEase(Ease.InBounce);
        }



        public void OpenThrowCardPopup()
        {
            for (int i = 0; i < FTUEplayerList.Count; i++)
                FTUEplayerList[i].BidBG.transform.DOScale(0, 0.5f);
            colorString = "HAND";
            currentPopupNo = 3;
            WritePopUpAnimation(throwCardPopup);
        }
        public void CloseThrowCardPopup()
        {
            throwCardPopup.transform.DOScale(0, popupShowSpeed / 2).SetEase(Ease.InBounce);
        }
        public void OpenWinHandPopup()
        {
            FTUEStepHolder.GetComponent<Image>().enabled = true;

            currentPopupNo = 4;
            //WritePopUpAnimation(winHandPopup);
            winHandPopup.SetActive(true);
            winHandPopup.transform.DOScale(1, 0);
            winHandPopup.transform.DOLocalMoveY(0, 0.8f).SetEase(Ease.OutBounce).OnComplete(() =>
            {

                if (FTUERunning)
                    Invoke(nameof(CloseWinHandPopup), 3);
            });

        }
        public void CloseWinHandPopup()
        {
            winHandPopup.transform.DOLocalMoveY(1500, 0.8f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                if (FTUERunning)
                    OpenLetsPlayPopup();
            });

        }
        public void OpenLetsPlayPopup()
        {
            currentPopupNo = 5;
            skipAllBtn.transform.DOScale(0, 1);
            letsPlayPopup.SetActive(true);
            letsPlayPopup.transform.DOScale(1, popupShowSpeed).SetEase(Ease.OutBounce);
        }
        public void CloseLetsPlayPopup()
        {
            letsPlayPopup.transform.DOScale(0, popupShowSpeed / 2).SetEase(Ease.InBounce).OnComplete(() =>
            {
                if (FTUERunning)
                    ResetAllFTUES();

            });
        }

        private void WritePopUpAnimation(GameObject _currentStep)
        {
            _currentStep.SetActive(true);
            textContainerText = _currentStep.GetComponentInChildren<Text>();
            textValue = textContainerText.text;

            textContainerText.text = "";
            _currentStep.transform.DOScale(1, popupShowSpeed).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                if (FTUERunning)
                {
                    CancelInvoke(nameof(TextAnimetion));
                    InvokeRepeating(nameof(TextAnimetion), 0.08F, textWriteAnimationSpeed);
                }
            });
        }
        void TextAnimetion()
        {
            string[] subStrings;
            charIndex++;
            textContainerText.text = "";
            string newString = textValue.Substring(0, charIndex);
            if (colorString != null && newString.Contains(colorString))
            {
                subStrings = newString.Split(' ');
                newString = subStrings[0] + "<color=#00e390>" + colorString + "</color>" + subStrings[1];
            }
            textContainerText.text = newString;
            if (charIndex >= textValue.Length)
            {
                CancelInvoke(nameof(TextAnimetion));
                CloseCurrentPopUp(currentPopupNo);
                charIndex = 0;
            }
        }

        public void StartCardDealAnim()
        {
            CancelInvoke(nameof(StartCardDealAnim));
            CloseStartCardDealPopup();
            StartCoroutine(CardThrowFTUE());
        }



        internal IEnumerator CardThrowFTUE()
        {
            FTUEStepHolder.GetComponent<Image>().enabled = false;
            skipAllBtn.interactable = false;
            yield return new WaitForSeconds(1f);
            CallBreak_UIManager.Inst.MyCardsParent.GetComponent<HorizontalLayoutGroup>().enabled = false;
            for (int n = 0; n < CallBreak_UIManager.Inst.MyCardsParent.transform.childCount; n++)
            {
                DestroyImmediate(CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(n).gameObject);
            }
            yield return new WaitForSeconds(0.1f);

            for (int i = 0; i < 13; i++)
            {
                GameObject cloneCard = Instantiate(CallBreak_UIManager.Inst.ftueCardPrefab, CallBreak_UIManager.Inst.CardGeneratePoint.transform);
                cloneCard.transform.DOScaleX(1, 0);
                cloneCard.transform.SetParent(CallBreak_UIManager.Inst.MyCardsParent.transform);
                cloneCard.SetActive(false);
                cloneCard.GetComponent<Image>().sprite = ftueCardSpriteList[i];
                cloneCard.name = ftueCardSpriteList[i].name;
                ftueCardList.Add(cloneCard);
            }

            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < CallBreak_UIManager.Inst.MyCardsParent.transform.childCount; i++)
            {
                RectTransform cardRectTransform = CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(i).GetComponent<RectTransform>();
                cardRectTransform.gameObject.SetActive(true);
                cardRectTransform.DOAnchorPos(CallBreak_UIManager.Inst.FaekCardHolder.transform.GetChild(i).GetComponent<RectTransform>().localPosition, 2, true).SetEase(Ease.OutElastic, 1, 1.3f).OnComplete(() =>
                {
                    if (FTUERunning)
                        if (cardRectTransform.name == CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(i - 1).name)
                        {
                            CallBreak_UIManager.Inst.MyCardsParent.GetComponent<HorizontalLayoutGroup>().enabled = true;
                            skipAllBtn.interactable = true;
                        }
                });
                CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.CardDealing);
                yield return new WaitForSeconds(0.1f);
            }

            Invoke(nameof(SelectBidAnim), 3);
        }
        public void SelectBidAnim()
        {
            StopCoroutine(CardThrowFTUE());
            ftueBidPopup.SetActive(true);
            ftueBidPopup.GetComponent<Canvas>().enabled = true;
            ftueBidPopup.transform.GetChild(0).DOScale(1, 0.5f).OnComplete(() =>
            {
                if (FTUERunning)
                {
                    bidHintStep.SetActive(true);
                    bidHintStep.transform.DOScale(1, popupShowSpeed).SetEase(Ease.OutBounce);
                }
            });
        }


        public void SelectBidValue(int value)
        {
            Debug.Log(" Bid Selected " + value);

            int x = UnityEngine.Random.Range(0, 10);

            ftueBidPopup.transform.GetChild(0).DOScale(0, 0.5f).OnComplete(() =>
            {
                if (FTUERunning)
                {
                    ftueBidPopup.GetComponent<Canvas>().enabled = false;
                    ftueBidPopup.SetActive(false);
                    IEnumerator enumerator = SelectBid(value);
                    StartCoroutine(enumerator);
                }
            });

            bidHintStep.transform.DOScale(0, 0.5f);
        }
        IEnumerator SelectBid(int value)
        {
            selectedBid = value;

            yield return new WaitForSeconds(0.6f);
            for (int i = 0; i < FTUEplayerList.Count; i++)
            {
                if (value == 0)
                    value = UnityEngine.Random.Range(2, 11);

                CallBreak_Users p = FTUEplayerList[i];

                Debug.Log(p.name);
                p.BidBG.transform.DOScale(1, 0.1f);
                p.BidBG.transform.GetChild(0).transform.GetComponent<Text>().text = "<color=#FEEDA8>Bid</color>" + Environment.NewLine + value;
                p.MyBidSelected = true;
                p.MyBidValue = value;
                p.BidAndHandValue.text = "0/" + value;
                Debug.Log("555");
                //CallBreak_UIManager.Inst.CenterMsgBG.transform.DOScale(0, 0.2f);
                //CallBreak_UIManager.Inst.CenterMsgBG.GetComponent<Canvas>().enabled = false;
                CallBreak_AllPopupHandler.Instance.CloseCenterMsgToast();
                value = 0;
                yield return new WaitForSeconds(0.8f);

            }
            yield return new WaitForSeconds(2f);

            OpenThrowCardPopup();
        }



        public void CloseCurrentPopUp(int x)
        {

            Debug.Log("--- " + x);
            if (x == 1)
                Invoke(nameof(CloseStep2), 3);
            else if (x == 2)
            {
                for (int i = 0; i < FTUEplayerList.Count; i++)
                {
                    CallBreak_Users p = FTUEplayerList[i];
                    p.transform.DOScale(1, 0.2f);
                }
                Invoke(nameof(StartCardDealAnim), 3);
            }
            else if (x == 3)
                GenerateAnimForThrowCard();

        }



        public void GenerateAnimForThrowCard()
        {
            Debug.Log(" Generate HAnd Now For Card ");
            for (int i = 0; i < ftueCardList.Count; i++)
            {
                if (ftueCardList[i].name != "S-1")
                {
                    ftueCardList[i].GetComponent<Image>().DOColor(CallBreak_UIManager.Inst.DisableCardColor, 0);
                }
                else
                {
                    ftueCardList[i].AddComponent<CallBreak_FTUECard>();
                    GameObject animationHand = Instantiate(handForCardThrow, ftueCardList[i].transform);
                    Instantiate(ftueHighLightCard, ftueCardList[i].transform);
                    Canvas canvas = animationHand.AddComponent<Canvas>();
                    canvas.overrideSorting = true;
                    canvas.sortingOrder = 11;
                }
            }
        }
        public void ClosePopupAndThrowAnotherCard()
        {
            CloseThrowCardPopup();

            Invoke(nameof(PLayer2ThrowCard), 1);
        }

        void PLayer2ThrowCard()
        {
            GameObject CardPrefab = CallBreak_CardDeal.Inst.CardPREFAB;
            GameObject Card = Instantiate(CardPrefab, CallBreak_CardDeal.Inst.discardedCardContainer.transform);
            Card.AddComponent<Canvas>().overrideSorting = true;
            Card.transform.GetComponent<Canvas>().sortingOrder = CallBreak_SocketEventReceiver.Inst.CardOrder;
            List<Sprite> temp = new List<Sprite>();
            temp = CallBreak_AssetsReferences.Inst.AllCardSprite.FindAll(x => x.name == "H-13");
            Card.GetComponent<Image>().sprite = temp[0];
            Vector3 GenPos = CallBreak_UIManager.Inst.P2DisGenPos.transform.localPosition;
            Card.transform.localPosition = new Vector3(GenPos.x + 100, GenPos.y, GenPos.z);
            Card.transform.DORotate(new Vector3(0, 0, -30), 0f);
            Card.transform.DORotate(new Vector3(0, 0, 90), 0.3f);
            Card.GetComponent<RectTransform>().DOSizeDelta(CallBreak_UIManager.Inst.DiscardPositions[1].GetComponent<RectTransform>().sizeDelta, 0f);
            tempDisCardedCardsList.Add(Card);
            Card.transform.DOLocalMove(CallBreak_UIManager.Inst.DiscardPositions[1].transform.localPosition, 0.2f).OnComplete(() =>
            {
                if (FTUERunning)
                    Invoke(nameof(PLayer3ThrowCard), 1);

            });
        }
        void PLayer3ThrowCard()
        {
            GameObject CardPrefab = CallBreak_CardDeal.Inst.CardPREFAB;
            GameObject Card = Instantiate(CardPrefab, CallBreak_CardDeal.Inst.discardedCardContainer.transform);
            Card.AddComponent<Canvas>().overrideSorting = true;
            Card.transform.GetComponent<Canvas>().sortingOrder = CallBreak_SocketEventReceiver.Inst.CardOrder;
            List<Sprite> temp = new List<Sprite>();
            temp = CallBreak_AssetsReferences.Inst.AllCardSprite.FindAll(x => x.name == "C-13");
            Card.GetComponent<Image>().sprite = temp[0];
            Vector3 GenPos = CallBreak_UIManager.Inst.P3DisGenPos.transform.localPosition;
            Card.transform.localPosition = new Vector3(GenPos.x + 100, GenPos.y, GenPos.z);
            Card.transform.DORotate(new Vector3(0, 0, -90), 0f);
            Card.transform.DORotate(new Vector3(0, 0, 0), 0.3f);
            Card.GetComponent<RectTransform>().DOSizeDelta(CallBreak_UIManager.Inst.DiscardPositions[2].GetComponent<RectTransform>().sizeDelta, 0f);
            tempDisCardedCardsList.Add(Card);
            Card.transform.DOLocalMove(CallBreak_UIManager.Inst.DiscardPositions[2].transform.localPosition, 0.2f).OnComplete(() =>
            {
                if (FTUERunning)
                    Invoke(nameof(PLayer4ThrowCard), 1);
            });
        }
        void PLayer4ThrowCard()
        {
            GameObject CardPrefab = CallBreak_CardDeal.Inst.CardPREFAB;
            GameObject Card = Instantiate(CardPrefab, CallBreak_CardDeal.Inst.discardedCardContainer.transform);
            Card.AddComponent<Canvas>().overrideSorting = true;
            Card.transform.GetComponent<Canvas>().sortingOrder = CallBreak_SocketEventReceiver.Inst.CardOrder;
            List<Sprite> temp = new List<Sprite>();
            temp = CallBreak_AssetsReferences.Inst.AllCardSprite.FindAll(x => x.name == "S-13");
            Card.GetComponent<Image>().sprite = temp[0];
            Vector3 GenPos = CallBreak_UIManager.Inst.P4DisGenPos.transform.localPosition;
            Card.transform.localPosition = new Vector3(GenPos.x + 100, GenPos.y, GenPos.z);
            Card.transform.DORotate(new Vector3(0, 0, -60), 0f);
            Card.transform.DORotate(new Vector3(0, 0, -90), 0.3f);
            Card.GetComponent<RectTransform>().DOSizeDelta(CallBreak_UIManager.Inst.DiscardPositions[3].GetComponent<RectTransform>().sizeDelta, 0f);
            tempDisCardedCardsList.Add(Card);
            Card.transform.DOLocalMove(CallBreak_UIManager.Inst.DiscardPositions[3].transform.localPosition, 0.2f).OnComplete(() =>
            {
                if (FTUERunning)
                    Invoke(nameof(CollectHandCards), 1);
            });
        }
        public void CollectHandCards()
        {
            HandCollectAnim("S-1");
        }

        internal async void HandCollectAnim(string wonCardName)
        {
            GameObject cardWon = tempDisCardedCardsList.Find(x => x.name == wonCardName);
            if (cardWon.GetComponent<Canvas>() == null)
            {
                cardWon.AddComponent<Canvas>().overrideSorting = true;
            }
            cardWon.GetComponent<Canvas>().sortingOrder = 3;
            cardWon.transform.DOPunchScale(new Vector2(0.8f, 0.3f), 0.2f, 0, 1).SetEase(Ease.InBounce);
            await Task.Delay(400);
            CallBreak_SoundManager.Inst.PlaySFX(CallBreak_SoundManager.Inst.HandCollect);
            try
            {
                for (int i = 0; i < tempDisCardedCardsList.Count; i++)
                {
                    if (tempDisCardedCardsList[i].gameObject.name != cardWon.name)
                    {
                        try
                        {
                            if (tempDisCardedCardsList[i].GetComponent<Canvas>() == null)
                            {
                                tempDisCardedCardsList[i].AddComponent<Canvas>().overrideSorting = true;
                            }
                            tempDisCardedCardsList[i].GetComponent<Canvas>().sortingOrder = 1;
                        }
                        catch (System.Exception e) { Debug.LogError(e.ToString()); }
                        tempDisCardedCardsList[i].transform.DOLocalMove(cardWon.transform.position, 0.2f);
                        tempDisCardedCardsList[i].transform.SetParent(cardWon.transform);
                    }
                }
            }
            catch (System.Exception e) { Debug.LogError(e.ToString()); }

            MoveCardToWinner(cardWon);
        }

        async void MoveCardToWinner(GameObject cardWon)
        {
            await Task.Delay(100);

            cardWon.transform.DOLocalMove(FTUEplayerList[0].transform.localPosition, 0.32f).OnComplete(() =>
           {
               if (FTUERunning)
               {
                   FTUEplayerList[0].BidAndHandValue.text = "1/" + selectedBid;
                   profileHighLight.SetActive(true);
                   tempDisCardedCardsList.Clear();
                   Destroy(cardWon);
                   OpenWinHandPopup();
               }
           });
        }


        public void ResetAllFTUES()
        {
            FTUERunning = false;
            CancelInvoke();
            StopAllCoroutines();
            FTUEStepHolder.transform.DOScale(0, popupShowSpeed / 4).SetEase(Ease.InBounce).OnComplete(() =>
            {
                FTUEStepHolder.GetComponent<Canvas>().enabled = false;
                skipAllBtn.gameObject.SetActive(false);
                for (int n = 0; n < CallBreak_UIManager.Inst.MyCardsParent.transform.childCount; n++)
                {
                    CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(n).transform.DOKill();
                    Destroy(CallBreak_UIManager.Inst.MyCardsParent.transform.GetChild(n).gameObject);
                }
                for (int i = 0; i < FTUEplayerList.Count; i++)
                {
                    CallBreak_Users p = FTUEplayerList[i];
                    p.transform.DOScale(0, 0.01f);
                    FTUEplayerList[i].BidBG.transform.DOScale(0, 0);
                    FTUEplayerList[i].BidAndHandValue.text = "0/0";
                }
                for (int i = 0; i < tempDisCardedCardsList.Count; i++)
                {
                    Destroy(tempDisCardedCardsList[i]);


                }
                tempDisCardedCardsList.Clear();

                CallBreak_UIManager.Inst.headerMainParent.SetActive(true);

                profileHighLight.SetActive(false);
                CallBreak_GameConfig.Inst.isFTUEFinished = true;
                CallBreak_SocketConnection.intance.StartConnection();
            });
        }
    }
}