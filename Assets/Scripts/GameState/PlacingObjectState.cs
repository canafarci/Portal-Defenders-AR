using System.Collections;
using System.Collections.Generic;
using PortalDefendersAR.CoreStructures;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace PortalDefendersAR.GameStates
{
    public class PlacingObjectState : IGameState
    {
        private ARPlaneManager _arPlaneManager; //TODO toggle visibility of the planes when placement finishes
        private ARRaycastManager _arRaycastManager;
        private Portal.Factory _portalFactory;
        private Fortress.Factory _fortressFactory;
        private PlacingObjectStates _currentState;

        private PlacingObjectState(ARPlaneManager arPlaneManager,
                                   ARRaycastManager arRaycastManager,
                                   Portal.Factory portalFactory,
                                   Fortress.Factory fortressFactory)
        {
            _arPlaneManager = arPlaneManager;
            _arRaycastManager = arRaycastManager;
            _portalFactory = portalFactory;
            _fortressFactory = fortressFactory;
        }

        public void Enter()
        {
            _currentState = PlacingObjectStates.PlacingPortal;
        }

        public void Exit()
        {
            UnityEngine.Debug.Log("Entered");
        }

        public GameState Tick()
        {
            GameState nextState = GameState.StayInState;

            if (TryRaycastValidPose(out Pose pose))
            {
                switch (_currentState)
                {
                    case PlacingObjectStates.PlacingPortal:
                        _portalFactory.Create(pose);
                        _currentState = PlacingObjectStates.PlacingFortress;
                        break;

                    case PlacingObjectStates.PlacingFortress:
                        _fortressFactory.Create(pose);
                        _currentState = PlacingObjectStates.Finished;
                        break;

                    case PlacingObjectStates.Finished: //TODO show UI to continue
                        nextState = GameState.Playing; //Signal game state machine to move to the next state
                        break;
                }
            }

            return nextState;
        }

        private bool TryRaycastValidPose(out Pose pose)
        {
            bool raycastSuccessful = false;

            if (CheckTouchedScreen(out Touch touch))
            {
                List<ARRaycastHit> hits = new();
                if (_arRaycastManager.Raycast(touch.position, hits, TrackableType.PlaneWithinPolygon))
                {
                    pose = hits[0].pose;
                    raycastSuccessful = true;
                }
                else
                {
                    pose = default;
                }
            }
            else
            {
                pose = default;
            }

            return raycastSuccessful;
        }

        private bool CheckTouchedScreen(out Touch touch)
        {
            bool hasTouched = false;

            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);

                if (touch.phase == TouchPhase.Began) //return true only if touch is in beginning phase
                {
                    hasTouched = true;
                }
            }
            else
            {
                touch = default;
            }

            return hasTouched;
        }
    }

    public enum PlacingObjectStates
    {
        PlacingPortal,
        PlacingFortress,
        Finished
    }
}
