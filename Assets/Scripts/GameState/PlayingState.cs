using System.Collections;
using System.Collections.Generic;
using PortalDefendersAR.ARModules;
using PortalDefendersAR.Creation;
using PortalDefendersAR.GameInput;
using PortalDefendersAR.GameStates;
using UnityEngine;
using Zenject;

namespace PortalDefendersAR.GameStates
{
    public class PlayingState : IGameState
    {
        private ITouchInputChecker _touchInputChecker;
        private ITouchInputChecker _draggingInputChecker;
        private TimeTracker _timeTracker;
        private IPoseRaycaster _bombRaycaster;
        private IPoseRaycaster _dragPlaneRaycaster;
        private BombThrower _bombThrower;
        private Bomb.Factory _bombFactory;

        private Bomb _currentBomb;
        private PlayingStates _currentPlayingState;

        public PlayingState([Inject(Id = InputCheckers.TouchChecker)] ITouchInputChecker touchChecker,
                            [Inject(Id = InputCheckers.DragChecker)] ITouchInputChecker dragChecker,
                            TimeTracker timeTracker,
                            [Inject(Id = Raycasters.BombRaycaster)] IPoseRaycaster bombRaycaster,
                            [Inject(Id = Raycasters.DragPlaneRaycaster)] IPoseRaycaster dragRaycaster,
                            BombThrower bombThrower,
                            Bomb.Factory bombFactory)
        {
            _touchInputChecker = touchChecker;
            _draggingInputChecker = dragChecker;
            _timeTracker = timeTracker;
            _bombRaycaster = bombRaycaster;
            _dragPlaneRaycaster = dragRaycaster;
            _bombThrower = bombThrower;
            _bombFactory = bombFactory;
        }

        public void Enter()
        {
            EnableUI();
            _currentBomb = _bombFactory.Create();
        }

        public void Exit()
        {
            DisableUI();
            DestroyBomb();
        }

        public GameStates.GameState Tick()
        {
            GameState nextGameState = CheckTransition();

            switch (_currentPlayingState)
            {
                case PlayingStates.Idle:
                    HandleIdleState();
                    break;

                case PlayingStates.DraggingBomb:
                    HandleDraggingBombState();
                    break;
            }

            return nextGameState;
        }

        private void HandleIdleState()
        {
            if (_touchInputChecker.CheckScreenTouch(out Touch touch))
            {
                if (_bombRaycaster.TryRaycastValidPose(touch.position, out Pose pose))
                {
                    _currentPlayingState = PlayingStates.DraggingBomb;
                }
            }
        }

        private void HandleDraggingBombState()
        {
            if (_draggingInputChecker.CheckScreenTouch(out Touch touch))
            {
                if (_dragPlaneRaycaster.TryRaycastValidPose(touch.position, out Pose pose))
                {
                    PrepareBombForDragging();
                    _bombThrower.DragBomb(pose);
                }
            }
            else //stopped dragging
            {
                _bombThrower.Throw();
                ResetBombAndState();
            }
        }

        private void PrepareBombForDragging()
        {
            if (!_bombThrower.HasBomb())
            {
                _bombThrower.SetBomb(_currentBomb);
            }
        }

        private void ResetBombAndState()
        {
            _bombThrower.ClearBomb();
            _currentBomb = _bombFactory.Create();
            _currentPlayingState = PlayingStates.Idle;
        }

        private GameState CheckTransition()
        {
            if (_timeTracker.CheckTimeOver(Time.deltaTime))
            {
                return GameState.GameWon;
            }
            else
            {
                return GameState.StayInState;
            }
        }

        private void EnableUI()
        {
            // TODO Code to enable UI
        }

        private void DisableUI()
        {
            // TODO Code to disable UI
        }

        private void DestroyBomb()
        {
            //TODO Code to destroy bomb
        }
    }

    public enum PlayingStates
    {
        Idle,
        DraggingBomb
    }

}
