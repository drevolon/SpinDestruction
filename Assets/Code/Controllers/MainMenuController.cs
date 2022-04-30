using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class MainMenuController : BaseController
{
    
    private readonly ProfilePlayer _profilePlayer;
    private readonly MainMenuView _view;

    private readonly ResourcePath _viewPath = new ResourcePath { PathResource = "Prefabs/mainMenu" };


    public MainMenuController(Transform placeForUi, ProfilePlayer profilePlayer)
    {
        _profilePlayer = profilePlayer;
        _view = LoadView(placeForUi);
            

        _view.Init(StartGame, ExitGame);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void StartGame()
    {
        //string v = GameObject.Find("InputFieldVelocity").GetComponent<TMP_InputField>().text;
        //string dv = GameObject.Find("InputFieldDeltaVelocity").GetComponent<TMP_InputField>().text;
        //string w = GameObject.Find("InputFieldOmega").GetComponent<TMP_InputField>().text;

        //float.TryParse(v, out PlayerParams.StartVelocity);
        //float.TryParse(dv, out PlayerParams.DeltaVelocity);
        //float.TryParse(w,out PlayerParams.StartOmega);

        _profilePlayer.CurrentState.Value = GameState.Game;
    }

    private MainMenuView LoadView(Transform placeForUi)
    {
        var objectView = Object.Instantiate(ResourceLoader.LoadPrefab(_viewPath), placeForUi, false);
        AddGameObjects(objectView);

        return objectView.GetComponent<MainMenuView>();
    }


}
