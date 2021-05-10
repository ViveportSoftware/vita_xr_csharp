using System;
using Htc.Vita.Core.Log;
using Htc.Vita.Core.Util;

namespace Htc.Vita.XR
{
    /// <summary>
    /// Class OpenVRManager.
    /// </summary>
    public abstract partial class OpenVRManager
    {
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
        /// Called when checking the status of this instance.
        /// </summary>
        /// <returns>CheckResult.</returns>
        protected abstract CheckResult OnCheck();
    }
}
