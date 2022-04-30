using UnityEngine;

public class SpawnController: BaseController
{
    private readonly ResourcePath _viewPath = new ResourcePath { PathResource = "Prefabs/Car" };
    private ProfilePlayer _profilePlayer;
    private SpawnView _spawnView;

    public SpawnController()
    {
        _spawnView= LoadView();
    }

    private SpawnView LoadView()
    {
        var objView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath));
        AddGameObjects(objView);

        return objView.GetComponent<SpawnView>();
    }

    public SpawnController(ProfilePlayer profilePlayer)
    {
        _profilePlayer = profilePlayer;
    }
}
