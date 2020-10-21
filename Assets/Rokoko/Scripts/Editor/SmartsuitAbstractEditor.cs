using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/// @cond nodoc
public class SmartsuitAbstractEditor : Editor {
    protected Texture2D livePanel;
    protected bool notesToggle = false;
    protected GUIStyle warningStyle = new GUIStyle();
    protected GUIStyle livePanelStyle = new GUIStyle();
    

    protected void OnEnable()
    {
        Color col = EditorGUIUtility.isProSkin
             ? (Color)new Color32(56, 56, 56, 255)
             : (Color)new Color32(194, 194, 194, 255);
        livePanel = MakeTex(1, 1, col * 0.8f);
        
        livePanelStyle.normal.background = livePanel;
        warningStyle.richText = true;
        warningStyle.wordWrap = true;
        
    }

    protected Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];

        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }
}
/// @endcond