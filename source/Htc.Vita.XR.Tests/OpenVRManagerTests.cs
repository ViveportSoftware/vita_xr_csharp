using Htc.Vita.Core.Log;
using Xunit;

namespace Htc.Vita.XR.Tests
{
    public static class OpenVRManagerTests
    {
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
                Logger.GetInstance(typeof(OpenVRManagerTests)).Warn("OpenVR runtime is not running. Skip");
                return;
            }

            openVRManager.OnEnableHomeAppSettingsHaveChanged += OpenVRManager_OnEnableHomeAppSettingsHaveChanged;
            openVRManager.OnSceneApplicationStateChanged += OpenVRManager_OnSceneApplicationStateChanged;

            var isListeningEvent = openVRManager.IsRuntimeConnected();
            if (isListeningEvent)
            {
                Assert.True(openVRManager.DisconnectRuntime());
            }

            Assert.False(openVRManager.IsRuntimeConnected());
            Assert.True(openVRManager.ConnectRuntime());
            Assert.True(openVRManager.ConnectRuntime());
            Assert.True(openVRManager.IsRuntimeConnected());
            Assert.True(openVRManager.DisconnectRuntime());
            Assert.True(openVRManager.DisconnectRuntime());
            Assert.False(openVRManager.IsRuntimeConnected());
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
