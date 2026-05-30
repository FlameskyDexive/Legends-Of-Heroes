#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using System.Reflection;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using static VHierarchy.Libs.VUtils;
using static VHierarchy.Libs.VGUI;
// using static VTools.VDebug;


namespace VHierarchy
{
    public class VHierarchyPalette : ScriptableObject
    {
        public List<Color> colors = new();

        public bool colorsEnabled;

        public float colorSaturation = 1;
        public float colorBrightness = 1;
        public bool colorGradientsEnabled = true;

        public void ResetColors()
        {
            colors.Clear();

            for (int i = 0; i < colorsCount; i++)
                colors.Add(GetDefaultColor(i));

            colorsEnabled = true;

            this.Dirty();

        }

        public static Color GetDefaultColor(int colorIndex)
        {
            Color color = default;

            void grey()
            {
                if (colorIndex >= greyColorsCount) return;

#if UNITY_2022_1_OR_NEWER
                color = Greyscale(isDarkTheme ? .16f : .9f);
#else
                color = Greyscale(isDarkTheme ? .315f : .9f);
#endif

            }
            void rainbowDarkTheme()
            {
                if (colorIndex < greyColorsCount) return;
                if (!isDarkTheme) return;

                color = ColorUtils.HSLToRGB((colorIndex - greyColorsCount.ToFloat()) / rainbowColorsCount, .45f, .35f);

                if (colorIndex == 1)
                    color *= 1.2f;

                if (colorIndex == 2)
                    color *= 1.1f;

                if (colorIndex == 6)
                    color *= 1.35f;

                if (colorIndex == 7)
                    color *= 1.3f;

                if (colorIndex == 8)
                    color *= 1.05f;


                color.a = .1f;

            }
            void rainbowLightTheme()
            {
                if (colorIndex < greyColorsCount) return;
                if (isDarkTheme) return;

                color = ColorUtils.HSLToRGB((colorIndex - greyColorsCount.ToFloat()) / rainbowColorsCount, .62f, .8f);

                color.a = .1f;

            }

            grey();
            rainbowDarkTheme();
            rainbowLightTheme();

            return color;

        }

        public static int greyColorsCount = 1;
        public static int rainbowColorsCount = 8;
        public static int colorsCount => greyColorsCount + rainbowColorsCount;




        public List<IconRow> iconRows = new();

        [System.Serializable]
        public class IconRow
        {
            public List<string> builtinIcons = new(); // names
            public List<string> customIcons = new(); // names or guids

            public bool enabled = true;

            public bool isCustom => !builtinIcons.Any() || customIcons.Any();
            public bool isEmpty => !builtinIcons.Any() && !customIcons.Any();
            public int iconCount => builtinIcons.Count + customIcons.Count;

            public IconRow(string[] builtinIcons) => this.builtinIcons = builtinIcons.ToList();
            public IconRow() { }

        }

        public void ResetIcons()
        {
            iconRows.Clear();

            iconRows.Add(new IconRow(new[]
            {
                "Folder Icon",
                "Canvas Icon",
                "AvatarMask On Icon",
                "cs Script Icon",
                "StandaloneInputModule Icon",
                "EventSystem Icon",
                "Terrain Icon",
                "ScriptableObject Icon",

            }));
            iconRows.Add(new IconRow(new[]
            {
                "Camera Icon",
                "ParticleSystem Icon",
                "TrailRenderer Icon",
                "Material Icon",
                "ReflectionProbe Icon",

            }));
            iconRows.Add(new IconRow(new[]
            {
                "Light Icon",
                "DirectionalLight Icon",
                "LightmapParameters Icon",
                "LightProbes Icon",

            }));
            iconRows.Add(new IconRow(new[]
            {
                "Rigidbody Icon",
                "BoxCollider Icon",
                "SphereCollider Icon",
                "CapsuleCollider Icon",
                "WheelCollider Icon",
                "MeshCollider Icon",

            }));
            iconRows.Add(new IconRow(new[]
            {
                "AudioSource Icon",
                "AudioClip Icon",
                "AudioListener Icon",
                "AudioEchoFilter Icon",
                "AudioReverbZone Icon",

            }));
            iconRows.Add(new IconRow(new[]
            {
                "PreMatCube",
                "PreMatSphere",
                "PreMatCylinder",
                "PreMatQuad",
                "Favorite",
                #if UNITY_2021_3_OR_NEWER
                "Settings Icon",
                #endif

            }));

            this.Dirty();

        }




        [ContextMenu("Export palette")]
        public void Export()
        {
            var packagePath = EditorUtility.SaveFilePanel("Export vHierarchy Palette", "", this.GetPath().GetFilename(withExtension: false), "unitypackage");

            var iconPaths = iconRows.SelectMany(r => r.customIcons).Select(r => r.ToPath()).Where(r => !r.IsNullOrEmpty());

            AssetDatabase.ExportPackage(iconPaths.Append(this.GetPath()).ToArray(), packagePath);

            EditorUtility.RevealInFinder(packagePath);

        }




        void Reset() { ResetColors(); ResetIcons(); }

    }
}
#endif