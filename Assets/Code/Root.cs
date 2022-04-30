using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Root : MonoBehaviour
{
    [SerializeField]
    private Transform _placeForUi;

    private MainController _mainController;

    // private MainMenuController _mainMenuController;

    private void Awake()
    {
        var profilePlayer = new ProfilePlayer(15f);

        profilePlayer.CurrentState.Value = GameState.Start;
        _mainController = new MainController(_placeForUi, profilePlayer);
        
        

        //_mainMenuController = new MainMenuController(_placeForUi, profilePlayer);

        //analytic.SendMessage("Game Start", new Dictionary<string, object>());
    }

    protected void OnDestroy()
    {
        _mainController?.Dispose();
       // _mainMenuController?.Dispose();
    }
}
