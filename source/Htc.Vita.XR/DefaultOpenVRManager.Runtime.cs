using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using Htc.Vita.Core.Json;
using Htc.Vita.Core.Log;
using Htc.Vita.Core.Runtime;

namespace Htc.Vita.XR
{
    public partial class DefaultOpenVRManager
    {
        internal static class Runtime
        {
            internal const string VRMonitorFileName = "vrmonitor.exe";
            internal const string VRPathRegFileName = "vrpathreg.exe";
            internal const string VRServerFileName = "vrserver.exe";
            internal const string VRStartUpFileName = "vrstartup.exe";

            private static readonly Dictionary<string, string> VersionCodeWithVersionName = new Dictionary<string, string>();

            static Runtime()
            {
                InitKnownVersion();
            }

            internal static FileInfo GetOpenVRPathPath()
            {
                var path = Environment.GetEnvironmentVariable("LocalAppData");
                if (!string.IsNullOrWhiteSpace(path))
                {
                    path = Path.Combine(path, "openvr", "openvrpaths.vrpath");
                }
                if (!string.IsNullOrWhiteSpace(path))
                {
                    return new FileInfo(path);
                }

                return null;
            }

            private static FileInfo GetSteamVRComponentPath(
                    DirectoryInfo runtimePath,
                    string relativePath)
            {
                if (runtimePath == null)
                {
                    return null;
                }

                var fileInfo = new FileInfo(Path.Combine(runtimePath.FullName, relativePath));
                if (!fileInfo.Exists)
                {
                    return null;
                }

                return fileInfo;
            }

            internal static List<SteamVRInfo> GetSteamVRList()
            {
                return GetSteamVRListFromOpenVRPath();
            }

            internal static List<SteamVRInfo> GetSteamVRListFromOpenVRPath()
            {
                var result = new List<SteamVRInfo>();

                var openVRPathPath = GetOpenVRPathPath();
                if (openVRPathPath == null || !openVRPathPath.Exists)
                {
                    return result;
                }

                try
                {
                    using (var streamReader = File.OpenText(openVRPathPath.FullName))
                    {
                        var data = streamReader.ReadToEnd();
                        var jsonObject = JsonFactory.GetInstance().GetJsonObject(data);
                        var runtime = jsonObject.ParseJsonArray("runtime");
                        var size = runtime.Size();
                        for (var i = 0; i < size; i++)
                        {
                            var runtimePathString = runtime.ParseString(i);
                            if (string.IsNullOrWhiteSpace(runtimePathString))
                            {
                                continue;
                            }

                            var runtimePath = new DirectoryInfo(runtimePathString);
                            if (!runtimePath.Exists)
                            {
                                continue;
                            }

                            var steamVrInfo = new SteamVRInfo
                            {
                                    InstallPath = runtimePath,
                                    InstallVersion = ParseSteamVRVersion(GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "version.txt"))),
                                    VRMonitor32Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win32", VRMonitorFileName)),
                                    VRMonitor64Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win64", VRMonitorFileName)),
                                    VRPathReg32Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win32", VRPathRegFileName)),
                                    VRPathReg64Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win64", VRPathRegFileName)),
                                    VRServer32Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win32", VRServerFileName)),
                                    VRServer64Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win64", VRServerFileName)),
                                    VRStartUp32Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win32", VRStartUpFileName)),
                                    VRStartUp64Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win64", VRStartUpFileName))
                            };

                            if (steamVrInfo.VRMonitor32Path == null && steamVrInfo.VRMonitor64Path == null)
                            {
                                continue;
                            }

                            if (steamVrInfo.VRServer32Path == null && steamVrInfo.VRServer64Path == null)
                            {
                                continue;
                            }

                            result.Add(steamVrInfo);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.GetInstance(typeof(Runtime)).Error($"Can not parse SteamVR package version: {e.Message}");
                }

                return result;
            }

            private static void InitKnownVersion()
            {
                VersionCodeWithVersionName.Add("1615513459", "1.16.10");
            }

            private static bool IsOn64BitSystem()
            {
                return Platform.DetectOsArch() == Platform.OsArch.Bit64;
            }

            internal static bool IsSteamVRInstalled()
            {
                var steamVRList = GetSteamVRList();
                return steamVRList.Count >= 0;
            }

            internal static bool IsSteamVRRunning()
            {
                var steamVRList = GetSteamVRList();
                foreach (var steamVRInfo in steamVRList)
                {
                    if (IsVRMonitorRunning(steamVRInfo) && IsVRServerRunning(steamVRInfo))
                    {
                        return true;
                    }
                }
                return false;
            }

            internal static bool IsVRMonitorRunning(SteamVRInfo steamVRInfo)
            {
                if (steamVRInfo == null)
                {
                    return false;
                }

                var targetProcessName = steamVRInfo.VRMonitor32Path?.FullName;
                if (string.IsNullOrWhiteSpace(targetProcessName))
                {
                    targetProcessName = steamVRInfo.VRMonitor64Path?.FullName;
                }
                if (string.IsNullOrWhiteSpace(targetProcessName))
                {
                    return false;
                }

                targetProcessName = Path.GetFileNameWithoutExtension(targetProcessName);
                var processList = Process.GetProcessesByName(targetProcessName);
                if (processList.Length <= 0)
                {
                    return false;
                }

                foreach (var process in processList)
                {
                    string processPath = null;
                    try
                    {
                        processPath = process.MainModule?.FileName;
                    }
                    catch (Win32Exception)
                    {
                        processPath = ProcessManager.GetProcessPathById(process.Id);
                    }
                    catch (Exception e)
                    {
                        Logger.GetInstance(typeof(Runtime)).Error($"Can not get process path: {e.Message}");
                    }

                    if (processPath == null)
                    {
                        continue;
                    }

                    if (processPath.Equals(steamVRInfo.VRMonitor32Path?.FullName))
                    {
                        return true;
                    }

                    if (processPath.Equals(steamVRInfo.VRMonitor64Path?.FullName))
                    {
                        return true;
                    }
                }

                return false;
            }

            internal static bool IsVRServerRunning(SteamVRInfo steamVRInfo)
            {
                if (steamVRInfo == null)
                {
                    return false;
                }

                var targetProcessName = steamVRInfo.VRServer32Path?.FullName;
                if (string.IsNullOrWhiteSpace(targetProcessName))
                {
                    targetProcessName = steamVRInfo.VRServer64Path?.FullName;
                }
                if (string.IsNullOrWhiteSpace(targetProcessName))
                {
                    return false;
                }

                targetProcessName = Path.GetFileNameWithoutExtension(targetProcessName);
                var processList = Process.GetProcessesByName(targetProcessName);
                if (processList.Length <= 0)
                {
                    return false;
                }

                foreach (var process in processList)
                {
                    string processPath = null;
                    try
                    {
                        processPath = process.MainModule?.FileName;
                    }
                    catch (Win32Exception)
                    {
                        processPath = ProcessManager.GetProcessPathById(process.Id);
                    }
                    catch (Exception e)
                    {
                        Logger.GetInstance(typeof(Runtime)).Error($"Can not get process path: {e.Message}");
                    }

                    if (processPath == null)
                    {
                        continue;
                    }

                    if (processPath.Equals(steamVRInfo.VRServer32Path?.FullName))
                    {
                        return true;
                    }

                    if (processPath.Equals(steamVRInfo.VRServer64Path?.FullName))
                    {
                        return true;
                    }
                }

                return false;
            }

            internal static bool KillProcessesByFullPath(string processFullPath)
            {
                if (string.IsNullOrWhiteSpace(processFullPath))
                {
                    return true;
                }

                var processName = Path.GetFileNameWithoutExtension(processFullPath);
                var processes = Process.GetProcessesByName(processName);

                try
                {
                    foreach (var process in processes)
                    {
                        var processPath = ProcessManager.GetProcessPathById(process.Id);
                        if (string.IsNullOrWhiteSpace(processPath))
                        {
                            continue;
                        }
                        if (!processPath.Equals(processFullPath))
                        {
                            continue;
                        }

                        var success = process.CloseMainWindow();
                        if (!success)
                        {
                            process.Kill();
                        }
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Logger.GetInstance(typeof(Runtime)).Error($"Can not kill processes by \"{processFullPath}\": {e.Message}");
                }

                return false;
            }

            internal static bool KillSteamVR()
            {
                var steamVRList = GetSteamVRList();
                if (steamVRList.Count <= 0)
                {
                    return false;
                }

                foreach (var steamVRInfo in steamVRList)
                {
                    var success = KillSteamVR(steamVRInfo);
                    if (!success)
                    {
                        return false;
                    }
                }

                return true;
            }

            internal static bool KillSteamVR(SteamVRInfo steamVRInfo)
            {
                return KillSteamVRViaVRMonitor(steamVRInfo);
            }

            internal static bool KillSteamVRViaVRMonitor(SteamVRInfo steamVRInfo)
            {
                return KillSteamVRViaVRMonitor32(steamVRInfo) && KillSteamVRViaVRMonitor64(steamVRInfo);
            }

            internal static bool KillSteamVRViaVRMonitor32(SteamVRInfo steamVRInfo)
            {
                var vrMonitor32Path = steamVRInfo?.VRMonitor32Path;
                if (vrMonitor32Path == null || !vrMonitor32Path.Exists)
                {
                    return true;
                }

                return KillProcessesByFullPath(vrMonitor32Path.FullName);
            }

            internal static bool KillSteamVRViaVRMonitor64(SteamVRInfo steamVRInfo)
            {
                var vrMonitor64Path = steamVRInfo?.VRMonitor64Path;
                if (vrMonitor64Path == null || !vrMonitor64Path.Exists)
                {
                    return true;
                }

                return KillProcessesByFullPath(vrMonitor64Path.FullName);
            }

            internal static bool LaunchSteamVR()
            {
                var steamVRList = GetSteamVRList();
                if (steamVRList.Count <= 0)
                {
                    return false;
                }

                foreach (var steamVRInfo in steamVRList)
                {
                    var success = LaunchSteamVR(steamVRInfo);
                    if (success)
                    {
                        return true;
                    }
                }

                return false;
            }

            internal static bool LaunchSteamVR(SteamVRInfo steamVRInfo)
            {
                return LaunchSteamVRViaVRStartUp(steamVRInfo) || LaunchSteamVRViaVRMonitor(steamVRInfo);
            }

            internal static bool LaunchSteamVRViaVRMonitor(SteamVRInfo steamVRInfo)
            {
                return LaunchSteamVRViaVRMonitor64(steamVRInfo) || LaunchSteamVRViaVRMonitor32(steamVRInfo);
            }

            internal static bool LaunchSteamVRViaVRMonitor32(SteamVRInfo steamVRInfo)
            {
                var vrMonitor32Path = steamVRInfo?.VRMonitor32Path;
                if (vrMonitor32Path == null || !vrMonitor32Path.Exists)
                {
                    return false;
                }

                var fullName = vrMonitor32Path.FullName;
                try
                {
                    using (Process.Start(fullName))
                    {
                        // Skip
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Logger.GetInstance(typeof(DefaultOpenVRManager)).Error($"Can not launch SteamVR via vrmonitor: \"{fullName}\". error: {e.Message}");
                }

                return false;
            }

            internal static bool LaunchSteamVRViaVRMonitor64(SteamVRInfo steamVRInfo)
            {
                var vrMonitor64Path = steamVRInfo?.VRMonitor64Path;
                if (!IsOn64BitSystem() || vrMonitor64Path == null || !vrMonitor64Path.Exists)
                {
                    return false;
                }

                var fullName = vrMonitor64Path.FullName;
                try
                {
                    using (Process.Start(fullName))
                    {
                        // Skip
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Logger.GetInstance(typeof(DefaultOpenVRManager)).Error($"Can not launch SteamVR via vrmonitor: \"{fullName}\". error: {e.Message}");
                }

                return false;
            }

            internal static bool LaunchSteamVRViaVRStartUp(SteamVRInfo steamVRInfo)
            {
                return LaunchSteamVRViaVRStartUp64(steamVRInfo) || LaunchSteamVRViaVRStartUp32(steamVRInfo);
            }

            internal static bool LaunchSteamVRViaVRStartUp32(SteamVRInfo steamVRInfo)
            {
                var vrStartUp32Path = steamVRInfo?.VRStartUp32Path;
                if (vrStartUp32Path == null || !vrStartUp32Path.Exists)
                {
                    return false;
                }

                var fullName = vrStartUp32Path.FullName;
                try
                {
                    using (Process.Start(fullName))
                    {
                        // Skip
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Logger.GetInstance(typeof(DefaultOpenVRManager)).Error($"Can not launch SteamVR via vrstartup: \"{fullName}\". error: {e.Message}");
                }

                return false;
            }

            internal static bool LaunchSteamVRViaVRStartUp64(SteamVRInfo steamVRInfo)
            {
                var vrStartUp64Path = steamVRInfo?.VRStartUp64Path;
                if (!IsOn64BitSystem() || vrStartUp64Path == null || !vrStartUp64Path.Exists)
                {
                    return false;
                }

                var fullName = vrStartUp64Path.FullName;
                try
                {
                    using (Process.Start(fullName))
                    {
                        // Skip
                    }
                    return true;
                }
                catch (Exception e)
                {
                    Logger.GetInstance(typeof(DefaultOpenVRManager)).Error($"Can not launch SteamVR via vrstartup: \"{fullName}\". error: {e.Message}");
                }

                return false;
            }

            private static string ParseSteamVRVersion(FileInfo filePath)
            {
                if (filePath == null || !filePath.Exists)
                {
                    return null;
                }

                try
                {
                    using (var streamReader = File.OpenText(filePath.FullName))
                    {
                        string line;
                        if ((line = streamReader.ReadLine()) != null)
                        {
                            var versionCode = line;
                            if (!string.IsNullOrWhiteSpace(versionCode))
                            {
                                var versionName = "Unknown";
                                if (VersionCodeWithVersionName.ContainsKey(versionCode))
                                {
                                    versionName = VersionCodeWithVersionName[versionCode];
                                }
                                return $"{versionName} ({versionCode})";
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.GetInstance(typeof(Runtime)).Error($"Can not parse SteamVR version: {e.Message}");
                }

                return null;
            }
        }
    }
}
