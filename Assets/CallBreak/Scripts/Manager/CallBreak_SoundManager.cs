using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThreePlusGamesCallBreak
{
    public class CallBreak_SoundManager : MonoBehaviour
    {
        #region Variables
        public static CallBreak_SoundManager Inst;
        public AudioSource SFX;
        public AudioClip CardDealing, HandCollect, UserTurn, ButtonClick, ThrowCard;
        public AudioClip backClip;
        public AudioSource backGroundAudio;
        #endregion

        #region Unity Callbacks
        private void Awake()
        {
            Inst = this;
            if (!PlayerPrefs.HasKey("Sound"))
            {
                PlayerPrefs.SetString("Sound", "ON");
                PlayerPrefs.SetString("Vibrate", "ON");
            }
            if (!PlayerPrefs.HasKey("Music"))
            {
                PlayerPrefs.SetString("Music", "ON");
            }
        }

        private void Start()
        {
            if (PlayerPrefs.GetString("Sound") == "ON")
            {
                CallBreak_UIManager.Inst.soundValueChangeOnAwake = true;
                CallBreak_UIManager.Inst.soundToggle.isOn = true;
                CallBreak_UIManager.Inst.soundSelectImage.sprite = CallBreak_UIManager.Inst.selectSprite;
            }
            else
            {
                CallBreak_UIManager.Inst.soundSelectImage.sprite = CallBreak_UIManager.Inst.deselectSprite;
            }
            if (PlayerPrefs.GetString("Vibrate") == "ON")
            {

                CallBreak_UIManager.Inst.soundValueChangeOnAwake = true;
                CallBreak_UIManager.Inst.vibrateToggle.isOn = true;
                CallBreak_UIManager.Inst.vibrateActive = true;
                CallBreak_UIManager.Inst.vibrationSelectImage.sprite = CallBreak_UIManager.Inst.selectSprite;

            }
            else
            {
                CallBreak_UIManager.Inst.vibrationSelectImage.sprite = CallBreak_UIManager.Inst.deselectSprite;

            }
            if (PlayerPrefs.GetString("Music") == "ON")
            {
                CallBreak_UIManager.Inst.musicToggle.isOn = true;
                CallBreak_UIManager.Inst.musicSelectImage.sprite = CallBreak_UIManager.Inst.selectSprite;
                backGroundAudio.clip = backClip;
                backGroundAudio.Play();
            }
            else
            {
                CallBreak_UIManager.Inst.musicSelectImage.sprite = CallBreak_UIManager.Inst.deselectSprite;
                backGroundAudio.Stop();
            }
        }
        #endregion


        internal void PlaySFX(AudioClip clip)
        {
            if (PlayerPrefs.GetString("Sound") == "ON")
            {
                SFX.PlayOneShot(clip);
            }
        }
    }
}
