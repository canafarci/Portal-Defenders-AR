using PortalDefendersAR.ARModules;
using PortalDefendersAR.Creation;
using PortalDefendersAR.GameInput;
using UnityEngine;
using Zenject;

namespace PortalDefendersAR.GameStates
{
    public class PlacingObjectState : IGameState
    {
        private Portal.Factory _portalFactory;
        private Fortress.Factory _fortressFactory;
        private PlacingObjectStates _currentState;
        private IPoseRaycaster _poseRaycaster;
        private ITouchInputChecker _touchInputChecker;

        private PlacingObjectState(Portal.Factory portalFactory,
                                   Fortress.Factory fortressFactory,
                                   IPoseRaycaster raycaster,
                                   ITouchInputChecker inputChecker)
        {
            _portalFactory = portalFactory;
            _fortressFactory = fortressFactory;
            _poseRaycaster = raycaster;
            _touchInputChecker = inputChecker;
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
            switch (_currentState)
            {
                case PlacingObjectStates.PlacingPortal:
                case PlacingObjectStates.PlacingFortress:
                    return HandlePlacingObject();
                case PlacingObjectStates.Finished:
                    //TODO: Show UI to continue
                    return GameState.Playing;
                default:
                    return GameState.StayInState;
            }
        }

        private GameState HandlePlacingObject()
        {
            if (!_touchInputChecker.CheckScreenTouch(out Touch touch))
            {
                return GameState.StayInState;
            }

            if (_poseRaycaster.TryRaycastValidPose(touch.position, out Pose pose))
            {
                CreateObjectBasedOnCurrentState(pose);
            }

            return GameState.StayInState;
        }

        private void CreateObjectBasedOnCurrentState(Pose pose)
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
            }
        }


    }

    public enum PlacingObjectStates
    {
        PlacingPortal,
        PlacingFortress,
        Finished
    }
}
