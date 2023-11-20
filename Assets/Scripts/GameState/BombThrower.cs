using System.Collections;
using System.Collections.Generic;
using PortalDefendersAR.Creation;
using UnityEngine;

namespace PortalDefendersAR.GameStates
{
    public class BombThrower
    {
        public void DragBomb(Bomb bomb, Pose pose)
        {
            bomb.Rigidbody.MovePosition(pose.position);
        }

        public void Throw(Bomb bomb)
        {
            bomb.Transform.parent = null;
            bomb.Rigidbody.isKinematic = false;
            bomb.Rigidbody.AddForce(Camera.main.transform.forward * 5f, ForceMode.Impulse);
        }
    }
}
