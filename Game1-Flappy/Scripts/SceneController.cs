using System;
using Game1flappy.Scripts.Globals.ConfigurationObjects;
using Godot;

public partial class SceneController : Node2D
{
    [Export] LevelControl _levelController;
    [Export] PopupMenuController _menuOverlay;
    [Export] CanvasModulate _lightingFilter;
    private SettingsManager _settingsBindings;
    [Export] private AudioStreamPlayer _menuEffectPlayer;


    public override void _Ready()
    {
        _settingsBindings = GetNode<SettingsManager>("/root/SettingsManager");

        _settingsBindings.LightingOnChange += OnLightingChange;
        OnLightingChange(_settingsBindings.LightingSettings);

        var allChildren = FindChildren("*", "Control");

        foreach (var control in allChildren)
        {
            if (control is BaseButton button)
            {
                button.ButtonDown += () => _menuEffectPlayer.Play();
            }
            else if (control is Slider slider)
            {
                slider.ValueChanged += (_) => _menuEffectPlayer.Play();
            }
        }

    }

    public void TogglePause()
    {
        var tree = GetTree();
        var currentState = tree.Paused;

        tree.Paused = !currentState;

        _menuOverlay.ActivatePaused();
        _menuOverlay.Visible = !currentState;

    }

    private void OnLightingChange(LightingSettings settings)
    {
        _lightingFilter.Visible = settings.DynamicLightingEnabled;

    }


    public void StartLife()
    {
        _levelController.BeginSpawning();
        _menuOverlay.Visible = false;
    }

    internal void EndOfLife()
    {

        if(_levelController.Score > _settingsBindings.HighScore)
        {
            _settingsBindings.HighScore = _levelController.Score;
            _settingsBindings.SaveConfiguration();
        }
        _menuOverlay.ActivateEndOfLife();
    }

    public void GoToMainMenu()
    {
        GetTree().ChangeSceneToFile("res://GameScenes/menuscene.tscn");
    }
}
