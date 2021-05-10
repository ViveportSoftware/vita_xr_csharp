using System.Runtime.InteropServices;

namespace Htc.Vita.XR
{
    public partial class DefaultOpenVRManager
    {
        internal static class Interop
        {
            [DllImport(Library.OpenVRApi,
                    CallingConvention = CallingConvention.Cdecl,
                    EntryPoint = "VR_IsHmdPresent",
                    ExactSpelling = true,
                    SetLastError = true)]
            internal static extern bool VRIsHmdPresent();
        }

        internal static class Library
        {
            internal const string OpenVRApi = "openvr_api";
        }
    }
}
