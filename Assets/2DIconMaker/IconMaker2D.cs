using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class IconMaker2D : MonoBehaviour {
    private Camera m_Camera;
    
    [ContextMenu("TakeScreenshot")]
    public void TakeScreenshot() {
        
        string filename = "icon.png";
        string filepath = Path.GetFullPath("Assets\\2dIconMaker\\" + filename);
        Debug.Log(filepath);
        int size = 1024;
        m_Camera = GetComponent<Camera>();

        RenderTexture renderTexture = new RenderTexture(size, size, 24);
        m_Camera.targetTexture= renderTexture;
        Texture2D screenshot = new Texture2D(size, size, TextureFormat.RGBA32, false);
        
        m_Camera.Render();
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, size, size), 0, 0);
        m_Camera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(renderTexture);
        byte[] data = screenshot.EncodeToPNG();
        System.IO.File.WriteAllBytes(filepath, data);

    }
}
