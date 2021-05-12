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
        /// Enum InitError
        /// </summary>
        public enum InitError
        {
            /// <summary>
            /// None
            /// </summary>
            None = 0,
            /// <summary>
            /// Unknown
            /// </summary>
            Unknown = 1,
            Init_InstallationNotFound = 100,
            Init_InstallationCorrupt = 101,
            Init_VRClientDLLNotFound = 102,
            Init_FileNotFound = 103,
            Init_FactoryNotFound = 104,
            Init_InterfaceNotFound = 105,
            Init_InvalidInterface = 106,
            Init_UserConfigDirectoryInvalid = 107,
            Init_HmdNotFound = 108,
            Init_NotInitialized = 109,
            Init_PathRegistryNotFound = 110,
            Init_NoConfigPath = 111,
            Init_NoLogPath = 112,
            Init_PathRegistryNotWritable = 113,
            Init_AppInfoInitFailed = 114,
            Init_Retry = 115,
            Init_InitCanceledByUser = 116,
            Init_AnotherAppLaunching = 117,
            Init_SettingsInitFailed = 118,
            Init_ShuttingDown = 119,
            Init_TooManyObjects = 120,
            Init_NoServerForBackgroundApp = 121,
            Init_NotSupportedWithCompositor = 122,
            Init_NotAvailableToUtilityApps = 123,
            Init_Internal = 124,
            Init_HmdDriverIdIsNone = 125,
            Init_HmdNotFoundPresenceFailed = 126,
            Init_VRMonitorNotFound = 127,
            Init_VRMonitorStartupFailed = 128,
            Init_LowPowerWatchdogNotSupported = 129,
            Init_InvalidApplicationType = 130,
            Init_NotAvailableToWatchdogApps = 131,
            Init_WatchdogDisabledInSettings = 132,
            Init_VRDashboardNotFound = 133,
            Init_VRDashboardStartupFailed = 134,
            Init_VRHomeNotFound = 135,
            Init_VRHomeStartupFailed = 136,
            Init_RebootingBusy = 137,
            Init_FirmwareUpdateBusy = 138,
            Init_FirmwareRecoveryBusy = 139,
            Init_USBServiceBusy = 140,
            Init_VRWebHelperStartupFailed = 141,
            Init_TrackerManagerInitFailed = 142,
            Init_AlreadyRunning = 143,
            Init_FailedForVrMonitor = 144,
            Init_PropertyManagerInitFailed = 145,
            Init_WebServerFailed = 146,
            Init_IllegalTypeTransition = 147,
            Init_MismatchedRuntimes = 148,
            Init_InvalidProcessId = 149,
            Init_VRServiceStartupFailed = 150,
            Init_PrismNeedsNewDrivers = 151,
            Init_PrismStartupTimedOut = 152,
            Init_CouldNotStartPrism = 153,
            Init_CreateDriverDirectDeviceFailed = 154,
            Init_PrismExitedUnexpectedly = 155,
            Driver_Failed = 200,
            Driver_Unknown = 201,
            Driver_HmdUnknown = 202,
            Driver_NotLoaded = 203,
            Driver_RuntimeOutOfDate = 204,
            Driver_HmdInUse = 205,
            Driver_NotCalibrated = 206,
            Driver_CalibrationInvalid = 207,
            Driver_HmdDisplayNotFound = 208,
            Driver_TrackedDeviceInterfaceUnknown = 209,
            Driver_HmdDriverIdOutOfBounds = 211,
            Driver_HmdDisplayMirrored = 212,
            Driver_HmdDisplayNotFoundLaptop = 213,
            IPC_ServerInitFailed = 300,
            IPC_ConnectFailed = 301,
            IPC_SharedStateInitFailed = 302,
            IPC_CompositorInitFailed = 303,
            IPC_MutexInitFailed = 304,
            IPC_Failed = 305,
            IPC_CompositorConnectFailed = 306,
            IPC_CompositorInvalidConnectResponse = 307,
            IPC_ConnectFailedAfterMultipleAttempts = 308,
            IPC_ConnectFailedAfterTargetExited = 309,
            IPC_NamespaceUnavailable = 310,
            Compositor_Failed = 400,
            Compositor_D3D11HardwareRequired = 401,
            Compositor_FirmwareRequiresUpdate = 402,
            Compositor_OverlayInitFailed = 403,
            Compositor_ScreenshotsInitFailed = 404,
            Compositor_UnableToCreateDevice = 405,
            Compositor_SharedStateIsNull = 406,
            Compositor_NotificationManagerIsNull = 407,
            Compositor_ResourceManagerClientIsNull = 408,
            Compositor_MessageOverlaySharedStateInitFailure = 409,
            Compositor_PropertiesInterfaceIsNull = 410,
            Compositor_CreateFullscreenWindowFailed = 411,
            Compositor_SettingsInterfaceIsNull = 412,
            Compositor_FailedToShowWindow = 413,
            Compositor_DistortInterfaceIsNull = 414,
            Compositor_DisplayFrequencyFailure = 415,
            Compositor_RendererInitializationFailed = 416,
            Compositor_DXGIFactoryInterfaceIsNull = 417,
            Compositor_DXGIFactoryCreateFailed = 418,
            Compositor_DXGIFactoryQueryFailed = 419,
            Compositor_InvalidAdapterDesktop = 420,
            Compositor_InvalidHmdAttachment = 421,
            Compositor_InvalidOutputDesktop = 422,
            Compositor_InvalidDeviceProvided = 423,
            Compositor_D3D11RendererInitializationFailed = 424,
            Compositor_FailedToFindDisplayMode = 425,
            Compositor_FailedToCreateSwapChain = 426,
            Compositor_FailedToGetBackBuffer = 427,
            Compositor_FailedToCreateRenderTarget = 428,
            Compositor_FailedToCreateDXGI2SwapChain = 429,
            Compositor_FailedtoGetDXGI2BackBuffer = 430,
            Compositor_FailedToCreateDXGI2RenderTarget = 431,
            Compositor_FailedToGetDXGIDeviceInterface = 432,
            Compositor_SelectDisplayMode = 433,
            Compositor_FailedToCreateNvAPIRenderTargets = 434,
            Compositor_NvAPISetDisplayMode = 435,
            Compositor_FailedToCreateDirectModeDisplay = 436,
            Compositor_InvalidHmdPropertyContainer = 437,
            Compositor_UpdateDisplayFrequency = 438,
            Compositor_CreateRasterizerState = 439,
            Compositor_CreateWireframeRasterizerState = 440,
            Compositor_CreateSamplerState = 441,
            Compositor_CreateClampToBorderSamplerState = 442,
            Compositor_CreateAnisoSamplerState = 443,
            Compositor_CreateOverlaySamplerState = 444,
            Compositor_CreatePanoramaSamplerState = 445,
            Compositor_CreateFontSamplerState = 446,
            Compositor_CreateNoBlendState = 447,
            Compositor_CreateBlendState = 448,
            Compositor_CreateAlphaBlendState = 449,
            Compositor_CreateBlendStateMaskR = 450,
            Compositor_CreateBlendStateMaskG = 451,
            Compositor_CreateBlendStateMaskB = 452,
            Compositor_CreateDepthStencilState = 453,
            Compositor_CreateDepthStencilStateNoWrite = 454,
            Compositor_CreateDepthStencilStateNoDepth = 455,
            Compositor_CreateFlushTexture = 456,
            Compositor_CreateDistortionSurfaces = 457,
            Compositor_CreateConstantBuffer = 458,
            Compositor_CreateHmdPoseConstantBuffer = 459,
            Compositor_CreateHmdPoseStagingConstantBuffer = 460,
            Compositor_CreateSharedFrameInfoConstantBuffer = 461,
            Compositor_CreateOverlayConstantBuffer = 462,
            Compositor_CreateSceneTextureIndexConstantBuffer = 463,
            Compositor_CreateReadableSceneTextureIndexConstantBuffer = 464,
            Compositor_CreateLayerGraphicsTextureIndexConstantBuffer = 465,
            Compositor_CreateLayerComputeTextureIndexConstantBuffer = 466,
            Compositor_CreateLayerComputeSceneTextureIndexConstantBuffer = 467,
            Compositor_CreateComputeHmdPoseConstantBuffer = 468,
            Compositor_CreateGeomConstantBuffer = 469,
            Compositor_CreatePanelMaskConstantBuffer = 470,
            Compositor_CreatePixelSimUBO = 471,
            Compositor_CreateMSAARenderTextures = 472,
            Compositor_CreateResolveRenderTextures = 473,
            Compositor_CreateComputeResolveRenderTextures = 474,
            Compositor_CreateDriverDirectModeResolveTextures = 475,
            Compositor_OpenDriverDirectModeResolveTextures = 476,
            Compositor_CreateFallbackSyncTexture = 477,
            Compositor_ShareFallbackSyncTexture = 478,
            Compositor_CreateOverlayIndexBuffer = 479,
            Compositor_CreateOverlayVertexBuffer = 480,
            Compositor_CreateTextVertexBuffer = 481,
            Compositor_CreateTextIndexBuffer = 482,
            Compositor_CreateMirrorTextures = 483,
            Compositor_CreateLastFrameRenderTexture = 484,
            Compositor_CreateMirrorOverlay = 485,
            Compositor_FailedToCreateVirtualDisplayBackbuffer = 486,
            Compositor_DisplayModeNotSupported = 487,
            Compositor_CreateOverlayInvalidCall = 488,
            Compositor_CreateOverlayAlreadyInitialized = 489,
            Compositor_FailedToCreateMailbox = 490,
            Compositor_WindowInterfaceIsNull = 491,
            Compositor_SystemLayerCreateInstance = 492,
            Compositor_SystemLayerCreateSession = 493,
            VendorSpecific_UnableToConnectToOculusRuntime = 1000,
            VendorSpecific_WindowsNotInDevMode = 1001,
            VendorSpecific_HmdFound_CantOpenDevice = 1101,
            VendorSpecific_HmdFound_UnableToRequestConfigStart = 1102,
            VendorSpecific_HmdFound_NoStoredConfig = 1103,
            VendorSpecific_HmdFound_ConfigTooBig = 1104,
            VendorSpecific_HmdFound_ConfigTooSmall = 1105,
            VendorSpecific_HmdFound_UnableToInitZLib = 1106,
            VendorSpecific_HmdFound_CantReadFirmwareVersion = 1107,
            VendorSpecific_HmdFound_UnableToSendUserDataStart = 1108,
            VendorSpecific_HmdFound_UnableToGetUserDataStart = 1109,
            VendorSpecific_HmdFound_UnableToGetUserDataNext = 1110,
            VendorSpecific_HmdFound_UserDataAddressRange = 1111,
            VendorSpecific_HmdFound_UserDataError = 1112,
            VendorSpecific_HmdFound_ConfigFailedSanityCheck = 1113,
            VendorSpecific_OculusRuntimeBadInstall = 1114,
            Steam_SteamInstallationNotFound = 2000,
            LastError = 2001,
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
