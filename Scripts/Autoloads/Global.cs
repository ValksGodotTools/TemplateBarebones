global using Godot;
global using System;
global using System.Collections.Generic;
global using System.Collections.Concurrent;
global using System.Diagnostics;
global using System.Linq;
global using System.Runtime.CompilerServices;
global using System.Threading;
global using System.Text.RegularExpressions;
global using System.Threading.Tasks;

namespace BarebonesTemplate;

public partial class Global : Node
{
    public event Action<GameData> OnGameSaved;
    public event Action<GameData> OnGameLoaded;
    public const string PATH = "/root/Global";

    const string SAVE_DATA_PATH = "user://save_data.tres";

    GameData gameData;

    public override void _Ready()
    {
        // Wait for all scenes to load before loading the game data
        CallDeferred(nameof(LoadGameData));
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
            Quit();
    }

    public void Quit()
    {
        // Handle cleanup here
        SaveGameData();

        GetTree().Quit();
    }

    void SaveGameData()
    {
        OnGameSaved?.Invoke(gameData);

        Error error = ResourceSaver.Save(gameData, SAVE_DATA_PATH);

        if (error != Error.Ok)
            throw new Exception($"Failed to save game data. {error}");
    }

    void LoadGameData()
    {
        bool fileExists = FileAccess.FileExists(SAVE_DATA_PATH);

        // Note that the GameData script path will need to be updated if the
        // GameData script was moved to a new spot.
        //
        // GameData is stored here if ran from within editor
        // C:\Users\VALK-DESKTOP\AppData\Roaming\Godot\app_userdata\BarebonesTemplate
        // 
        // GameData is stored here if ran from exported release
        // C:\Users\VALK-DESKTOP\AppData\Roaming\BarebonesTemplate
        gameData = fileExists ?
            GD.Load<GameData>(SAVE_DATA_PATH) : new();

        OnGameLoaded?.Invoke(gameData);
    }
}
