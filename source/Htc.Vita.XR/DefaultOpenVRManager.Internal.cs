using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Htc.Vita.Core.Crypto;
using Htc.Vita.Core.Log;
using Htc.Vita.Core.Runtime;
using Htc.Vita.Core.Util;
using Convert = Htc.Vita.Core.Util.Convert;

namespace Htc.Vita.XR
{
    public partial class DefaultOpenVRManager
    {
        internal static class Internal
        {
            private const string NativeApiVersion = "1.16.8";
            private const string UnknownVersion = "Unknown";

            private static readonly object FileCheckingLock = new object();
            private static readonly object FileExtractingLock = new object();
            private static readonly Assembly ModuleAssembly = Assembly.GetEntryAssembly() ?? Assembly.GetCallingAssembly();
            private static readonly Dictionary<string, string> Sha1ChecksumWithVersion = new Dictionary<string, string>();

            static Internal()
            {
                InitKnownVersion();
            }

            private static void CheckFileProperties(FileInfo fileInfo)
            {
                Task.Run(() =>
                {
                        DoCheckFileProperties(fileInfo);
                });
            }

            private static bool CheckPathWritable(string path)
            {
                if (!Directory.Exists(path))
                {
                    try
                    {
                        Directory.CreateDirectory(path);
                    }
                    catch (Exception e)
                    {
                        Logger.GetInstance(typeof(DefaultOpenVRManager)).Error($"Can not create \"{path}\", {e.Message}");
                        return false;
                    }
                }

                var now = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture);
                var testFilePath = Path.Combine(path, $"{Sha1.GetInstance().GenerateInHex(now)}.tmp");
                try
                {
                    File.WriteAllText(testFilePath, now);
                    return true;
                }
                catch (Exception e)
                {
                    Logger.GetInstance(typeof(DefaultOpenVRManager)).Error($"Can not write file to \"{path}\", {e.Message}");
                }
                return false;
            }

            private static void DoCheckFileProperties(FileInfo fileInfo)
            {
                if (fileInfo == null || !fileInfo.Exists)
                {
                    return;
                }

                lock (FileCheckingLock)
                {
                    var checksum = Sha1.GetInstance().GenerateInHex(fileInfo);
                    var version = GetVersionFrom(checksum);
                    if (!UnknownVersion.Equals(version))
                    {
                        Logger.GetInstance(typeof(DefaultOpenVRManager)).Debug($"{fileInfo.FullName}, sha1: {checksum}, version: {version}");
                        return;
                    }
                    Logger.GetInstance(typeof(DefaultOpenVRManager)).Warn($"{fileInfo.FullName}, sha1: {checksum}, version: {version}");

                    var sourceFileName = fileInfo.FullName;
                    var destFileName = $"{sourceFileName}_{Convert.ToTimestampInMilli(DateTime.UtcNow)}.bak";
                    try
                    {
                        File.Move(sourceFileName, destFileName);
                    }
                    catch (Exception e)
                    {
                        Logger.GetInstance(typeof(DefaultOpenVRManager)).Error($"Can not move unknown \"{sourceFileName}\" to \"{destFileName}\". {e}");
                    }
                }
            }

            private static string GetApplicationDataPath()
            {
                var path = GetWritablePathFromEnvironmentVariable("TEMP")
                        ?? GetWritablePathFromEnvironmentVariable("TMP")
                        ?? GetWritablePathFromEnvironmentVariableLocalAppData()
                        ?? GetWritablePathFromEnvironmentVariableUserProfile();
                if (!string.IsNullOrWhiteSpace(path))
                {
                    return Path.Combine(path, "Vita");
                }
                return string.Empty;
            }

            private static string GetBinaryFilePath(string platform, string fileName)
            {
                var path = GetApplicationDataPath();
                if (string.IsNullOrWhiteSpace(path))
                {
                    return string.Empty;
                }
                var path2 = Sha1.GetInstance().GenerateInHex($"{ModuleAssembly.Location}_{NativeApiVersion}_{platform}_{fileName}");
                return Path.Combine(path, path2, fileName);
            }

            private static string GetVersionFrom(string checksum)
            {
                if (string.IsNullOrWhiteSpace(checksum) || !Sha1ChecksumWithVersion.ContainsKey(checksum))
                {
                    return UnknownVersion;
                }

                return Sha1ChecksumWithVersion[checksum];
            }

            private static string GetWritablePathFromEnvironmentVariable(string key)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    return null;
                }
                var path = Environment.GetEnvironmentVariable(key);
                if (string.IsNullOrWhiteSpace(path))
                {
                    return null;
                }

                return CheckPathWritable(path)
                        ? path
                        : null;
            }

            private static string GetWritablePathFromEnvironmentVariableLocalAppData()
            {
                var path = Environment.GetEnvironmentVariable("LOCALAPPDATA");
                if (string.IsNullOrWhiteSpace(path))
                {
                    return null;
                }

                path = Path.Combine(path, "Temp");
                return CheckPathWritable(path)
                        ? path
                        : null;
            }

            private static string GetWritablePathFromEnvironmentVariableUserProfile()
            {
                var path = Environment.GetEnvironmentVariable("USERPROFILE");
                if (string.IsNullOrWhiteSpace(path))
                {
                    return null;
                }

                path = Path.Combine(path, "AppData", "Local", "Temp");
                return CheckPathWritable(path)
                        ? path
                        : null;
            }

            private static void InitKnownVersion()
            {
                Sha1ChecksumWithVersion.Add("7fd3a13b0abec02f84564bef0fb0e89c142ed9c9", "SDK 1.16.8 / Runtime 1.16.8 (win32_x86)");
                Sha1ChecksumWithVersion.Add("c6b6213d1fcd033a3b8b29b247e18f371db556a5", "SDK 1.16.8 / Runtime 1.16.8 (win32_amd64)");
            }

            private static string PrepareBinary(string resourceName, string platformName, string binaryName)
            {
                if (string.IsNullOrWhiteSpace(binaryName))
                {
                    return null;
                }

                var binaryPath = GetBinaryFilePath(platformName, binaryName);
                if (string.IsNullOrWhiteSpace(binaryPath))
                {
                    Logger.GetInstance(typeof(DefaultOpenVRManager)).Error("Can not find binary path to load");
                    return null;
                }

                lock (FileExtractingLock)
                {
                    if (File.Exists(binaryPath))
                    {
                        CheckFileProperties(new FileInfo(binaryPath));
                        return binaryPath;
                    }

                    var tempBinaryPath = $"{binaryPath}.{Convert.ToTimestampInMilli(DateTime.UtcNow)}";

                    Extract.FromAssemblyToFileByResourceName(
                            resourceName,
                            new FileInfo(tempBinaryPath),
                            Extract.CompressionType.Gzip
                    );

                    if (!File.Exists(binaryPath) && File.Exists(tempBinaryPath))
                    {
                        try
                        {
                            File.Move(tempBinaryPath, binaryPath);
                        }
                        catch (Exception e)
                        {
                            Logger.GetInstance(typeof(DefaultOpenVRManager)).Error($"Can not move file from \"{tempBinaryPath}\". {e}");
                        }
                    }

                    if (File.Exists(binaryPath))
                    {
                        CheckFileProperties(new FileInfo(binaryPath));
                        return binaryPath;
                    }
                }

                return null;
            }

            internal static Platform.NativeLibInfo PrepareLibrary()
            {
                const string prefix = "Htc.Vita.XR";
                var is64 = IntPtr.Size == 8;
                if (is64)
                {
                    return Platform.LoadNativeLib(
                            PrepareBinary(
                                    $"{prefix}.amd64.{Library.OpenVRApi}.dll.gz",
                                    "amd64",
                                    $"{Library.OpenVRApi}.dll"
                            ) ?? $"amd64/{Library.OpenVRApi}.dll"
                    );
                }
                return Platform.LoadNativeLib(
                        PrepareBinary(
                                $"{prefix}.x86.{Library.OpenVRApi}.dll.gz",
                                "x86",
                                $"{Library.OpenVRApi}.dll"
                        ) ?? $"x86/{Library.OpenVRApi}.dll"
                );
            }
        }

        internal static class Library
        {
            internal const string OpenVRApi = "openvr_api";
        }
    }
}
