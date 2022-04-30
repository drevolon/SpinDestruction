using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MainMenuView : MonoBehaviour
{
    [SerializeField]
    private Button _buttonStart;
    [SerializeField]
    private Button _buttonExit;

    //public UnityAction Start;
    //public UnityAction Exit;

    //private void Awake()
    //{
    //   // _buttonStart.onClick.AddListener(OnStartClick);
    //   // _buttonExit.onClick.AddListener(OnExitClick);
    //}

    //private void OnStartClick()
    //{
    //    Start?.Invoke();
    //}

    //private void OnExitClick()
    //{
    //    Exit?.Invoke();
    //}

    public void Init(UnityAction startGame, UnityAction exitGame)
    {
        
        _buttonStart.onClick.AddListener(()=> { startGame?.Invoke(); });
        _buttonExit.onClick.AddListener(()=> { exitGame?.Invoke(); });
        //_buttonStart.onClick.AddListener(OnStartClick);
        //_buttonExit.onClick.AddListener(OnExitClick);

    }

    protected void OnDestroy()
    {
        _buttonStart.onClick.RemoveAllListeners();
        _buttonExit.onClick.RemoveAllListeners();
    }
}
