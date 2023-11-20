using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PortalDefendersAR.GameInput
{
    public class DragInputChecker : ITouchInputChecker
    {
        public bool CheckScreenTouch(out Touch touch)
        {
            touch = default;

            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                return true;
            }

            return false;
        }
    }
}
