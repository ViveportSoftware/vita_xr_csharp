namespace Htc.Vita.XR
{
    public partial class OpenVRManager
    {
        /// <summary>
        /// Class CheckResult.
        /// </summary>
        public class CheckResult
        {
            /// <summary>
            /// Gets or sets a value indicating whether the API is ready.
            /// </summary>
            /// <value><c>true</c> if the API is ready; otherwise, <c>false</c>.</value>
            public bool IsApiReady { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether the HMD is present.
            /// </summary>
            /// <value><c>true</c> if the HMD is present; otherwise, <c>false</c>.</value>
            public bool IsHmdPresent { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether the runtime is installed.
            /// </summary>
            /// <value><c>true</c> if the runtime is installed; otherwise, <c>false</c>.</value>
            public bool IsRuntimeInstalled { get; set; }
            /// <summary>
            /// Gets or sets a value indicating whether the runtime is running.
            /// </summary>
            /// <value><c>true</c> if the runtime is running; otherwise, <c>false</c>.</value>
            public bool IsRuntimeRunning { get; set; }
        }
    }
}
