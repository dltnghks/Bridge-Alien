
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public List<AK.Wwise.Event> TestEvent;

    private void Start()
    {
        AkUnitySoundEngine.PostEvent("Play_PostHold", gameObject);
    }
}
