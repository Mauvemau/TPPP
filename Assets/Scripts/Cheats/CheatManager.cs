using System;
using System.Collections.Generic;
using UnityEngine;

public class CheatManager : MonoBehaviour {
    [Header("Config")]
    [SerializeField]
    private List<Transform> quickTeleportWaypoints;

    private PlayerCharacter _playerCharacterReference;
    private PlayerCharacterController _playerControllerReference;
    private GameManager _gameManagerReference;
    private int _currentWaypoint = 0;
    private bool _superSpeed = false;

    [ContextMenu("Instant Skip Level")]
    private void RequestSkipLevel() {
        if (!_gameManagerReference) return;
        if (!_playerCharacterReference) return;
        if (!_gameManagerReference.GetIsGameReady()) return;
        
        _currentWaypoint++;
        if (_currentWaypoint >= quickTeleportWaypoints.Count) _currentWaypoint = 0;
        _playerCharacterReference.RequestSetPosition(quickTeleportWaypoints[_currentWaypoint].position);
    }
    
    [ContextMenu("Infinite Jump Toggle")]
    private void RequestInfiniteJump() {
        if (!_playerControllerReference) return;
        if (!_gameManagerReference.GetIsGameReady()) return;
        _playerControllerReference.ToggleInfiniteJump();
    }
    
    [ContextMenu("Toggle Super Speed")]
    private void RequestSuperSpeed() {
        if (!_gameManagerReference) return;
        if (!_gameManagerReference.GetIsGameReady()) return;
        _superSpeed = !_superSpeed;

        Time.timeScale = _superSpeed ? 2f : 1f;
    }
    
    private void OnGameStarted() {
        if (!ServiceLocator.TryGetService<PlayerCharacter>(out var playerCharacter)) {
            Debug.LogError($"{name}: Unable to find player character on ServiceLocator!");
            return;
        }
        if (!ServiceLocator.TryGetService<PlayerCharacterController>(out var playerController)) {
            Debug.LogError($"{name}: Unable to find player character controller on ServiceLocator!");
            return;
        }
        if (!ServiceLocator.TryGetService<GameManager>(out var gameManager)) {
            Debug.LogError($"{name}: Unable to find game manager on ServiceLocator!");
            return;
        }
        _playerCharacterReference = playerCharacter;
        _playerControllerReference = playerController;
        _gameManagerReference = gameManager;
    }

    private void OnEnable() {
        GameManager.OnPlayerSpawned += OnGameStarted;
        InputManager.OnCheatLevelInputPerformed += RequestSkipLevel;
        InputManager.OnCheatJumpInputPerformed += RequestInfiniteJump;
        InputManager.OnCheatSpeedInputPerformed += RequestSuperSpeed;
    }

    private void OnDisable() {
        GameManager.OnPlayerSpawned -= OnGameStarted;
        InputManager.OnCheatLevelInputPerformed -= RequestSkipLevel;
        InputManager.OnCheatJumpInputPerformed -= RequestInfiniteJump;
        InputManager.OnCheatSpeedInputPerformed -= RequestSuperSpeed;
    }
}
