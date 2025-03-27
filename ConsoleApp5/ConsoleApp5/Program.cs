using System;
using System.IO;

public class TempFileWatcher
{
    private readonly FileSystemWatcher _watcher;
    private readonly string _logFilePath;

    public TempFileWatcher(string folderToWatch, string logFilePath)
    {
        _logFilePath = logFilePath;

        _watcher = new FileSystemWatcher
        {
            Path = folderToWatch,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite,
            Filter = "*.tmp",
            EnableRaisingEvents = true
        };

        _watcher.Created += OnFileCreated;
        _watcher.Deleted += OnFileDeleted;
        _watcher.Renamed += OnFileRenamed;
        _watcher.Error += OnError;

        LogMessage($"Начато наблюдение за папкой: {folderToWatch}");
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        try
        {
            LogMessage($"Обнаружен новый временный файл: {e.Name}");

            System.Threading.Thread.Sleep(100);

            File.Delete(e.FullPath);
            LogMessage($"Файл {e.Name} успешно удален");
        }
        catch (Exception ex)
        {
            LogMessage($"Ошибка при удалении файла {e.Name}: {ex.Message}");
        }
    }

    private void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        LogMessage($"Файл {e.Name} был удален");
    }

    private void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        LogMessage($"Файл {e.OldName} переименован в {e.Name}");

        if (Path.GetExtension(e.Name).Equals(".tmp", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                File.Delete(e.FullPath);
                LogMessage($"Переименованный файл {e.Name} удален");
            }
            catch (Exception ex)
            {
                LogMessage($"Ошибка при удалении переименованного файла: {ex.Message}");
            }
        }
    }

    private void OnError(object sender, ErrorEventArgs e)
    {
        LogMessage($"Ошибка в FileSystemWatcher: {e.GetException().Message}");
    }

    private void LogMessage(string message)
    {
        try
        {
            string logEntry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
            File.AppendAllText(_logFilePath, logEntry);
            Console.WriteLine(logEntry);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при записи в лог: {ex.Message}");
        }
    }

    public void StopWatching()
    {
        _watcher.EnableRaisingEvents = false;
        _watcher.Dispose();
        LogMessage("Наблюдение за папкой остановлено");
    }
}

class Program
{
    static void Main(string[] args)
    {
       
        string watchFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "TempFilesWatch");

        string logFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
            "file_watcher.log");

        Directory.CreateDirectory(watchFolder);

        Console.WriteLine($"Наблюдение за временными файлами (*.tmp) в папке: {watchFolder}");
        Console.WriteLine($"Лог-файл: {logFile}");
        Console.WriteLine("Нажмите Q для выхода...");

        var watcher = new TempFileWatcher(watchFolder, logFile);

        while (Console.ReadKey().Key != ConsoleKey.Q)
        {
        }

        watcher.StopWatching();
    }
}
