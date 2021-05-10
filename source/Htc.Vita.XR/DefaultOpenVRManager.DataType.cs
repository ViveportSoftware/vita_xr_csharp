using System.IO;

namespace Htc.Vita.XR
{
    public partial class DefaultOpenVRManager
    {
        /// <summary>
        /// Class SteamVRInfo.
        /// </summary>
        internal class SteamVRInfo
        {
            /// <summary>
            /// Gets or sets the install path.
            /// </summary>
            /// <value>The install path.</value>
            public DirectoryInfo InstallPath { get; set; }
            /// <summary>
            /// Gets or sets the install version.
            /// </summary>
            /// <value>The install version.</value>
            public string InstallVersion { get; set; }
            /// <summary>
            /// Gets or sets the VRCmd (32-bit) path.
            /// </summary>
            /// <value>The VRCmd (32-bit) path.</value>
            public FileInfo VRCmd32Path { get; set; }
            /// <summary>
            /// Gets or sets the VRCmd (64-bit) path.
            /// </summary>
            /// <value>The VRCmd (64-bit) path.</value>
            public FileInfo VRCmd64Path { get; set; }
            /// <summary>
            /// Gets or sets the VRMonitor (32-bit) path.
            /// </summary>
            /// <value>The VRMonitor (32-bit) path.</value>
            public FileInfo VRMonitor32Path { get; set; }
            /// <summary>
            /// Gets or sets the VRMonitor (64-bit) path.
            /// </summary>
            /// <value>The VRMonitor (64-bit) path.</value>
            public FileInfo VRMonitor64Path { get; set; }
            /// <summary>
            /// Gets or sets the VRPathReg (32-bit) path.
            /// </summary>
            /// <value>The VRPathReg (32-bit) path.</value>
            public FileInfo VRPathReg32Path { get; set; }
            /// <summary>
            /// Gets or sets the VRPathReg (64-bit) path.
            /// </summary>
            /// <value>The VRPathReg (64-bit) path.</value>
            public FileInfo VRPathReg64Path { get; set; }
            /// <summary>
            /// Gets or sets the VRServer (32-bit) path.
            /// </summary>
            /// <value>The VRServer (32-bit) path.</value>
            public FileInfo VRServer32Path { get; set; }
            /// <summary>
            /// Gets or sets the VRServer (64-bit) path.
            /// </summary>
            /// <value>The VRServer (64-bit) path.</value>
            public FileInfo VRServer64Path { get; set; }
            /// <summary>
            /// Gets or sets the VRStartUp (32-bit) path.
            /// </summary>
            /// <value>The VRStartUp (32-bit) path.</value>
            public FileInfo VRStartUp32Path { get; set; }
            /// <summary>
            /// Gets or sets the VRStartUp (64-bit) path.
            /// </summary>
            /// <value>The VRStartUp (64-bit) path.</value>
            public FileInfo VRStartUp64Path { get; set; }
        }
    }
}
