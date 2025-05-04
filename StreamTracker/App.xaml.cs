using StreamLib.Core;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace StreamTracker;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // ロガーの初期化
        Logger.Initialize(
            logFilePath: Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "StreamTracker",
                "logs",
                $"streamtracker_{DateTime.Now:yyyyMMdd}.log"),
            minimumLevel: LogLevel.Debug,
            outputs: LogOutput.All
        );

        // 起動ログ
        Logger.Instance.Info("アプリケーションを起動しました", "Application");
    }
}



