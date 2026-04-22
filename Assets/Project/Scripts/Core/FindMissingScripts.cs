using UnityEngine;

public class FindMissingScripts : MonoBehaviour
{
    [ContextMenu("Buscar Missing Scripts")]
    void Buscar()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        foreach (GameObject go in allObjects)
        {
            Component[] components = go.GetComponents<Component>();

            foreach (Component comp in components)
            {
                if (comp == null)
                {
                    Debug.Log("❌ Missing Script en: " + GetFullPath(go), go);
                }
            }
        }
    }

    string GetFullPath(GameObject obj)
    {
        string path = obj.name;
        while (obj.transform.parent != null)
        {
            obj = obj.transform.parent.gameObject;
            path = obj.name + "/" + path;
        }
        return path;
    }
}