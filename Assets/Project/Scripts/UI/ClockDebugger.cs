using UnityEngine;

public class ClockDebugger : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ClockManager.Instance.ForceChime(1);
        }
    }
}