using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.InteropServices;
using System.Text;
using System.Web;
using UnityEngine;

namespace CrazyGames
{
    public class GameModule : MonoBehaviour
    {
        private CrazySDK _crazySDK;

        public void Init(CrazySDK crazySDK)
        {
            _crazySDK = crazySDK;
        }

        public void HappyTime()
        {
            _crazySDK.DebugLog("Happy time!");
            _crazySDK.WrapSDKAction(HappyTimeSDK, () => { });
        }

        public void GameplayStart()
        {
            _crazySDK.DebugLog("Gameplay start called");
            _crazySDK.WrapSDKAction(GameplayStartSDK, () => { });
        }

        public void GameplayStop()
        {
            _crazySDK.DebugLog("Gameplay stop called");
            _crazySDK.WrapSDKAction(GameplayStopSDK, () => { });
        }

        public string GetInviteLinkParameter(string key)
        {
            if (Application.isEditor)
            {
                Debug.LogError("Cannot parse url in Unity editor, try running it in a browser");
                return "";
            }

            var queryString = HttpUtility.ParseQueryString(new Uri(Application.absoluteURL).Query);
            return queryString[key];
        }

        public string InviteLink(Dictionary<string, string> parameters)
        {
            return _crazySDK.WrapSDKFunc(() =>
            {
                var paramsJson = Utils.ConvertDictionaryToJson(parameters);
                _crazySDK.DebugLog($"Requesting invite URL with params: {paramsJson}");
                return RequestInviteUrlSDK(paramsJson);
            }, () =>
            {
                _crazySDK.DebugLog("Invite URL requested");
                const string baseUrl = "https://crazygames.com/game/your-game";
                return Utils.AppendQueryParameters(baseUrl, parameters);
            });
        }

        public string ShowInviteButton(Dictionary<string, string> parameters)
        {
            return _crazySDK.WrapSDKFunc(() =>
            {
                var paramsJson = Utils.ConvertDictionaryToJson(parameters);
                _crazySDK.DebugLog($"Requesting invite button with params: {paramsJson}");
                return ShowInviteButtonSDK(paramsJson);
            }, () =>
            {
                _crazySDK.DebugLog("Invite button called");
                const string baseUrl = "https://crazygames.com/game/your-game";
                return Utils.AppendQueryParameters(baseUrl, parameters);
            });
        }

        public void HideInviteButton()
        {
            _crazySDK.DebugLog("Hide invite button called");
            _crazySDK.WrapSDKAction(HideInviteButtonSDK, () => { });
        }

        public void CopyToClipboard(string text)
        {
            _crazySDK.WrapSDKAction(() => CopyToClipboardSDK(text),
                () => { GUIUtility.systemCopyBuffer = text; });
        }


#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void HappyTimeSDK();

        [DllImport("__Internal")]
        private static extern void GameplayStartSDK();

        [DllImport("__Internal")]
        private static extern void GameplayStopSDK();

        [DllImport("__Internal")]
        private static extern string RequestInviteUrlSDK(string urlParams);

        [DllImport("__Internal")]
        private static extern string ShowInviteButtonSDK(string urlParams);

        [DllImport("__Internal")]
        private static extern void HideInviteButtonSDK();

        [DllImport("__Internal")]
        private static extern void CopyToClipboardSDK(string text);
#else
        private void HappyTimeSDK() { }
        private void GameplayStartSDK() { }
        private void GameplayStopSDK() { }

        private string RequestInviteUrlSDK(string urlParams) { return ""; }
        private string ShowInviteButtonSDK(string urlParams) { return ""; }
        private void HideInviteButtonSDK() { }
        private void CopyToClipboardSDK(string text) { }
#endif
    }
}