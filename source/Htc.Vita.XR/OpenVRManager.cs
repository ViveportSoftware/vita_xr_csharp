using System;
using System.Threading.Tasks;
using Htc.Vita.Core.Log;
using Htc.Vita.Core.Util;

namespace Htc.Vita.XR
{
    /// <summary>
    /// Class OpenVRManager.
    /// </summary>
    public abstract partial class OpenVRManager
    {
        /// <summary>
        /// Occurs when enable-home-application settings have changed.
        /// </summary>
        public event Action<bool> OnEnableHomeAppSettingsHaveChanged;
        /// <summary>
        /// Occurs when scene application state changed.
        /// </summary>
        public event Action<SceneApplicationState> OnSceneApplicationStateChanged;

        static OpenVRManager()
        {
            TypeRegistry.RegisterDefault<OpenVRManager, DefaultOpenVRManager>();
        }

        /// <summary>
        /// Registers the instance type.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static void Register<T>()
                where T : OpenVRManager, new()
        {
            TypeRegistry.Register<OpenVRManager, T>();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <returns>OpenVRManager.</returns>
        public static OpenVRManager GetInstance()
        {
            return TypeRegistry.GetInstance<OpenVRManager>();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>OpenVRManager.</returns>
        public static OpenVRManager GetInstance<T>()
                where T : OpenVRManager, new()
        {
            return TypeRegistry.GetInstance<OpenVRManager, T>();
        }

        /// <summary>
        /// Checks the status of this instance.
        /// </summary>
        /// <returns>CheckResult.</returns>
        public CheckResult Check()
        {
            CheckResult result = null;
            try
            {
                result = OnCheck();
            }
            catch (Exception e)
            {
                Logger.GetInstance(typeof(OpenVRManager)).Error(e.ToString());
            }
            return result ?? new CheckResult();
        }

        /// <summary>
        /// Connects the runtime.
        /// </summary>
        /// <returns><c>true</c> if connecting the runtime successfully, <c>false</c> otherwise.</returns>
        public bool ConnectRuntime()
        {
            var result = false;
            try
            {
                result = OnConnectRuntime();
            }
            catch (Exception e)
            {
                Logger.GetInstance(typeof(OpenVRManager)).Error(e.ToString());
            }
            return result;
        }

        /// <summary>
        /// Disconnects the runtime.
        /// </summary>
        /// <returns><c>true</c> if disconnecting the runtime successfully, <c>false</c> otherwise.</returns>
        public bool DisconnectRuntime()
        {
            var result = false;
            try
            {
                result = OnDisconnectRuntime();
            }
            catch (Exception e)
            {
                Logger.GetInstance(typeof(OpenVRManager)).Error(e.ToString());
            }
            return result;
        }

        /// <summary>
        /// Enables the home application.
        /// </summary>
        /// <param name="homeAppEnabled">if set to <c>true</c> the home application should be enabled.</param>
        /// <returns><c>true</c> if enabling the home application successfully, <c>false</c> otherwise.</returns>
        public bool EnableHomeApp(bool homeAppEnabled)
        {
            var result = false;
            try
            {
                result = OnEnableHomeApp(homeAppEnabled);
            }
            catch (Exception e)
            {
                Logger.GetInstance(typeof(OpenVRManager)).Error(e.ToString());
            }
            return result;
        }

        /// <summary>
        /// Gets the application process identifier.
        /// </summary>
        /// <param name="appKey">The application key.</param>
        /// <returns>System.UInt32.</returns>
        public uint GetApplicationProcessId(string appKey)
        {
            if (string.IsNullOrWhiteSpace(appKey))
            {
                return 0U;
            }

            var result = 0U;
            try
            {
                result = OnGetApplicationProcessId(appKey);
            }
            catch (Exception e)
            {
                Logger.GetInstance(typeof(OpenVRManager)).Error(e.ToString());
            }
            return result;
        }

        /// <summary>
        /// Gets the scene application state.
        /// </summary>
        /// <returns>SceneApplicationState.</returns>
        public SceneApplicationState GetSceneApplicationState()
        {
            var result = SceneApplicationState.None;
            try
            {
                result = OnGetSceneApplicationState();
            }
            catch (Exception e)
            {
                Logger.GetInstance(typeof(OpenVRManager)).Error(e.ToString());
            }
            return result;
        }

        /// <summary>
        /// Determines whether the home application is enabled.
        /// </summary>
        /// <returns><c>true</c> if the home application is enabled; otherwise, <c>false</c>.</returns>
        public bool IsHomeAppEnabled()
        {
            var result = false;
            try
            {
                result = OnIsHomeAppEnabled();
            }
            catch (Exception e)
            {
                Logger.GetInstance(typeof(OpenVRManager)).Error(e.ToString());
            }
            return result;
        }

        /// <summary>
        /// Determines whether the runtime is connected.
        /// </summary>
        /// <returns><c>true</c> if the runtime is connected; otherwise, <c>false</c>.</returns>
        public bool IsRuntimeConnected()
        {
            var result = false;
            try
            {
                result = OnIsRuntimeConnected();
            }
            catch (Exception e)
            {
                Logger.GetInstance(typeof(OpenVRManager)).Error(e.ToString());
            }
            return result;
        }

        /// <summary>
        /// Launches the application.
        /// </summary>
        /// <param name="appKey">The application key.</param>
        /// <returns>ApplicationError.</returns>
        public ApplicationError LaunchApplication(string appKey)
        {
            if (string.IsNullOrWhiteSpace(appKey))
            {
                return ApplicationError.InvalidApplication;
            }

            var result = ApplicationError.None;
            try
            {
                result = OnLaunchApplication(appKey);
            }
            catch (Exception e)
            {
                Logger.GetInstance(typeof(OpenVRManager)).Error(e.ToString());
            }
            return result;
        }

        /// <summary>
        /// Launches the runtime.
        /// </summary>
        /// <returns><c>true</c> if launching the runtime successfully, <c>false</c> otherwise.</returns>
        public bool LaunchRuntime()
        {
            var result = false;
            try
            {
                result = OnLaunchRuntime();
            }
            catch (Exception e)
            {
                Logger.GetInstance(typeof(OpenVRManager)).Error(e.ToString());
            }
            return result;
        }

        /// <summary>
        /// Notifies the enable home application settings have changed.
        /// </summary>
        /// <param name="homeAppEnabled">if set to <c>true</c> [home application enabled].</param>
        protected void NotifyEnableHomeAppSettingsHaveChanged(bool homeAppEnabled)
        {
            Task.Run(() =>
            {
                    try
                    {
                        OnEnableHomeAppSettingsHaveChanged?.Invoke(homeAppEnabled);
                    }
                    catch (Exception e)
                    {
                        Logger.GetInstance(typeof(OpenVRManager)).Error(e.ToString());
                    }
            });
        }

        /// <summary>
        /// Notifies the scene application state changed.
        /// </summary>
        /// <param name="sceneApplicationState">State of the scene application.</param>
        protected void NotifySceneApplicationStateChanged(SceneApplicationState sceneApplicationState)
        {
            Task.Run(() =>
            {
                    try
                    {
                        OnSceneApplicationStateChanged?.Invoke(sceneApplicationState);
                    }
                    catch (Exception e)
                    {
                        Logger.GetInstance(typeof(OpenVRManager)).Error(e.ToString());
                    }
            });
        }

        /// <summary>
        /// Called when checking the status of this instance.
        /// </summary>
        /// <returns>CheckResult.</returns>
        protected abstract CheckResult OnCheck();
        /// <summary>
        /// Called when connecting the runtime.
        /// </summary>
        /// <returns><c>true</c> if connecting the runtime successfully, <c>false</c> otherwise.</returns>
        protected abstract bool OnConnectRuntime();
        /// <summary>
        /// Called when disconnecting the runtime.
        /// </summary>
        /// <returns><c>true</c> if disconnecting the runtime successfully, <c>false</c> otherwise.</returns>
        protected abstract bool OnDisconnectRuntime();
        /// <summary>
        /// Called when enabling the home application.
        /// </summary>
        /// <param name="homeAppEnabled">if set to <c>true</c> the home application should be enabled.</param>
        /// <returns><c>true</c> if enabling the home application successfully, <c>false</c> otherwise.</returns>
        protected abstract bool OnEnableHomeApp(bool homeAppEnabled);
        /// <summary>
        /// Called when getting application process identifier.
        /// </summary>
        /// <param name="appKey">The application key.</param>
        /// <returns>System.UInt32.</returns>
        protected abstract uint OnGetApplicationProcessId(string appKey);
        /// <summary>
        /// Called when getting scene application state.
        /// </summary>
        /// <returns>SceneApplicationState.</returns>
        protected abstract SceneApplicationState OnGetSceneApplicationState();
        /// <summary>
        /// Called when checking if the home application is enabled.
        /// </summary>
        /// <returns><c>true</c> if the home application is enabled, <c>false</c> otherwise.</returns>
        protected abstract bool OnIsHomeAppEnabled();
        /// <summary>
        /// Called when checking if the runtime is connected.
        /// </summary>
        /// <returns><c>true</c> if the runtime is connected, <c>false</c> otherwise.</returns>
        protected abstract bool OnIsRuntimeConnected();
        /// <summary>
        /// Called when launching the application.
        /// </summary>
        /// <param name="appKey">The application key.</param>
        /// <returns>ApplicationError.</returns>
        protected abstract ApplicationError OnLaunchApplication(string appKey);
        /// <summary>
        /// Called when launching the runtime.
        /// </summary>
        /// <returns><c>true</c> if launching the runtime successfully, <c>false</c> otherwise.</returns>
        protected abstract bool OnLaunchRuntime();
    }
}
