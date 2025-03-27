using System;
using System.IO;

public class FileManager
{
    public void CreateFileWithText(string filePath, string content)
    {
        try
        {
            File.WriteAllText(filePath, content);
            Console.WriteLine($"Файл {filePath} успешно создан и заполнен текстом.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при создании файла: {ex.Message}");
        }
    }

    public string ReadFileContent(string filePath)
    {
        try
        {
            return File.ReadAllText(filePath);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            return string.Empty;
        }
    }

    public void DeleteFileIfExists(string filePath)
    {
        try
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Console.WriteLine($"Файл {filePath} успешно удален.");
            }
            else
            {
                Console.WriteLine($"Файл {filePath} не существует.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении файла: {ex.Message}");
        }
    }

    public void CopyFile(string sourcePath, string destPath)
    {
        try
        {
            File.Copy(sourcePath, destPath, true);
            Console.WriteLine($"Файл успешно скопирован из {sourcePath} в {destPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при копировании файла: {ex.Message}");
        }
    }

    public void MoveFile(string sourcePath, string destPath)
    {
        try
        {
            File.Move(sourcePath, destPath);
            Console.WriteLine($"Файл успешно перемещен из {sourcePath} в {destPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при перемещении файла: {ex.Message}");
        }
    }

    public void RenameFile(string oldPath, string newPath)
    {
        try
        {
            File.Move(oldPath, newPath);
            Console.WriteLine($"Файл успешно переименован из {oldPath} в {newPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при переименовании файла: {ex.Message}");
        }
    }

    public void DeleteFilesByPattern(string directory, string pattern)
    {
        try
        {
            var files = Directory.GetFiles(directory, pattern);
            foreach (var file in files)
            {
                File.Delete(file);
                Console.WriteLine($"Удален файл: {file}");
            }
            Console.WriteLine($"Удалено {files.Length} файлов с шаблоном {pattern}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при удалении файлов: {ex.Message}");
        }
    }

    public void ListFilesInDirectory(string directory)
    {
        try
        {
            var files = Directory.GetFiles(directory);
            Console.WriteLine($"Файлы в директории {directory}:");
            foreach (var file in files)
            {
                Console.WriteLine(Path.GetFileName(file));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении списка файлов: {ex.Message}");
        }
    }

    public void SetReadOnlyAndTryWrite(string filePath, string content)
    {
        try
        {
           
            File.SetAttributes(filePath, File.GetAttributes(filePath) | FileAttributes.ReadOnly);
            Console.WriteLine($"Установлен атрибут 'только для чтения' для файла {filePath}");

            File.WriteAllText(filePath, content);
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Ошибка: Запись запрещена (файл только для чтения)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Другая ошибка: {ex.Message}");
        }
        finally
        {
            
            File.SetAttributes(filePath, File.GetAttributes(filePath) & ~FileAttributes.ReadOnly);
        }
    }
}

public class FileInfoProvider
{
    public void DisplayFileInfo(string filePath)
    {
        try
        {
            var fileInfo = new FileInfo(filePath);

            Console.WriteLine($"Информация о файле {filePath}:");
            Console.WriteLine($"Размер: {fileInfo.Length} байт");
            Console.WriteLine($"Дата создания: {fileInfo.CreationTime}");
            Console.WriteLine($"Дата последнего изменения: {fileInfo.LastWriteTime}");
            Console.WriteLine($"Дата последнего доступа: {fileInfo.LastAccessTime}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при получении информации о файле: {ex.Message}");
        }
    }

    public void CompareFilesBySize(string filePath1, string filePath2)
    {
        try
        {
            var fileInfo1 = new FileInfo(filePath1);
            var fileInfo2 = new FileInfo(filePath2);

            Console.WriteLine($"Сравнение файлов по размеру:");
            Console.WriteLine($"{filePath1}: {fileInfo1.Length} байт");
            Console.WriteLine($"{filePath2}: {fileInfo2.Length} байт");

            if (fileInfo1.Length > fileInfo2.Length)
                Console.WriteLine($"{filePath1} больше {filePath2}");
            else if (fileInfo1.Length < fileInfo2.Length)
                Console.WriteLine($"{filePath1} меньше {filePath2}");
            else
                Console.WriteLine("Файлы имеют одинаковый размер");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при сравнении файлов: {ex.Message}");
        }
    }

    public void CheckFilePermissions(string filePath)
    {
        try
        {
            FileInfo fileInfo = new FileInfo(filePath);

            Console.WriteLine($"Права доступа для файла {filePath}:");
            Console.WriteLine($"Чтение возможно: {CanRead(filePath)}");
            Console.WriteLine($"Запись возможна: {CanWrite(filePath)}");
            Console.WriteLine($"Файл только для чтения: {fileInfo.IsReadOnly}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при проверке прав доступа: {ex.Message}");
        }
    }

    private bool CanRead(string filePath)
    {
        try
        {
            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    private bool CanWrite(string filePath)
    {
        try
        {
            using (FileStream fs = File.Open(filePath, FileMode.Open, FileAccess.Write))
            {
                return true;
            }
        }
        catch
        {
            return false;
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        var fileManager = new FileManager();
        var fileInfoProvider = new FileInfoProvider();

        string basePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "FileOperations");
        string originalFileName = "ivanov.ii";
        string originalFilePath = Path.Combine(basePath, originalFileName);
        string copyFilePath = Path.Combine(basePath, "copy.ii");
        string movedDirPath = Path.Combine(basePath, "moved");
        string movedFilePath = Path.Combine(movedDirPath, originalFileName);
        string renamedFilePath = Path.Combine(basePath, "familiya.io");

        Directory.CreateDirectory(basePath);
        Directory.CreateDirectory(movedDirPath);

        fileManager.CreateFileWithText(originalFilePath, "Пример содержимого файла.");
        string content = fileManager.ReadFileContent(originalFilePath);
        Console.WriteLine($"Содержимое файла:\n{content}");

        fileManager.DeleteFileIfExists(originalFilePath);

        fileManager.CreateFileWithText(originalFilePath, "Новое содержимое файла.");
        fileInfoProvider.DisplayFileInfo(originalFilePath);

        fileManager.CopyFile(originalFilePath, copyFilePath);
        Console.WriteLine($"Копия существует: {File.Exists(copyFilePath)}");

        fileManager.MoveFile(originalFilePath, movedFilePath);
        Console.WriteLine($"Исходный файл существует: {File.Exists(originalFilePath)}");
        Console.WriteLine($"Перемещенный файл существует: {File.Exists(movedFilePath)}");

        fileManager.RenameFile(movedFilePath, renamedFilePath);
        Console.WriteLine($"Переименованный файл существует: {File.Exists(renamedFilePath)}");

        fileManager.DeleteFileIfExists(Path.Combine(basePath, "несуществующий_файл.txt"));

        string file1 = Path.Combine(basePath, "file1.txt");
        string file2 = Path.Combine(basePath, "file2.txt");
        fileManager.CreateFileWithText(file1, "Содержимое первого файла");
        fileManager.CreateFileWithText(file2, "Содержимое второго файла, которое длиннее");
        fileInfoProvider.CompareFilesBySize(file1, file2);

        fileManager.CreateFileWithText(Path.Combine(basePath, "test1.ii"), "");
        fileManager.CreateFileWithText(Path.Combine(basePath, "test2.ii"), "");
        fileManager.DeleteFilesByPattern(basePath, "*.ii");

        fileManager.ListFilesInDirectory(basePath);

        string testFilePath = Path.Combine(basePath, "test_permissions.txt");
        fileManager.CreateFileWithText(testFilePath, "Исходное содержимое");
        fileManager.SetReadOnlyAndTryWrite(testFilePath, "Новое содержимое");

        fileInfoProvider.CheckFilePermissions(testFilePath);

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.ReadKey();
    }
}