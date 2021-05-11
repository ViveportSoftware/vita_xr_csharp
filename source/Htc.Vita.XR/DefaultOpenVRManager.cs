using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Htc.Vita.Core.Log;
using Htc.Vita.Mod.Valve.VR;

namespace Htc.Vita.XR
{
    /// <summary>
    /// Class DefaultOpenVRManager.
    /// Implements the <see cref="OpenVRManager" />
    /// </summary>
    /// <seealso cref="OpenVRManager" />
    public partial class DefaultOpenVRManager : OpenVRManager
    {
        private readonly object _runtimeConnectingLock = new object();

        private volatile bool _isRuntimeConnected;

        static DefaultOpenVRManager()
        {
#if NATIVE_LIBRARY_EMBEDDED
            Internal.PrepareLibrary();
#else
            var is64 = IntPtr.Size == 8;
            Core.Runtime.Platform.LoadNativeLib(is64 ? $"x64/{Library.OpenVRApi}.dll" : $"x86/{Library.OpenVRApi}.dll");
#endif
        }

        /// <inheritdoc />
        protected override CheckResult OnCheck()
        {
            var isApiReady = false;
            var isHmdPresent = false;
            try
            {
                isHmdPresent = OpenVRInterop.IsHmdPresent();
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

        /// <inheritdoc />
        protected override bool OnConnectRuntime()
        {
            if (OnIsRuntimeConnected())
            {
                OnDisconnectRuntime();
            }

            var isSteamVRRunning = Runtime.IsSteamVRRunning();
            if (!isSteamVRRunning)
            {
                lock (_runtimeConnectingLock)
                {
                    _isRuntimeConnected = false;
                }
                return OnIsRuntimeConnected();
            }

            lock (_runtimeConnectingLock)
            {
                var evrInitError = EVRInitError.None;
                OpenVR.Init(ref evrInitError, EVRApplicationType.VRApplication_Utility);
                if (evrInitError != EVRInitError.None)
                {
                    Logger.GetInstance(typeof(DefaultOpenVRManager)).Error($"Can not connect runtime: {evrInitError}");
                }
                else
                {
                    _isRuntimeConnected = true;
                }
            }

            Task.Run(() =>
            {
                    var lastHomeAppEnabled = false;
                    var lastSceneApplicationState = SceneApplicationState.None;

                    var vrEvent = new VREvent_t();
                    while (OnIsRuntimeConnected())
                    {
                        var hasEvent = false;
                        lock (_runtimeConnectingLock)
                        {
                            if (OnIsRuntimeConnected())
                            {
                                hasEvent = OpenVR.System.PollNextEvent(ref vrEvent, (uint)Marshal.SizeOf(vrEvent));
                            }
                        }
                        if (!hasEvent)
                        {
                            if (!Runtime.IsSteamVRRunning())
                            {
                                Logger.GetInstance(typeof(DefaultOpenVRManager)).Error("SteamVR has exited");
                                vrEvent.eventType = (uint)EVREventType.VREvent_Quit;
                            }
                            else
                            {
                                SpinWait.SpinUntil(() => false, TimeSpan.FromMilliseconds(200));
                                continue;
                            }
                        }

                        var eventType = (EVREventType)vrEvent.eventType;
                        if (eventType == EVREventType.VREvent_Quit)
                        {
                            OpenVR.System.AcknowledgeQuit_Exiting();
                            Logger.GetInstance(typeof(DefaultOpenVRManager)).Warn($"{eventType} received. Try to disconnect runtime.");
                            OnDisconnectRuntime();
                        }
                        else if (eventType == EVREventType.VREvent_SceneApplicationStateChanged)
                        {
                            var sceneApplicationState = OnGetSceneApplicationState();
                            if (lastSceneApplicationState != sceneApplicationState)
                            {
                                lastSceneApplicationState = sceneApplicationState;
                                NotifySceneApplicationStateChanged(sceneApplicationState);
                            }
                        }
                        else if (eventType == EVREventType.VREvent_EnableHomeAppSettingsHaveChanged)
                        {
                            var homeAppEnabled = OnIsHomeAppEnabled();
                            if (lastHomeAppEnabled != homeAppEnabled)
                            {
                                lastHomeAppEnabled = homeAppEnabled;
                                NotifyEnableHomeAppSettingsHaveChanged(homeAppEnabled);
                            }
                        }
                        else
                        {
                            // Logger.GetInstance(typeof(DefaultOpenVRManager)).Debug($"{eventType} is not handled");
                        }

                        vrEvent = new VREvent_t();
                    }
            });

            return OnIsRuntimeConnected();
        }

        /// <inheritdoc />
        protected override bool OnDisconnectRuntime()
        {
            lock (_runtimeConnectingLock)
            {
                OpenVR.Shutdown();
                _isRuntimeConnected = false;
            }

            return !OnIsRuntimeConnected();
        }

        /// <inheritdoc />
        protected override bool OnEnableHomeApp(bool homeAppEnabled)
        {
            lock (_runtimeConnectingLock)
            {
                if (OnIsRuntimeConnected())
                {
                    var evrSettingsError = EVRSettingsError.None;
                    OpenVR.Settings.SetBool(
                            OpenVR.k_pch_SteamVR_Section,
                            OpenVR.k_pch_SteamVR_EnableHomeApp,
                            homeAppEnabled,
                            ref evrSettingsError
                    );
                    if (evrSettingsError == EVRSettingsError.None)
                    {
                        return true;
                    }

                    Logger.GetInstance(typeof(DefaultOpenVRManager)).Error($"Can not enable home app: {evrSettingsError}");
                    return false;
                }

                Logger.GetInstance(typeof(DefaultOpenVRManager)).Warn("Runtime is not connected when enabling home app.");
                return false;
            }
        }

        /// <inheritdoc />
        protected override uint OnGetApplicationProcessId(string appKey)
        {
            lock (_runtimeConnectingLock)
            {
                if (OnIsRuntimeConnected())
                {
                    return OpenVR.Applications.GetApplicationProcessId(appKey);
                }

                return 0U;
            }
        }

        /// <inheritdoc />
        protected override SceneApplicationState OnGetSceneApplicationState()
        {
            lock (_runtimeConnectingLock)
            {
                if (OnIsRuntimeConnected())
                {
                    return (SceneApplicationState) OpenVR.Applications.GetSceneApplicationState();
                }

                return SceneApplicationState.None;
            }
        }

        /// <inheritdoc />
        protected override bool OnIsHomeAppEnabled()
        {
            lock (_runtimeConnectingLock)
            {
                if (OnIsRuntimeConnected())
                {
                    var evrSettingsError = EVRSettingsError.None;
                    var homeAppEnabled = OpenVR.Settings.GetBool(
                            OpenVR.k_pch_SteamVR_Section,
                            OpenVR.k_pch_SteamVR_EnableHomeApp,
                            ref evrSettingsError
                    );
                    if (evrSettingsError == EVRSettingsError.None)
                    {
                        return homeAppEnabled;
                    }

                    Logger.GetInstance(typeof(DefaultOpenVRManager)).Error($"Can not check if home app is enabled: {evrSettingsError}");
                    return true;
                }

                Logger.GetInstance(typeof(DefaultOpenVRManager)).Warn("Runtime is not connected when checking if home app is enabled.");
                return true;
            }
        }

        /// <inheritdoc />
        protected override bool OnIsRuntimeConnected()
        {
            lock (_runtimeConnectingLock)
            {
                return _isRuntimeConnected;
            }
        }

        /// <inheritdoc />
        protected override ApplicationError OnLaunchApplication(string appKey)
        {
            lock (_runtimeConnectingLock)
            {
                if (OnIsRuntimeConnected())
                {
                    return (ApplicationError) OpenVR.Applications.LaunchApplication(appKey);
                }

                Logger.GetInstance(typeof(DefaultOpenVRManager)).Warn($"Runtime is not connected when launching application {appKey}.");
                return ApplicationError.IPCFailed;
            }
        }
    }
}
