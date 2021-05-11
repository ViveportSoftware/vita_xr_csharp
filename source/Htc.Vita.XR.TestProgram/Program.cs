using System;
using Htc.Vita.Core.Log;

namespace Htc.Vita.XR.TestProgram
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var openVRManager = OpenVRManager.GetInstance();
            var checkResult = openVRManager.Check();
            var isApiReady = checkResult.IsApiReady;
            var isRuntimeInstalled = checkResult.IsRuntimeInstalled;
            var isRuntimeRunning = checkResult.IsRuntimeRunning;
            Logger.GetInstance(typeof(Program)).Info($"IsApiReady: {isApiReady}");
            Logger.GetInstance(typeof(Program)).Info($"IsHmdPresent: {checkResult.IsHmdPresent}");
            Logger.GetInstance(typeof(Program)).Info($"IsRuntimeInstalled: {isRuntimeInstalled}");
            Logger.GetInstance(typeof(Program)).Info($"IsRuntimeRunning: {isRuntimeRunning}");
            if (!isApiReady)
            {
                Logger.GetInstance(typeof(Program)).Error("OpenVR API is not ready.");
                return;
            }
            if (!isRuntimeInstalled)
            {
                Logger.GetInstance(typeof(Program)).Error("OpenVR runtime is not installed.");
                return;
            }
            if (!isRuntimeRunning)
            {
                Logger.GetInstance(typeof(Program)).Error("OpenVR runtime is not running.");
                return;
            }
            Console.ReadKey();

            openVRManager.OnEnableHomeAppSettingsHaveChanged += OpenVRManager_OnEnableHomeAppSettingsHaveChanged;
            openVRManager.OnSceneApplicationStateChanged += OpenVRManager_OnSceneApplicationStateChanged;
            var isRuntimeConnected = openVRManager.IsRuntimeConnected();
            if (!isRuntimeConnected)
            {
                Logger.GetInstance(typeof(Program)).Info("OpenVR runtime is not connected yet. try to connect.");
                var connected = openVRManager.ConnectRuntime();
                if (!connected)
                {
                    Logger.GetInstance(typeof(Program)).Error("Con not connect to OpenVR runtime.");
                    return;
                }
            }
            Logger.GetInstance(typeof(Program)).Info("OpenVR runtime is connected.");
            Logger.GetInstance(typeof(Program)).Info($"EnableHomeApp: {openVRManager.IsHomeAppEnabled()}");
            Logger.GetInstance(typeof(Program)).Info($"SceneApplicationState: {openVRManager.GetSceneApplicationState()}");
            Console.ReadKey();

            Logger.GetInstance(typeof(Program)).Info("Try to disconnect and reconnect OpenVR runtime.");
            var disconnected = openVRManager.DisconnectRuntime();
            if (!disconnected)
            {
                Logger.GetInstance(typeof(Program)).Error("Con not disconnect to OpenVR runtime. part 1");
                return;
            }
            isRuntimeConnected = openVRManager.IsRuntimeConnected();
            if (isRuntimeConnected)
            {
                Logger.GetInstance(typeof(Program)).Error("Con not disconnect to OpenVR runtime. part 2");
                return;
            }
            var reconnected = openVRManager.ConnectRuntime();
            if (!reconnected)
            {
                Logger.GetInstance(typeof(Program)).Error("Con not reconnect to OpenVR runtime. part 1");
                return;
            }
            isRuntimeConnected = openVRManager.IsRuntimeConnected();
            if (!isRuntimeConnected)
            {
                Logger.GetInstance(typeof(Program)).Error("Con not reconnect to OpenVR runtime. part 2");
                return;
            }
            Logger.GetInstance(typeof(Program)).Info("OpenVR runtime is reconnected.");
            Console.ReadKey();

            Logger.GetInstance(typeof(Program)).Info("Done");
        }

        private static void OpenVRManager_OnSceneApplicationStateChanged(OpenVRManager.SceneApplicationState sceneApplicationState)
        {
            Logger.GetInstance(typeof(Program)).Info($"SceneApplicationState is changed to {sceneApplicationState}");
        }

        private static void OpenVRManager_OnEnableHomeAppSettingsHaveChanged(bool homeAppEnabled)
        {
            Logger.GetInstance(typeof(Program)).Info($"EnableHomeApp: {homeAppEnabled}");
        }
    }
}
