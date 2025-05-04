using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace StreamLib.Core
{
    /// <summary>
    /// ログの重要度レベル
    /// </summary>
    public enum LogLevel
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        Fatal = 4
    }

    /// <summary>
    /// ログ出力先の種類
    /// </summary>
    [Flags]
    public enum LogOutput
    {
        None = 0,
        Console = 1,
        Debug = 2,
        File = 4,
        All = Console | Debug | File
    }

    /// <summary>
    /// StreamLibのロギング機能を提供するクラス
    /// </summary>
    public class Logger
    {
        private static readonly object _lockObject = new object();
        private static Logger? _instance;

        private readonly string _logFilePath;
        private readonly LogLevel _minimumLevel;
        private readonly LogOutput _outputs;

        /// <summary>
        /// LoggerのInstanceを取得する
        /// </summary>
        public static Logger Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        _instance ??= new Logger();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Loggerを初期化する
        /// </summary>
        /// <param name="logFilePath">ログファイルのパス（nullの場合はファイル出力なし）</param>
        /// <param name="minimumLevel">出力する最小ログレベル</param>
        /// <param name="outputs">ログの出力先</param>
        public static void Initialize(string? logFilePath = null, LogLevel minimumLevel = LogLevel.Debug, LogOutput outputs = LogOutput.All)
        {
            lock (_lockObject)
            {
                _instance = new Logger(logFilePath, minimumLevel, outputs);
            }
        }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        private Logger()
            : this(null, LogLevel.Debug, LogOutput.Debug)
        {
        }

        /// <summary>
        /// Loggerのコンストラクタ
        /// </summary>
        /// <param name="logFilePath">ログファイルのパス</param>
        /// <param name="minimumLevel">出力する最小ログレベル</param>
        /// <param name="outputs">ログの出力先</param>
        private Logger(string? logFilePath, LogLevel minimumLevel, LogOutput outputs)
        {
            _logFilePath = logFilePath ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StreamLib",
                "logs",
                $"streamlib_{DateTime.Now:yyyyMMdd}.log");

            _minimumLevel = minimumLevel;
            _outputs = outputs;

            // ログファイルのディレクトリが存在しない場合は作成
            if ((_outputs & LogOutput.File) == LogOutput.File)
            {
                var directory = Path.GetDirectoryName(_logFilePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
            }
        }

        /// <summary>
        /// ログを出力する
        /// </summary>
        /// <param name="level">ログレベル</param>
        /// <param name="message">メッセージ</param>
        /// <param name="category">カテゴリ</param>
        /// <param name="callerMemberName">呼び出し元メンバー名</param>
        /// <param name="callerFilePath">呼び出し元ファイルパス</param>
        /// <param name="callerLineNumber">呼び出し元行番号</param>
        public void Log(
            LogLevel level,
            string message,
            string? category = null,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            if (level < _minimumLevel)
                return;

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var callerInfo = Path.GetFileName(callerFilePath);
            var categoryInfo = string.IsNullOrEmpty(category) ? "" : $"[{category}]";
            var formattedMessage = $"{timestamp} {level.ToString().ToUpper()} {categoryInfo}[{callerInfo}:{callerMemberName}({callerLineNumber})] {message}";

            // デバッグ出力
            if ((_outputs & LogOutput.Debug) == LogOutput.Debug)
            {
                System.Diagnostics.Debug.WriteLine(formattedMessage);
            }

            // コンソール出力
            if ((_outputs & LogOutput.Console) == LogOutput.Console)
            {
                var originalColor = Console.ForegroundColor;
                SetConsoleColor(level);
                Console.WriteLine(formattedMessage);
                Console.ForegroundColor = originalColor;
            }

            // ファイル出力
            if ((_outputs & LogOutput.File) == LogOutput.File)
            {
                try
                {
                    lock (_lockObject)
                    {
                        File.AppendAllText(_logFilePath, formattedMessage + Environment.NewLine);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ログファイルへの書き込みに失敗しました: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// デバッグレベルのログを出力する
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="category">カテゴリ</param>
        /// <param name="callerMemberName">呼び出し元メンバー名</param>
        /// <param name="callerFilePath">呼び出し元ファイルパス</param>
        /// <param name="callerLineNumber">呼び出し元行番号</param>
        public void Debug(
            string message,
            string? category = null,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Log(LogLevel.Debug, message, category, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// 情報レベルのログを出力する
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="category">カテゴリ</param>
        /// <param name="callerMemberName">呼び出し元メンバー名</param>
        /// <param name="callerFilePath">呼び出し元ファイルパス</param>
        /// <param name="callerLineNumber">呼び出し元行番号</param>
        public void Info(
            string message,
            string? category = null,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Log(LogLevel.Info, message, category, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// 警告レベルのログを出力する
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="category">カテゴリ</param>
        /// <param name="callerMemberName">呼び出し元メンバー名</param>
        /// <param name="callerFilePath">呼び出し元ファイルパス</param>
        /// <param name="callerLineNumber">呼び出し元行番号</param>
        public void Warning(
            string message,
            string? category = null,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Log(LogLevel.Warning, message, category, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// エラーレベルのログを出力する
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="category">カテゴリ</param>
        /// <param name="callerMemberName">呼び出し元メンバー名</param>
        /// <param name="callerFilePath">呼び出し元ファイルパス</param>
        /// <param name="callerLineNumber">呼び出し元行番号</param>
        public void Error(
            string message,
            string? category = null,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Log(LogLevel.Error, message, category, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// 致命的エラーレベルのログを出力する
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="category">カテゴリ</param>
        /// <param name="callerMemberName">呼び出し元メンバー名</param>
        /// <param name="callerFilePath">呼び出し元ファイルパス</param>
        /// <param name="callerLineNumber">呼び出し元行番号</param>
        public void Fatal(
            string message,
            string? category = null,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            Log(LogLevel.Fatal, message, category, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// 例外をログに出力する
        /// </summary>
        /// <param name="ex">例外</param>
        /// <param name="message">追加メッセージ</param>
        /// <param name="category">カテゴリ</param>
        /// <param name="level">ログレベル</param>
        /// <param name="callerMemberName">呼び出し元メンバー名</param>
        /// <param name="callerFilePath">呼び出し元ファイルパス</param>
        /// <param name="callerLineNumber">呼び出し元行番号</param>
        public void Exception(
            Exception ex,
            string? message = null,
            string? category = null,
            LogLevel level = LogLevel.Error,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0)
        {
            var exMessage = message != null
                ? $"{message} - 例外: {ex.GetType().Name}: {ex.Message}"
                : $"例外: {ex.GetType().Name}: {ex.Message}";

            if (ex.StackTrace != null)
            {
                exMessage += $"{Environment.NewLine}StackTrace: {ex.StackTrace}";
            }

            Log(level, exMessage, category, callerMemberName, callerFilePath, callerLineNumber);

            if (ex.InnerException != null)
            {
                Exception(ex.InnerException, "InnerException", category, level, callerMemberName, callerFilePath, callerLineNumber);
            }
        }

        /// <summary>
        /// ログレベルに応じてコンソールの色を設定する
        /// </summary>
        /// <param name="level">ログレベル</param>
        private static void SetConsoleColor(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Gray;
                    break;
                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                case LogLevel.Fatal:
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
        }
    }
}
