using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NatCorder.Clocks;
using NatCorder.Inputs;
using NatCorder;

public class PlayerMovement : MonoBehaviour
{
    private GameObject player;

    private void Awake()
    {
        player = GameObject.Find("Player");
    }
    private void FixedUpdate()
    {
        player.transform.Rotate(0, 120 * Time.deltaTime, 0);
    }

    [Header("Recording")]
    public int videoWidth = 1280;
    public int videoHeight = 720;
    public bool recordMicrophone;

    private MP4Recorder recorder;
    private CameraInput cameraInput;

    public void StartRecording()
    {
        var frameRate = 30;

        Debug.Log("Recording Started !!!");

        // Create a recorder
        recorder = new MP4Recorder(videoWidth, videoHeight, frameRate);
        var clock = new RealtimeClock();
        // And use a `CameraInput` to record the main game camera
        cameraInput = new CameraInput(recorder, clock, Camera.main);
    }

    public async void StopRecording()
    {
        cameraInput.Dispose();
        var path = await recorder.FinishWriting();

        Debug.Log("Recording Stopped ...");

        // Playback recording
        Debug.Log($"Saved recording to: {path}");
        var prefix = Application.platform == RuntimePlatform.IPhonePlayer ? "file://" : "";
        Handheld.PlayFullScreenMovie($"{prefix}{path}");
    }
}