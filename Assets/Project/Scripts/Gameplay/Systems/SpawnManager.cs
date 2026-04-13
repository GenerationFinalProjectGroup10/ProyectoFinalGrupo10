using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject relojPrefab;
    public GameObject piezaPrefab;

    public Transform puntoReloj;
    public Transform puntoPieza;

    void Start()
    {
        SpawnPuzzle();
    }

    void SpawnPuzzle()
    {
        Instantiate(relojPrefab, puntoReloj.position, Quaternion.identity);
        Instantiate(piezaPrefab, puntoPieza.position, Quaternion.identity);
    }
}