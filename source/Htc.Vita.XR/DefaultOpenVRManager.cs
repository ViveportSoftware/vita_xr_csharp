using System;
using Htc.Vita.Core.Log;

namespace Htc.Vita.XR
{
    /// <summary>
    /// Class DefaultOpenVRManager.
    /// Implements the <see cref="OpenVRManager" />
    /// </summary>
    /// <seealso cref="OpenVRManager" />
    public partial class DefaultOpenVRManager : OpenVRManager
    {
        static DefaultOpenVRManager()
        {
#if NATIVE_LIBRARY_EMBEDDED
            Internal.PrepareLibrary();
#else
            var is64 = IntPtr.Size == 8;
            Core.Runtime.Platform.LoadNativeLib(is64 ? $"x64/{Library.OpenVrApi}.dll" : $"x86/{Library.OpenVrApi}.dll");
#endif
        }

        /// <inheritdoc />
        protected override CheckResult OnCheck()
        {
            var isApiReady = false;
            var isHmdPresent = false;
            try
            {
                isHmdPresent = Interop.VRIsHmdPresent();
                isApiReady = true;
            }
            catch (Exception e)
            {
                Logger.GetInstance(typeof(DefaultOpenVRManager)).Error(e.ToString());
            }

            return new CheckResult
            {
                    IsApiReady = isApiReady,
                    IsHmdPresent = isHmdPresent,
                    IsRuntimeInstalled = Runtime.IsSteamVRInstalled(),
                    IsRuntimeRunning = Runtime.IsSteamVRRunning()
            };
        }
    }
}
