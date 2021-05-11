namespace Htc.Vita.XR
{
    public partial class OpenVRManager
    {
        /// <summary>
        /// Enum ApplicationError
        /// </summary>
        public enum ApplicationError
        {
            /// <summary>
            /// None
            /// </summary>
            None = 0,
            /// <summary>
            /// Application key already exists
            /// </summary>
            AppKeyAlreadyExists = 100,
            /// <summary>
            /// No manifest
            /// </summary>
            NoManifest = 101,
            /// <summary>
            /// No application
            /// </summary>
            NoApplication = 102,
            /// <summary>
            /// Invalid index
            /// </summary>
            InvalidIndex = 103,
            /// <summary>
            /// Unknown application
            /// </summary>
            UnknownApplication = 104,
            /// <summary>
            /// UPC failed
            /// </summary>
            IPCFailed = 105,
            /// <summary>
            /// Application is already running
            /// </summary>
            ApplicationAlreadyRunning = 106,
            /// <summary>
            /// Invalid manifest
            /// </summary>
            InvalidManifest = 107,
            /// <summary>
            /// Invalid application
            /// </summary>
            InvalidApplication = 108,
            /// <summary>
            /// Launch failed
            /// </summary>
            LaunchFailed = 109,
            /// <summary>
            /// Application is already starting
            /// </summary>
            ApplicationAlreadyStarting = 110,
            /// <summary>
            /// Launch in progress
            /// </summary>
            LaunchInProgress = 111,
            /// <summary>
            /// Old application is quitting
            /// </summary>
            OldApplicationQuitting = 112,
            /// <summary>
            /// The transition is aborted
            /// </summary>
            TransitionAborted = 113,
            /// <summary>
            /// Application is a template
            /// </summary>
            IsTemplate = 114,
            /// <summary>
            /// SteamVR is exiting
            /// </summary>
            SteamVRIsExiting = 115,
            /// <summary>
            /// The buffer is too small
            /// </summary>
            BufferTooSmall = 200,
            /// <summary>
            /// The property is not set
            /// </summary>
            PropertyNotSet = 201,
            /// <summary>
            /// Unknown property
            /// </summary>
            UnknownProperty = 202,
            /// <summary>
            /// Invalid parameter
            /// </summary>
            InvalidParameter = 203,
            /// <summary>
            /// Not implemented
            /// </summary>
            NotImplemented = 300,
        }

        /// <summary>
        /// Enum SceneApplicationState
        /// </summary>
        public enum SceneApplicationState
        {
            /// <summary>
            /// The scene application is not enabled.
            /// </summary>
            None = 0,
            /// <summary>
            /// OpenVR runtime is starting the scene application.
            /// </summary>
            Starting = 1,
            /// <summary>
            /// OpenVR runtime is quitting the scene application.
            /// </summary>
            Quitting = 2,
            /// <summary>
            /// The scene application is running.
            /// </summary>
            Running = 3,
            /// <summary>
            /// OpenVR runtime is waiting the scene application.
            /// </summary>
            Waiting = 4,
        }
    }
}
