using UnityEngine;

namespace PortalDefendersAR.ARModules
{
    public class DragPlaneRaycaster : IPoseRaycaster
    {
        private const int DRAG_PLANE_LAYERMASK = 1 << 7;
        public bool TryRaycastValidPose(Vector2 screenPosition, out Pose pose)
        {
            bool raycastSuccessful = false;
            pose = default;


            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, DRAG_PLANE_LAYERMASK))
            {
                raycastSuccessful = true;
                pose.position = hit.point;
            }

            return raycastSuccessful;
        }
    }
}
