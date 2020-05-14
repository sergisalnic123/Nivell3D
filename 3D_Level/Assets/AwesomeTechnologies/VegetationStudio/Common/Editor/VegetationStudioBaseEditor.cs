using UnityEditor;
using UnityEngine;

namespace AwesomeTechnologies
{
    public class VegetationStudioBaseEditor : Editor
    {
        public bool LargeLogo = false;
        public bool ShowLogo = true;
        public bool IsScriptableObject = false;
        private Texture2D _logoTexture;
        private Texture2D _logoTextureSmall;
        public string OverrideLogoTextureName;
        private Texture2D _overrideLogoTextureSmall;
        public GUIStyle LabelStyle;
        public string HelpTopic = "";

        public virtual void Awake()
        {
            LabelStyle = new GUIStyle("Label") {fontStyle = FontStyle.Italic}; 
            if (EditorGUIUtility.isProSkin)
            {
                LabelStyle.normal.textColor = new Color(1f, 1f, 1f);
                _logoTexture = (Texture2D)Resources.Load("VegetationStudioSplashLarge", typeof(Texture2D));
                _logoTextureSmall = (Texture2D)Resources.Load("VegetationStudioSplashSmall", typeof(Texture2D));


            }
            else
            {
                LabelStyle.normal.textColor = new Color(0f, 0f, 0f);
                _logoTexture = (Texture2D)Resources.Load("VegetationStudioSplashLarge", typeof(Texture2D));
                _logoTextureSmall = (Texture2D)Resources.Load("VegetationStudioSplashSmall", typeof(Texture2D));
            }
        }

        public override void OnInspectorGUI()
        {
            if (!ShowLogo) return;

            EditorGUIUtility.labelWidth = 200;

            Texture2D selectedLogoTexture = _logoTextureSmall;
            if (LargeLogo) selectedLogoTexture = _logoTexture;

            if (OverrideLogoTextureName != "")
            {
                if (_overrideLogoTextureSmall == null)
                {
                    _overrideLogoTextureSmall = (Texture2D)Resources.Load(OverrideLogoTextureName, typeof(Texture2D));
                }
                if(_overrideLogoTextureSmall) selectedLogoTexture = _overrideLogoTextureSmall;
            }
            GUILayoutUtility.GetRect(1, 3, GUILayout.ExpandWidth(false));
            Rect space = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Height(selectedLogoTexture.height));
            float width = space.width;

            space.xMin = (width - selectedLogoTexture.width +18 ) / 2;
            if (space.xMin < 0) space.xMin = 0;

            space.width = selectedLogoTexture.width;
            space.height = selectedLogoTexture.height;
            GUI.DrawTexture(space, selectedLogoTexture, ScaleMode.ScaleToFit, true, 0);

            if (LargeLogo)
            {
                EditorGUILayout.LabelField("Version: 1.5.2.0", LabelStyle);
                EditorGUILayout.LabelField("", LabelStyle);
            }

            GUILayoutUtility.GetRect(1, 3, GUILayout.ExpandWidth(false));                  
        }  
    }
}