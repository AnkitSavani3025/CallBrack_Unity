﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ThreePlusGamesCallBreak
{
    public static class CallBreak_WaitExtension
    {
        public static void WaitforTime(this MonoBehaviour mono, float delay, UnityAction action)
        {
            mono.StartCoroutine(ExecuteActionAfterDelay(delay, action));
        }

        private static IEnumerator ExecuteActionAfterDelay(float delay, UnityAction action)
        {
            yield return new WaitForSecondsRealtime(delay);
            action.Invoke();
            yield break;
        }
    }
}
