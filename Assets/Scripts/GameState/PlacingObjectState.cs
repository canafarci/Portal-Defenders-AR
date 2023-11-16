using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PortalDefendersAR.GameStates
{
    public class PlacingObjectState : IGameState
    {
        public void Enter()
        {
            UnityEngine.Debug.Log("Entered");
        }

        public void Exit()
        {
            UnityEngine.Debug.Log("Entered");
        }

        public GameState Tick()
        {
            UnityEngine.Debug.Log("Ticking");
            return GameState.StayInState;
        }
    }
}
