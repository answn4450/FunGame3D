using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopAuraController : Structure
{
    public void StopPlayerBlock()
    {
        GroundController ground = Tools.GetInstance().GetUnderGround(transform);
        if (ground)
            ground.temporailyStop = true;
    }
}
