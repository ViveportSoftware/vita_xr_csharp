namespace Htc.Vita.XR
{
    public partial class OpenVRManager
    {
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
