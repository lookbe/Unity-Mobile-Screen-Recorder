#if  PLATFORM_ANDROID && !UNITY_EDITOR
#define ANDROID_SCREEN_RECORD
#elif PLATFORM_IOS && !UNITY_EDITOR
#define IOS_SCREEN_RECORDER
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

#if ANDROID_SCREEN_RECORD
using UnityEngine.Android;
#elif IOS_SCREEN_RECORDER
using UnityEngine.iOS;
using UnityEngine.Apple.ReplayKit;
#endif

namespace ExtraFeatures
{
    public class ScreenRecorder : MonoBehaviour
    {
        public static ScreenRecorder instance;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            if (instance == null)
            {
                instance = new GameObject("ScreenRecorder").AddComponent<ScreenRecorder>();
                DontDestroyOnLoad(instance.gameObject);
            }
        }

#if ANDROID_SCREEN_RECORD
        AndroidJavaObject screenRecorder;
        Dictionary<string, bool> permission;
        bool waitPermission;

        void PermissionCallbacks_PermissionDeniedAndDontAskAgain(string permissionName)
        {
            permission[permissionName] = false;
            Debug.Log($"{permissionName} denied");
            waitPermission = false;
        }

        void PermissionCallbacks_PermissionDenied(string permissionName)
        {
            permission[permissionName] = false;
            Debug.Log($"{permissionName} denied");
            waitPermission = false;
        }

        void PermissionCallbacks_PermissionGranted(string permissionName)
        {
            permission[permissionName] = true;
            Debug.Log($"{permissionName} granted");
            waitPermission = false;
        }

        public void SuccessCallback(string msg)
        {
            Debug.Log(msg);
        }

        public void ErrorCallback(string msg)
        {
            Debug.Log(msg);
        }
#endif

        public bool available
        {
            get
            {
#if IOS_SCREEN_RECORDER
                return ReplayKit.APIAvailable;
#elif ANDROID_SCREEN_RECORD
                return true;
#else
                return false;
#endif
            }
        }

        public bool recordingAvailable
        {
            get
            {
#if IOS_SCREEN_RECORDER
                return ReplayKit.recordingAvailable;
#else
                return false;
#endif
            }
        }

        public bool isRecording
        {
            get
            {
#if IOS_SCREEN_RECORDER
                return ReplayKit.isRecording;
#elif ANDROID_SCREEN_RECORD
                return screenRecorder.CallStatic<bool>("isRecording");
#else
                return false;
#endif
            }
        }

        private void Awake()
        {
#if ANDROID_SCREEN_RECORD
            permission = new Dictionary<string, bool>();

            permission[Permission.ExternalStorageWrite] = false;
            permission[Permission.Microphone] = false;

            screenRecorder = new AndroidJavaObject("com.setik.androidutils.ScreenRecorder");
            screenRecorder.CallStatic("Initialize");
#endif
        }
        public void StartRecording()
        {
#if IOS_SCREEN_RECORDER
            ReplayKit.StartRecording(false, false);
#elif ANDROID_SCREEN_RECORD
            StartCoroutine(StartRecording_internal());
#endif
        }

#if ANDROID_SCREEN_RECORD
        public IEnumerator StartRecording_internal()
        {
            if (!Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
            {
                permission[Permission.ExternalStorageWrite] = false;

                waitPermission = true;
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
                callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
                callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;
                
                Permission.RequestUserPermission(Permission.ExternalStorageWrite, callbacks);
                while (waitPermission) yield return null;
            }
            else
            {
                permission[Permission.ExternalStorageWrite] = true;
            }

            if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
            {
                permission[Permission.Microphone] = false;

                waitPermission = true;
                var callbacks = new PermissionCallbacks();
                callbacks.PermissionDenied += PermissionCallbacks_PermissionDenied;
                callbacks.PermissionGranted += PermissionCallbacks_PermissionGranted;
                callbacks.PermissionDeniedAndDontAskAgain += PermissionCallbacks_PermissionDeniedAndDontAskAgain;

                Permission.RequestUserPermission(Permission.Microphone, callbacks);
                while (waitPermission) yield return null;
            }
            else
            {
                permission[Permission.Microphone] = true;
            }

            if (permission[Permission.ExternalStorageWrite] && permission[Permission.Microphone])
            {
                // save to folder Movie/ApplicationName
                screenRecorder.CallStatic("StartRecord", Application.productName);
            }
        }
#endif

        public void StopRecording()
        {
#if IOS_SCREEN_RECORDER
            ReplayKit.StopRecording();
#elif ANDROID_SCREEN_RECORD
            screenRecorder.CallStatic("StopRecord");
#endif
        }

        public void Preview()
        {
#if IOS_SCREEN_RECORDER
            ReplayKit.Preview();
#endif
        }

        public void Discard()
        {
#if IOS_SCREEN_RECORDER
            ReplayKit.Discard();
#endif
        }
    }
}
