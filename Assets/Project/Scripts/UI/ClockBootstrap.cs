using UnityEngine;

public class ClockBootstrap : MonoBehaviour
{
    public GameObject clockManagerPrefab;

    void Awake()
    {
        if (ClockManager.Instance == null)
        {
            Instantiate(clockManagerPrefab);
            Debug.Log("🕰️ ClockManager creado automaticamente");
        }
    }
}