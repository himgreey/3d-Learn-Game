using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 将选角信息应用到 UI 头像控件。
/// 优先加载 Resources 中的 2D 头像；没有美术资源时从角色预制体离屏渲染。
/// </summary>
public static class RolePortraitUtil
{
    private const int CaptureSize = 256;
    private static readonly Vector3 CaptureWorldOffset = new Vector3(1000f, 1000f, 1000f);

    public static void Apply(RawImage rawImage, RoleInfo roleInfo)
    {
        if (rawImage == null || roleInfo == null) return;

        Texture texture = LoadPortraitTexture(roleInfo);
        if (texture != null)
        {
            rawImage.texture = texture;
        }
    }

    public static void Apply(Image image, RoleInfo roleInfo)
    {
        if (image == null || roleInfo == null) return;

        Sprite sprite = LoadPortraitSprite(roleInfo);
        if (sprite != null)
        {
            image.sprite = sprite;
            image.enabled = true;
        }
    }

    public static string GetPortraitPath(RoleInfo roleInfo)
    {
        if (roleInfo == null) return null;
        return string.IsNullOrEmpty(roleInfo.headIcon)
            ? $"UI/RoleHead/{roleInfo.id}"
            : roleInfo.headIcon;
    }

    public static Sprite LoadPortraitSprite(RoleInfo roleInfo)
    {
        if (roleInfo == null) return null;

        string path = GetPortraitPath(roleInfo);
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite != null) return sprite;

        Texture2D texture = Resources.Load<Texture2D>(path);
        if (texture != null)
        {
            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        }

        return Sprite.Create(CapturePortrait(roleInfo.res), new Rect(0, 0, CaptureSize, CaptureSize), new Vector2(0.5f, 0.5f));
    }

    public static Texture2D LoadPortraitTexture(RoleInfo roleInfo)
    {
        if (roleInfo == null) return null;

        string path = GetPortraitPath(roleInfo);
        Sprite sprite = Resources.Load<Sprite>(path);
        if (sprite != null) return sprite.texture;

        Texture2D texture = Resources.Load<Texture2D>(path);
        if (texture != null) return texture;

        return CapturePortrait(roleInfo.res);
    }

    private static Texture2D CapturePortrait(string resPath)
    {
        if (string.IsNullOrEmpty(resPath)) return null;

        GameObject prefab = Resources.Load<GameObject>(resPath);
        if (prefab == null) return null;

        PlayerObject previousPlayer = PlayerObject.Instance;

        GameObject role = Object.Instantiate(prefab, CaptureWorldOffset, Quaternion.identity);
        role.transform.localScale = Vector3.one;

        if (previousPlayer != null && PlayerObject.Instance != previousPlayer)
        {
            PlayerObject.ForceSetInstance(previousPlayer);
        }

        PlayerObject portraitPlayer = role.GetComponent<PlayerObject>();
        if (portraitPlayer != null)
        {
            Object.Destroy(portraitPlayer);
            if (previousPlayer != null)
            {
                PlayerObject.ForceSetInstance(previousPlayer);
            }
        }

        Bounds bounds = CalculateBounds(role);
        Vector3 center = bounds.center;
        float height = Mathf.Max(bounds.size.y, 0.5f);

        GameObject camObj = new GameObject("RolePortraitCamera");
        Camera cam = camObj.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.15f, 0.15f, 0.15f, 0f);
        cam.fieldOfView = 28f;
        cam.nearClipPlane = 0.1f;
        cam.farClipPlane = 20f;
        cam.allowHDR = false;
        cam.allowMSAA = false;

        Vector3 lookTarget = center + Vector3.up * height * 0.15f;
        camObj.transform.position = lookTarget + new Vector3(0f, height * 0.05f, height * 1.1f);
        camObj.transform.LookAt(lookTarget);

        RenderTexture rt = RenderTexture.GetTemporary(CaptureSize, CaptureSize, 24);
        cam.targetTexture = rt;
        cam.Render();

        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = rt;

        Texture2D result = new Texture2D(CaptureSize, CaptureSize, TextureFormat.RGB24, false);
        result.ReadPixels(new Rect(0, 0, CaptureSize, CaptureSize), 0, 0);
        result.Apply();

        RenderTexture.active = previous;
        cam.targetTexture = null;
        RenderTexture.ReleaseTemporary(rt);

        Object.Destroy(camObj);
        Object.Destroy(role);

        if (previousPlayer != null)
        {
            PlayerObject.ForceSetInstance(previousPlayer);
        }

        return result;
    }

    private static Bounds CalculateBounds(GameObject root)
    {
        Renderer[] renderers = root.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
        {
            return new Bounds(root.transform.position + Vector3.up, Vector3.one * 2f);
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        return bounds;
    }
}
