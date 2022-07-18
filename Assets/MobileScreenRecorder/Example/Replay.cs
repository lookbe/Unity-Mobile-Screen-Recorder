using System;
using UnityEngine;
public class Replay : MonoBehaviour
{
    public bool enableMicrophone = false;
    public bool enableCamera = false;

    string lastError = "";
    void OnGUI()
    {
        if (!ExtraFeatures.ScreenRecorder.instance.available)
        {
            return;
        }
        var recording = ExtraFeatures.ScreenRecorder.instance.isRecording;
        string caption = recording ? "Stop Recording" : "Start Recording";
        if (GUI.Button(new Rect(10, 10, 500, 200), caption))
        {
            try
            {
                recording = !recording;
                if (recording)
                {
                    ExtraFeatures.ScreenRecorder.instance.StartRecording();
                }
                else
                {
                    ExtraFeatures.ScreenRecorder.instance.StopRecording();
                }
            }
            catch (Exception e)
            {
                lastError = e.ToString();
            }
        }

        //GUI.Label(new Rect(10, 220, 500, 50), "Last error: " + ReplayKit.lastError);
        //GUI.Label(new Rect(10, 280, 500, 50), "Last exception: " + lastError);

        if (ExtraFeatures.ScreenRecorder.instance.recordingAvailable)
        {
            if (GUI.Button(new Rect(10, 350, 500, 200), "Preview"))
            {
                ExtraFeatures.ScreenRecorder.instance.Preview();
            }
            if (GUI.Button(new Rect(10, 560, 500, 200), "Discard"))
            {
                ExtraFeatures.ScreenRecorder.instance.Discard();
            }
        }
    }

    //void Update()
    //{
    //    // If the camera is enabled, show the recorded video overlaying the game.
    //    if (ReplayKit.isRecording && enableCamera)
    //        ReplayKit.ShowCameraPreviewAt(10, 350, 200, 200);
    //    else
    //        ReplayKit.HideCameraPreview();
    //}
}