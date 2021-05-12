using System;
using System.Threading;
using Htc.Vita.Core.Log;
using Xunit;

namespace Htc.Vita.XR.Tests
{
    public static class OpenVRManagerTests
    {
        private const int HomeAppKillingTimeInSec = 5;
        private const int RuntimeLaunchingTimeInSec = 10;

        [Fact]
        public static void Default_0_GetInstance()
        {
            var openVRManager = OpenVRManager.GetInstance();
            Assert.NotNull(openVRManager);
        }

        [Fact]
        public static void Default_1_Check()
        {
            var openVRManager = OpenVRManager.GetInstance();
            var checkResult = openVRManager.Check();
            Logger.GetInstance(typeof(OpenVRManagerTests)).Info($"IsApiReady: {checkResult.IsApiReady}");
            Logger.GetInstance(typeof(OpenVRManagerTests)).Info($"IsHmdPresent: {checkResult.IsHmdPresent}");
            Logger.GetInstance(typeof(OpenVRManagerTests)).Info($"IsRuntimeInstalled: {checkResult.IsRuntimeInstalled}");
            Logger.GetInstance(typeof(OpenVRManagerTests)).Info($"IsRuntimeRunning: {checkResult.IsRuntimeRunning}");
        }

        [Fact]
        public static void Default_2_IsRuntimeConnected()
        {
            var openVRManager = OpenVRManager.GetInstance();
            var checkResult = openVRManager.Check();
            if (!checkResult.IsApiReady)
            {
                Logger.GetInstance(typeof(OpenVRManagerTests)).Warn("OpenVR API is not ready. Skip");
                return;
            }
            if (!checkResult.IsRuntimeRunning)
            {
                Logger.GetInstance(typeof(OpenVRManagerTests)).Warn("OpenVR runtime is not running. Try to launch runtime");
                var success = openVRManager.LaunchRuntime();
                if (!success)
                {
                    Logger.GetInstance(typeof(OpenVRManagerTests)).Warn("OpenVR runtime can not be launched. Skip");
                    return;
                }

                SpinWait.SpinUntil(() => false, TimeSpan.FromSeconds(RuntimeLaunchingTimeInSec));
            }

            openVRManager.OnEnableHomeAppSettingsHaveChanged += OpenVRManager_OnEnableHomeAppSettingsHaveChanged;
            openVRManager.OnSceneApplicationStateChanged += OpenVRManager_OnSceneApplicationStateChanged;

            var isRuntimeConnected = openVRManager.IsRuntimeConnected();
            if (isRuntimeConnected)
            {
                Assert.True(openVRManager.DisconnectRuntime());
            }

            Assert.False(openVRManager.IsRuntimeConnected());
            var connected = openVRManager.ConnectRuntime();
            if (!connected)
            {
                Logger.GetInstance(typeof(OpenVRManagerTests)).Error($"Con not connect to OpenVR runtime. error: {openVRManager.GetLastRuntimeConnectingError()}");
                return;
            }
            Assert.True(openVRManager.ConnectRuntime());
            Assert.True(openVRManager.IsRuntimeConnected());
            Assert.True(openVRManager.DisconnectRuntime());
            Assert.True(openVRManager.DisconnectRuntime());
            Assert.False(openVRManager.IsRuntimeConnected());
        }

        [Fact]
        public static void Default_3_EnableHomeApp()
        {
            var openVRManager = OpenVRManager.GetInstance();
            var checkResult = openVRManager.Check();
            if (!checkResult.IsApiReady)
            {
                Logger.GetInstance(typeof(OpenVRManagerTests)).Warn("OpenVR API is not ready. Skip");
                return;
            }
            if (!checkResult.IsRuntimeRunning)
            {
                Logger.GetInstance(typeof(OpenVRManagerTests)).Warn("OpenVR runtime is not running. Try to launch runtime");
                var success = openVRManager.LaunchRuntime();
                if (!success)
                {
                    Logger.GetInstance(typeof(OpenVRManagerTests)).Warn("OpenVR runtime can not be launched. Skip");
                    return;
                }

                SpinWait.SpinUntil(() => false, TimeSpan.FromSeconds(RuntimeLaunchingTimeInSec));
            }

            openVRManager.OnEnableHomeAppSettingsHaveChanged += OpenVRManager_OnEnableHomeAppSettingsHaveChanged;

            var isRuntimeConnected = openVRManager.IsRuntimeConnected();
            if (!isRuntimeConnected)
            {
                var connected = openVRManager.ConnectRuntime();
                if (!connected)
                {
                    Logger.GetInstance(typeof(OpenVRManagerTests)).Error($"Con not connect to OpenVR runtime. error: {openVRManager.GetLastRuntimeConnectingError()}");
                    return;
                }
            }

            var isHomeAppEnabled = openVRManager.IsHomeAppEnabled();
            if (isHomeAppEnabled)
            {
                const string homeAppKey = "openvr.tool.steamvr_environments";
                var homeAppProcessId = openVRManager.GetApplicationProcessId(homeAppKey);
                Logger.GetInstance(typeof(OpenVRManagerTests)).Info($"{homeAppKey}({homeAppProcessId})");
                Assert.False(homeAppProcessId == 0);

                Assert.True(openVRManager.EnableHomeApp(false));
                Assert.True(openVRManager.EnableHomeApp(false));
                Assert.False(openVRManager.IsHomeAppEnabled());

                SpinWait.SpinUntil(() => false, TimeSpan.FromSeconds(HomeAppKillingTimeInSec));

                var applicationError = openVRManager.LaunchApplication(homeAppKey);
                Logger.GetInstance(typeof(OpenVRManagerTests)).Info($"Try to launch {homeAppKey}, error: {applicationError}");
                Assert.True(applicationError == OpenVRManager.ApplicationError.None
                        || applicationError == OpenVRManager.ApplicationError.ApplicationAlreadyRunning);
            }
            else
            {
                Assert.True(openVRManager.EnableHomeApp(true));
                Assert.True(openVRManager.EnableHomeApp(true));
                Assert.True(openVRManager.IsHomeAppEnabled());
            }
        }

        private static void OpenVRManager_OnSceneApplicationStateChanged(OpenVRManager.SceneApplicationState sceneApplicationState)
        {
            Logger.GetInstance(typeof(OpenVRManagerTests)).Warn($"sceneApplicationState: {sceneApplicationState}");
        }

        private static void OpenVRManager_OnEnableHomeAppSettingsHaveChanged(bool homeAppEnabled)
        {
            Logger.GetInstance(typeof(OpenVRManagerTests)).Warn($"homeAppEnabled: {homeAppEnabled}");
        }
    }
}
