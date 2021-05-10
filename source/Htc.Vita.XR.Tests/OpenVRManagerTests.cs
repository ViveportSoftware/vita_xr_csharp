using Htc.Vita.Core.Log;
using Xunit;

namespace Htc.Vita.XR.Tests
{
    public static class OpenVRManagerTests
    {
        [Fact]
        public static void Default_0_GetInstance()
        {
            var openVrManager = OpenVRManager.GetInstance();
            Assert.NotNull(openVrManager);
        }

        [Fact]
        public static void Default_1_Check()
        {
            var openVrManager = OpenVRManager.GetInstance();
            var checkResult = openVrManager.Check();
            Logger.GetInstance(typeof(OpenVRManagerTests)).Info($"IsApiReady: {checkResult.IsApiReady}");
            Logger.GetInstance(typeof(OpenVRManagerTests)).Info($"IsHmdPresent: {checkResult.IsHmdPresent}");
            Logger.GetInstance(typeof(OpenVRManagerTests)).Info($"IsRuntimeInstalled: {checkResult.IsRuntimeInstalled}");
            Logger.GetInstance(typeof(OpenVRManagerTests)).Info($"IsRuntimeRunning: {checkResult.IsRuntimeRunning}");
        }
    }
}
