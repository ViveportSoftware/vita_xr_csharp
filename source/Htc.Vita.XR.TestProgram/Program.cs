using System;
using Htc.Vita.Core.Log;

namespace Htc.Vita.XR.TestProgram
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var openVRManager = OpenVRManager.GetInstance();
            openVRManager.OnRuntimeConnected += OpenVRManager_OnRuntimeConnected;
            openVRManager.OnRuntimeDisconnected += OpenVRManager_OnRuntimeDisconnected;
            openVRManager.OnRuntimeKilled += OpenVRManager_OnRuntimeKilled;
            openVRManager.OnRuntimeLaunched += OpenVRManager_OnRuntimeLaunched;
            Logger.GetInstance(typeof(Program)).Info("Start OpenVR runtime watching");
            openVRManager.StartRuntimeWatching();
            Console.ReadKey();

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
                Logger.GetInstance(typeof(Program)).Error("OpenVR runtime is not running. Try to launch runtime");
                var success = openVRManager.LaunchRuntime();
                if (!success)
                {
                    Logger.GetInstance(typeof(Program)).Warn("OpenVR runtime can not be launched. Skip");
                    return;
                }
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
                    Logger.GetInstance(typeof(Program)).Error($"Con not connect to OpenVR runtime. error: {openVRManager.GetLastRuntimeConnectingError()}");
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

            var killed = openVRManager.KillRuntime();
            if (!killed)
            {
                Logger.GetInstance(typeof(Program)).Error("Can not kill OpenVR runtime");
            }
            Console.ReadKey();

            Logger.GetInstance(typeof(Program)).Info("Stop OpenVR runtime watching");
            openVRManager.StopRuntimeWatching();
            Console.ReadKey();

            Logger.GetInstance(typeof(Program)).Info("Done");
        }

        private static void OpenVRManager_OnRuntimeConnected()
        {
            Logger.GetInstance(typeof(Program)).Info("OpenVR Runtime is connected");
        }

        private static void OpenVRManager_OnRuntimeDisconnected()
        {
            Logger.GetInstance(typeof(Program)).Info("OpenVR Runtime is disconnected");
        }

        private static void OpenVRManager_OnRuntimeKilled()
        {
            Logger.GetInstance(typeof(Program)).Info("OpenVR Runtime is killed");
        }

        private static void OpenVRManager_OnRuntimeLaunched()
        {
            Logger.GetInstance(typeof(Program)).Info("OpenVR Runtime is launched");
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
