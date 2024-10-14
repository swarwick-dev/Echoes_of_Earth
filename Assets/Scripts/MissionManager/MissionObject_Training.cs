using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionObject_Training : MonoBehaviour
{
    public static event Action OnTargetKilled;

    public void InvokeOnTargetKilled() => OnTargetKilled?.Invoke();
}
