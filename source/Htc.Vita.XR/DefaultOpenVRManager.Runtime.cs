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
                                    VRMonitor32Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win32", "vrmonitor.exe")),
                                    VRMonitor64Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win64", "vrmonitor.exe")),
                                    VRPathReg32Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win32", "vrpathreg.exe")),
                                    VRPathReg64Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win64", "vrpathreg.exe")),
                                    VRServer32Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win32", "vrserver.exe")),
                                    VRServer64Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win64", "vrserver.exe")),
                                    VRStartUp32Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win32", "vrstartup.exe")),
                                    VRStartUp64Path = GetSteamVRComponentPath(runtimePath, Path.Combine("bin", "win64", "vrstartup.exe"))
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
                VersionCodeWithVersionName.Add("1615513459", "1.6.10");
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

                var targetProcessName = steamVRInfo.VRMonitor32Path?.Name;
                if (string.IsNullOrWhiteSpace(targetProcessName))
                {
                    targetProcessName = steamVRInfo.VRMonitor64Path?.Name;
                }
                if (string.IsNullOrWhiteSpace(targetProcessName))
                {
                    return false;
                }

                const string suffix = ".exe";
                if (targetProcessName.EndsWith(suffix))
                {
                    targetProcessName = targetProcessName.Substring(0, targetProcessName.Length - suffix.Length);
                }

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

                var targetProcessName = steamVRInfo.VRServer32Path?.Name;
                if (string.IsNullOrWhiteSpace(targetProcessName))
                {
                    targetProcessName = steamVRInfo.VRServer64Path?.Name;
                }
                if (string.IsNullOrWhiteSpace(targetProcessName))
                {
                    return false;
                }

                const string suffix = ".exe";
                if (targetProcessName.EndsWith(suffix))
                {
                    targetProcessName = targetProcessName.Substring(0, targetProcessName.Length - suffix.Length);
                }

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
