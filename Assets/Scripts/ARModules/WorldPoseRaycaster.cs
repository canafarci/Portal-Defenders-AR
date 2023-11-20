using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

namespace PortalDefendersAR.ARModules
{
    public class WorldPoseRaycaster : IPoseRaycaster
    {
        private ARRaycastManager _arRaycastManager;

        private WorldPoseRaycaster(ARRaycastManager arRaycastManager)
        {
            _arRaycastManager = arRaycastManager;
        }

        public bool TryRaycastValidPose(Vector2 screenPosition, out Pose pose)
        {
            bool raycastSuccessful = false;

            List<ARRaycastHit> hits = new();
            if (_arRaycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon))
            {
                pose = hits[0].pose;
                raycastSuccessful = true;
            }
            else
            {
                pose = default;
            }

            return raycastSuccessful;
        }
    }

    public interface IPoseRaycaster
    {
        public bool TryRaycastValidPose(Vector2 screenPosition, out Pose pose);
    }
}
