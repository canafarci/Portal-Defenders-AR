using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PortalDefendersAR.GameInput
{
    public class TouchInputChecker : ITouchInputChecker
    {
        public bool CheckTouchedScreen(out Touch touch)
        {
            touch = default;

            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                touch = Input.GetTouch(0);
                return true;
            }

            return false;
        }
    }

    public interface ITouchInputChecker
    {
        public bool CheckTouchedScreen(out Touch touch);
    }
}
