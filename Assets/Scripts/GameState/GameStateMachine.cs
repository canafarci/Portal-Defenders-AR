using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace PortalDefendersAR.GameState
{
    public class GameStateMachine : IInitializable, ITickable
    {
        private Dictionary<GameState, IGameState> _stateLookup = new();
        private IGameState _currentState;

        public void Initialize()
        {
            _currentState.Enter();
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

        public void Tick()
        {
            GameState nextState = _currentState.Tick();

            //tick function returns a pointer to the next state when state has decided to exit
            if (nextState != GameState.StayInState)
            {
                ChangeState(nextState);
            }
        }

        protected void TransitionToNextState(IGameState nextState)
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


