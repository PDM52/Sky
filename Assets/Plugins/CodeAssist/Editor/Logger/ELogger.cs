#nullable enable


using System;
using System.IO;
using System.Runtime.InteropServices;
using Meryel.UnityCodeAssist.Synchronizer.Model;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using UnityEditor;
using UnityEngine;

namespace Meryel.UnityCodeAssist.Editor.Logger
{
    [InitializeOnLoad]
    public static class ELogger
    {
        // Change 'new LoggerConfiguration().MinimumLevel.Debug();' if you change these values
        private const LogEventLevel fileMinLevel = LogEventLevel.Debug;
        private const LogEventLevel outputWindowMinLevel = LogEventLevel.Information;
        private static LoggingLevelSwitch? fileLevelSwitch, outputWindowLevelSwitch;

        //static bool IsInitialized { get; set; }

        private static ILogEventSink? _outputWindowSink;
        private static ILogEventSink? _memorySink;

        //**-- make it work with multiple clients
        private static string? _vsInternalLog;


        static ELogger()
        {
            var isFirst = false;
            const string stateName = "isFirst";
            if (!SessionState.GetBool(stateName, false))
            {
                isFirst = true;
                SessionState.SetBool(stateName, true);
            }

            var projectPath = CommonTools.GetProjectPath();
            var outputWindowSink = new Lazy<ILogEventSink>(() => new UnityOutputWindowSink(null));

            Init(isFirst, projectPath, outputWindowSink);

            if (isFirst)
                LogHeader(Application.unityVersion, projectPath);
            Log.Debug("PATH: {Path}", projectPath);
        }

        public static string? FilePath { get; private set; }
        public static string? VSFilePath { get; private set; }

        public static string? VsInternalLog
        {
            get => _vsInternalLog;
            set
            {
                _vsInternalLog = value;
                OnVsInternalLogChanged?.Invoke();
            }
        }

        //**-- UI for these two
        private static bool OptionsIsLoggingToFile => true;
        private static bool OptionsIsLoggingToOutputWindow => true;
        public static event Action? OnVsInternalLogChanged;


        public static string GetInternalLogContent()
        {
            return _memorySink == null ? string.Empty : ((MemorySink)_memorySink).Export();
        }

        public static int GetErrorCountInInternalLog()
        {
            return _memorySink == null ? 0 : ((MemorySink)_memorySink).ErrorCount;
        }

        public static int GetWarningCountInInternalLog()
        {
            return _memorySink == null ? 0 : ((MemorySink)_memorySink).WarningCount;
        }


        private static void LogHeader(string unityVersion, string solutionDir)
        {
            var os = RuntimeInformation.OSDescription;
            var assisterVersion = Assister.Version;
            var syncModel = Utilities.Version;
            var hash = CommonTools.GetHashOfPath(solutionDir);
            Log.Debug(
                "Beginning logging {OS}, Unity {U}, Unity Code Assist {A}, Communication Protocol {SM}, Project: '{Dir}', Project Hash: {Hash}",
                os, unityVersion, assisterVersion, syncModel, solutionDir, hash);
        }


        private static string GetFilePath(string solutionDir)
        {
            var solutionHash = CommonTools.GetHashOfPath(solutionDir);
            var tempDir = Path.GetTempPath();
            var fileName =
                $"UCA_U_LOG_{solutionHash}_.TXT"; // hour code will be appended to the end of file, so add a trailing '_'
            var filePath = Path.Combine(tempDir, fileName);
            return filePath;
        }

        private static string GetVSFilePath(string solutionDir)
        {
            var solutionHash = CommonTools.GetHashOfPath(solutionDir);
            var tempDir = Path.GetTempPath();
            var fileName =
                $"UCA_VS_LOG_{solutionHash}_.TXT"; // hour code will be appended to the end of file, so add a trailing '_'
            var filePath = Path.Combine(tempDir, fileName);
            return filePath;
        }


        public static void Init(bool isFirst, string solutionDir, Lazy<ILogEventSink> outputWindowSink)
        {
            FilePath = GetFilePath(solutionDir);
            VSFilePath = GetVSFilePath(solutionDir);

            fileLevelSwitch = new LoggingLevelSwitch(fileMinLevel);
            outputWindowLevelSwitch = new LoggingLevelSwitch();

            var config = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.With(new DomainHashEnricher());

            const string outputTemplate =
                "{Timestamp:HH:mm:ss.fff} [U] [{Level:u3}] [{DomainHash}] {Message:lj}{NewLine}{Exception}";

            config = config.WriteTo.PersistentFile(FilePath
                , outputTemplate: outputTemplate
                , shared: true
                , persistentFileRollingInterval: PersistentFileRollingInterval.Day
                , preserveLogFilename: true
                , levelSwitch: fileLevelSwitch
                , rollOnEachProcessRun: isFirst
            );

            _outputWindowSink ??= outputWindowSink.Value;
            if (_outputWindowSink != null)
                config = config.WriteTo.Sink(_outputWindowSink, outputWindowMinLevel, outputWindowLevelSwitch);

            _memorySink ??= new MemorySink(outputTemplate);
            config = config.WriteTo.Sink(_memorySink, fileMinLevel, null);

            Log.Logger = config.CreateLogger();
            //switchableLogger.Set(config.CreateLogger(), disposePrev: true);

            OnOptionsChanged();

            //IsInitialized = true;
        }

        public static void OnOptionsChanged()
        {
            // Since we don't use LogEventLevel.Fatal, we can use it for disabling sinks

            var isLoggingToFile = OptionsIsLoggingToFile;
            var targetFileLevel = isLoggingToFile ? fileMinLevel : LogEventLevel.Fatal;
            if (fileLevelSwitch != null)
                fileLevelSwitch.MinimumLevel = targetFileLevel;

            var isLoggingToOutputWindow = OptionsIsLoggingToOutputWindow;
            var targetOutputWindowLevel = isLoggingToOutputWindow ? outputWindowMinLevel : LogEventLevel.Fatal;
            if (outputWindowLevelSwitch != null)
                outputWindowLevelSwitch.MinimumLevel = targetOutputWindowLevel;
        }
    }
}