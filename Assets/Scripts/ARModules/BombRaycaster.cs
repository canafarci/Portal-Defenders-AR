using UnityEngine;

namespace PortalDefendersAR.ARModules
{
    public class BombRaycaster : IPoseRaycaster
    {
        private const int BOMB_LAYERMASK = 1 << 6;
        public bool TryRaycastValidPose(Vector2 screenPosition, out Pose pose)
        {
            bool raycastSuccessful = false;
            pose = default;


            Ray ray = Camera.main.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, BOMB_LAYERMASK))
            {
                raycastSuccessful = true;
            }

            return raycastSuccessful;
        }
    }
}
