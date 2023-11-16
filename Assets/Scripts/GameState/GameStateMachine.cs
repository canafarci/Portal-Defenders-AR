using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PortalDefendersAR.GameStates
{
    public class GameStateMachine : IInitializable, ITickable
    {
        private Dictionary<GameState, IGameState> _stateLookup = new();
        private IGameState _currentState;

        private GameStateMachine([Inject(Id = GameState.PlacingObject)] IGameState placingObjectState,
                                 [Inject(Id = GameState.Playing)] IGameState playingState)
        {
            _currentState = placingObjectState;
            _stateLookup[GameState.PlacingObject] = placingObjectState;
            _stateLookup[GameState.Playing] = playingState;
        }

        public void Initialize()
        {
            _currentState.Enter();
        }

        public void Tick()
        {
            GameState nextState = _currentState.Tick();

            //tick function returns a enum for the next state when state has decided to exit
            if (nextState != GameState.StayInState)
            {
                ChangeState(nextState);
            }
        }

        private void ChangeState(GameState state)
        {
            if (_stateLookup.ContainsKey(state))
            {
                IGameState nextState = _stateLookup[state];
                TransitionToNextState(nextState);
            }
            else
            {
                throw new Exception($"{state} state is not found");
            }
        }

        private void TransitionToNextState(IGameState nextState)
        {
            _currentState.Exit();
            _currentState = nextState;
            _currentState.Enter();
        }
    }

    public interface IGameState
    {
        public void Enter();
        public GameState Tick();
        public void Exit();
    }

    public enum GameState
    {
        StayInState,
        PlacingObject,
        Playing,
        GameWon,
        GameLost
    }
}


