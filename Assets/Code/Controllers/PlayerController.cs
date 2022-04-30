using System.Collections.Generic;
using UnityEngine;

public class PlayerController : BaseController
{
    private readonly ResourcePath _viewPath = new ResourcePath { PathResource = "Prefabs/top" };
    private readonly ResourcePath _viewPathobjLine = new ResourcePath { PathResource = "Prefabs/LineTarget" };
    private ProfilePlayer _profilePlayer;
    private PlayerView _playerView;
    private PlayerUIView _playerUIView;
    private MousePointerController _mouseController;
    private TouchController _touchController;
    private GameObject _playerObject;
    private SubscriptionProperty<PlayerState> CurrentPlayerState;

    private GameObject LoadObject()
    {
        var objView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath));
        AddGameObjects(objView);
        return objView;
    }
    private void LoadUIController<T>() where T : BaseController
    {
        AddController((_playerObject.AddComponent<T>()));
    }

    public PlayerController(ProfilePlayer profilePlayer)
    {
        _profilePlayer = profilePlayer;

        CurrentPlayerState = new SubscriptionProperty<PlayerState>();

        _playerObject = LoadObject();
        _playerView = _playerObject.AddComponent<PlayerView>();
        _playerView.Init(CurrentPlayerState);

        _playerUIView = _playerObject.AddComponent<PlayerUIView>();
        _playerUIView._view = _playerView;
        _playerUIView.LineTarget = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPathobjLine));
        _playerUIView.Init(CurrentPlayerState);

        // Загружаем контроллеры переферии (мышь, тач и т.д.)
        LoadUIController<MousePointerController>();
        LoadUIController<TouchController>();

        // Первоначальное состояние 
        CurrentPlayerState.Value = PlayerState.NotStart;


    }

    protected override void OnDispose()
    {
        base.OnDispose();
    }
}
