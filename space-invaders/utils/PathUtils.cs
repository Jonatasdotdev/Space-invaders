using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using CapstoneProg3.model;

namespace CapstoneProg3.utils;

public abstract class PathUtils
{
    private static readonly string AppDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    private static readonly string GameDataDir = Path.Combine(AppDataDir, "SpaceInvaders");
    public static readonly string GameFilePath = GameDataDir + "\\" + "score.json";
    public static readonly string FullHealthFilePath = GetFullPath("assets\\fullHeart.png");
    public static readonly string EmptyHealthFilePath = GetFullPath("assets\\emptyHeart.png");
    public static readonly string ExplosionSoundPath = GetFullPath(@"assets\sounds\invasor_killed.wav");
    
    private static string GetProjectBasePath()
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        // removing "bin\Debug\netX.0-windows" from path
        var projectDirectory = Directory.GetParent(baseDirectory)?.Parent?.Parent?.Parent?.FullName;

        if (string.IsNullOrEmpty(projectDirectory))
            throw new InvalidOperationException("Não foi possível determinar o diretório base do projeto.");

        return projectDirectory;
    }
    
    public static string GetFullPath(string relativePath)
    {
        var basePath = GetProjectBasePath();
        return Path.Combine(basePath, relativePath);
    }
    
    public static void CreateGameDirectory()
    {
        try
        {
            if (!Directory.Exists(GameDataDir)) Directory.CreateDirectory(GameDataDir);
            if (!File.Exists(GameDataDir + "\\" + "score.json")) CreateScoreFile();
        }
        catch (UnauthorizedAccessException error)
        {
            MessageBox.Show("Access denied while trying to open AppData directory." +
                            "Please, make it accessible.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (DirectoryNotFoundException error)
        {
            MessageBox.Show("AppData directory not found. " +
                            "Verify if the AppData directory exists", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        catch (Exception error)
        {
            MessageBox.Show("Unexpected error occured.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Console.WriteLine(error.Message);
        }
    }

    public static void CreateScoreFile()
    {
        try
        {
            if (File.Exists(GameFilePath)) return;
            
            var jsonObject = Array.Empty<object>();
            var json = JsonSerializer.Serialize(jsonObject);
            File.WriteAllText(GameFilePath, json);
        }
        catch (UnauthorizedAccessException error)
        {
            MessageBox.Show("Access denied while trying to open AppData directory." +
                            "Please, make it accessible.", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (DirectoryNotFoundException error)
        {
            MessageBox.Show("AppData directory not found. " +
                            "Verify if the AppData directory exists", "Error", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        catch (Exception error)
        {
            MessageBox.Show("Unexpected error occured.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Console.WriteLine(error.Message);
        }
        
    }
}