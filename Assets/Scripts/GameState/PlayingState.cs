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

        private PlayingState([Inject(Id = InputCheckers.TouchChecker)] ITouchInputChecker touchChecker,
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
            //enable ui
            //spawn a ball from factory
            //signal portal to start spawning
            _currentBomb = _bombFactory.Create();
        }

        public void Exit()
        {
            //disble ui
            //destroy ball
        }

        public GameStates.GameState Tick()
        {
            GameState nextGameState = CheckTransition();

            if (_currentPlayingState == PlayingStates.Idle)
            {
                if (_touchInputChecker.CheckScreenTouch(out Touch touch))
                {
                    if (_bombRaycaster.TryRaycastValidPose(touch.position, out Pose pose))
                    {
                        _currentPlayingState = PlayingStates.DraggingBomb;
                    }
                }
            }
            else if (_currentPlayingState == PlayingStates.DraggingBomb)
            {
                if (_draggingInputChecker.CheckScreenTouch(out Touch touch))
                {
                    if (_dragPlaneRaycaster.TryRaycastValidPose(touch.position, out Pose pose))
                    {
                        _bombThrower.DragBomb(_currentBomb, pose);
                    }
                }
                else
                {
                    _bombThrower.Throw(_currentBomb);
                    _currentBomb = _bombFactory.Create();
                    _currentPlayingState = PlayingStates.Idle;
                }
            }


            //tick timer

            //listen for raycasts to ball
            //on drag, move ball in the plane
            //on release, throw the ball
            //create new ball on the screen

            return nextGameState;
        }

        private GameState CheckTransition()
        {
            GameState nextGameState;

            if (_timeTracker.CheckTimeOver(Time.deltaTime))
            {
                nextGameState = GameState.GameWon;
            }
            else
            {
                nextGameState = GameState.StayInState;
            }

            return nextGameState;
        }
    }

    public enum PlayingStates
    {
        Idle,
        DraggingBomb
    }


}
