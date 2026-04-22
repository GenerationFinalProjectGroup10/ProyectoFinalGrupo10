using UnityEngine;
using UnityEditor;
using System.IO;

public class IconGeneratorWindow : EditorWindow
{
    public GameObject[] prefabs;
    public int resolution = 512;

    Camera renderCam;
    RenderTexture rt;

    [MenuItem("Tools/Icon Generator")]
    public static void OpenWindow()
    {
        GetWindow<IconGeneratorWindow>("Icon Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("📦 Prefabs para generar íconos", EditorStyles.boldLabel);
        SerializedObject so = new SerializedObject(this);
        SerializedProperty list = so.FindProperty("prefabs");
        EditorGUILayout.PropertyField(list, true);
        so.ApplyModifiedProperties();

        resolution = EditorGUILayout.IntSlider("Resolución", resolution, 128, 2048);

        if (GUILayout.Button("GENERAR ICONOS"))
        {
            GenerateIcons();
        }
    }

    void GenerateIcons()
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning("No hay prefabs asignados.");
            return;
        }

        SetupCamera();

        foreach (GameObject prefab in prefabs)
        {
            if (prefab == null) continue;

            CreateIcon(prefab);
        }

        Cleanup();
        Debug.Log("Íconos generados!");
    }

    void SetupCamera()
    {
        GameObject camObj = new GameObject("IconCamera");
        renderCam = camObj.AddComponent<Camera>();

        renderCam.backgroundColor = new Color(0,0,0,0);
        renderCam.clearFlags = CameraClearFlags.SolidColor;
        renderCam.orthographic = false;

        rt = new RenderTexture(resolution, resolution, 16, RenderTextureFormat.ARGB32);
        renderCam.targetTexture = rt;
    }

    void CreateIcon(GameObject prefab)
    {
        GameObject instance = Instantiate(prefab);
        instance.transform.position = new Vector3(0, 0, 0);
        instance.transform.rotation = Quaternion.identity;

        Bounds bounds = CalculateBounds(instance);
        float size = Mathf.Max(bounds.size.x, bounds.size.y, bounds.size.z);

        renderCam.transform.position = bounds.center + new Vector3(0, 0, size * 2f);
        renderCam.transform.LookAt(bounds.center);

        // iluminación simple
        GameObject lightObj = new GameObject("TempLight");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.transform.rotation = Quaternion.Euler(50, 30, 0);

        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = rt;

        renderCam.Render();

        Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        tex.Apply();

        string path = "Assets/IconGenerator/Icons/";
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        string file = path + prefab.name + ".png";
        File.WriteAllBytes(file, tex.EncodeToPNG());

        DestroyImmediate(instance);
        DestroyImmediate(lightObj);
        RenderTexture.active = currentRT;

        AssetDatabase.Refresh();
    }

    Bounds CalculateBounds(GameObject go)
    {
        Renderer[] renderers = go.GetComponentsInChildren<Renderer>();
        Bounds b = renderers[0].bounds;
        foreach (Renderer r in renderers) b.Encapsulate(r.bounds);
        return b;
    }

    void Cleanup()
    {
        if (renderCam != null) DestroyImmediate(renderCam.gameObject);
    }
}