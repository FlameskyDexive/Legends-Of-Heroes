// CREDITS:
// https://github.com/NewBloodInteractive/com.newblood.lighting-internals/tree/master
//

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using Object = UnityEngine.Object;

namespace ReferenceFinder
{
    internal static class RF_Lightmap
    {
        public static IEnumerable<Texture> Read(LightingDataAsset source)
        {
            string json = EditorJsonUtility.ToJson(source);
            var result = new LightingDataAssetRoot();
            EditorJsonUtility.FromJsonOverwrite(json, result);

            foreach (var item in result.LightingDataAsset.m_Lightmaps)
            {
                if (item.lightmap != null) yield return item.lightmap;
                if (item.dirLightmap != null) yield return item.dirLightmap;
                if (item.shadowMask != null) yield return item.shadowMask;
            }

            foreach (var item in result.LightingDataAsset.m_AOTextures)
            {
                if (item != null) yield return item;
            }

            foreach (var item in result.LightingDataAsset.m_BakedReflectionProbeCubemaps)
            {
                if (item != null) yield return item;
            }
        }
        
        [Serializable]
        private struct EnlightenRendererInformation
        {
            public Object renderer;
            public Vector4 dynamicLightmapSTInSystem;
            public int systemId;
            public Hash128 instanceHash;
            public Hash128 geometryHash;
        }

        [Serializable]
        private struct EnlightenSystemAtlasInformation
        {
            public int atlasSize;
            public Hash128 atlasHash;
            public int firstSystemId;
        }

        [Serializable]
        private struct EnlightenTerrainChunksInformation
        {
            public int firstSystemId;
            public int numChunksInX;
            public int numChunksInY;
        }

        [Serializable]
        private struct EnlightenSystemInformation
        {
            public int rendererIndex;
            public int rendererSize;
            public int atlasIndex;
            public int atlasOffsetX;
            public int atlasOffsetY;
            public Hash128 inputSystemHash;
            public Hash128 radiositySystemHash;
        }

        [Serializable]
        private sealed class EnlightenSceneMapping
        {
            [SerializeField]
            EnlightenRendererInformation[] m_Renderers;

            [SerializeField]
            EnlightenSystemInformation[] m_Systems;

            [SerializeField]
            Hash128[] m_Probesets;

            [SerializeField]
            EnlightenSystemAtlasInformation[] m_SystemAtlases;

            [SerializeField]
            EnlightenTerrainChunksInformation[] m_TerrainChunks;

            public EnlightenRendererInformation[] renderers
            {
                get => m_Renderers;
                set => m_Renderers = value;
            }

            public EnlightenSystemInformation[] systems
            {
                get => m_Systems;
                set => m_Systems = value;
            }

            public Hash128[] probesets
            {
                get => m_Probesets;
                set => m_Probesets = value;
            }

            public EnlightenSystemAtlasInformation[] systemAtlases
            {
                get => m_SystemAtlases;
                set => m_SystemAtlases = value;
            }

            public EnlightenTerrainChunksInformation[] terrainChunks
            {
                get => m_TerrainChunks;
                set => m_TerrainChunks = value;
            }
        }


        [Serializable]
        private sealed class LightingDataAssetRoot
        {
            public SerializedData LightingDataAsset;

            [Serializable]
            public struct SerializedData
            {
                public int serializedVersion;
                public SceneAsset m_Scene;
                public LightmapData[] m_Lightmaps;
                public Texture2D[] m_AOTextures;
                public string[] m_LightmapsCacheFiles;
                public LightProbes m_LightProbes;
                public int m_LightmapsMode;
                public SphericalHarmonicsL2 m_BakedAmbientProbeInLinear;
                public RendererData[] m_LightmappedRendererData;
                public SceneObjectIdentifier[] m_LightmappedRendererDataIDs;
                public EnlightenSceneMapping m_EnlightenSceneMapping;
                public SceneObjectIdentifier[] m_EnlightenSceneMappingRendererIDs;
                public SceneObjectIdentifier[] m_Lights;
                public LightBakingOutput[] m_LightBakingOutputs;
                public string[] m_BakedReflectionProbeCubemapCacheFiles;
                public Texture[] m_BakedReflectionProbeCubemaps;
                public SceneObjectIdentifier[] m_BakedReflectionProbes;
                public byte[] m_EnlightenData;
                public int m_EnlightenDataVersion;
            }
        }

        [Serializable]
        private struct SceneObjectIdentifier : IEquatable<SceneObjectIdentifier>
        {
            public long targetObject;

            public long targetPrefab;

            public SceneObjectIdentifier(GlobalObjectId id)
            {
                if (id.identifierType != 2) throw new ArgumentException("GlobalObjectId must refer to a scene object.", nameof(id));

                targetObject = unchecked((long)id.targetObjectId);
                targetPrefab = unchecked((long)id.targetPrefabId);
            }

            public bool Equals(SceneObjectIdentifier other)
            {
                return targetObject == other.targetObject && targetPrefab == other.targetPrefab;
            }

            // public GlobalObjectId ToGlobalObjectId(SceneAsset scene)
            // {
            //     return ToGlobalObjectId(AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(scene)));
            // }
            //
            // public GlobalObjectId ToGlobalObjectId(Scene scene)
            // {
            //     return ToGlobalObjectId(AssetDatabase.GUIDFromAssetPath(scene.path));
            // }
            //
            // public GlobalObjectId ToGlobalObjectId(GUID sceneGuid)
            // {
            //     GlobalObjectId id;
            //     GlobalObjectId.TryParse($"GlobalObjectId_V1-2-{sceneGuid}-{unchecked((ulong)targetObject)}-{unchecked((ulong)targetPrefab)}", out id);
            //     return id;
            // }
            //
            // public static Object SceneObjectIdentifierToObjectSlow(SceneAsset scene, SceneObjectIdentifier id)
            // {
            //     return SceneObjectIdentifierToObjectSlow(AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(scene)), id);
            // }
            //
            // public static Object SceneObjectIdentifierToObjectSlow(Scene scene, SceneObjectIdentifier id)
            // {
            //     return SceneObjectIdentifierToObjectSlow(AssetDatabase.GUIDFromAssetPath(scene.path), id);
            // }
            //
            // public static Object SceneObjectIdentifierToObjectSlow(GUID sceneGuid, SceneObjectIdentifier id)
            // {
            //     return GlobalObjectId.GlobalObjectIdentifierToObjectSlow(id.ToGlobalObjectId(sceneGuid));
            // }
            //
            // public static void SceneObjectIdentifiersToObjectsSlow(SceneAsset scene, SceneObjectIdentifier[] identifiers, Object[] outputObjects)
            // {
            //     SceneObjectIdentifiersToObjectsSlow(AssetDatabase.GUIDFromAssetPath(AssetDatabase.GetAssetPath(scene)), identifiers, outputObjects);
            // }
            //
            // public static void SceneObjectIdentifiersToObjectsSlow(Scene scene, SceneObjectIdentifier[] identifiers, Object[] outputObjects)
            // {
            //     SceneObjectIdentifiersToObjectsSlow(AssetDatabase.GUIDFromAssetPath(scene.path), identifiers, outputObjects);
            // }

            // public static void SceneObjectIdentifiersToObjectsSlow(GUID sceneGuid, SceneObjectIdentifier[] identifiers, Object[] outputObjects)
            // {
            //     var globalIdentifiers = new GlobalObjectId[identifiers.Length];
            //
            //     for (int i = 0; i < identifiers.Length; i++)
            //         globalIdentifiers[i] = identifiers[i].ToGlobalObjectId(sceneGuid);
            //
            //     GlobalObjectId.GlobalObjectIdentifiersToObjectsSlow(globalIdentifiers, outputObjects);
            // }
            //
            // public static void GetSceneObjectIdentifiersSlow(Object[] objects, SceneObjectIdentifier[] outputIdentifiers)
            // {
            //     var globalIdentifiers = new GlobalObjectId[outputIdentifiers.Length];
            //     GlobalObjectId.GetGlobalObjectIdsSlow(objects, globalIdentifiers);
            //
            //     for (int i = 0; i < outputIdentifiers.Length; i++)
            //     {
            //         outputIdentifiers[i] = new SceneObjectIdentifier(globalIdentifiers[i]);
            //     }
            // }
        }

        [Serializable]
        private struct LightBakingOutput
        {
            public int serializedVersion;
            public int probeOcclusionLightIndex;
            public int occlusionMaskChannel;
            public LightmapBakeMode lightmapBakeMode;
            public bool isBaked;

            [Serializable]
            public struct LightmapBakeMode
            {
                public LightmapBakeType lightmapBakeType;
                public MixedLightingMode mixedLightingMode;
            }
        }

        [Serializable]
        private struct RendererData
        {
            public Mesh uvMesh;
            public Vector4 terrainDynamicUVST;
            public Vector4 terrainChunkDynamicUVST;
            public Vector4 lightmapST;
            public Vector4 lightmapSTDynamic;
            public ushort lightmapIndex;
            public ushort lightmapIndexDynamic;
            public Hash128 explicitProbeSetHash;
        }

        [Serializable]
        private struct LightmapData
        {
            [SerializeField]
            Texture2D m_Lightmap;

            [SerializeField]
            Texture2D m_DirLightmap;

            [SerializeField]
            Texture2D m_ShadowMask;

            public Texture2D lightmap
            {
                get => m_Lightmap;
                set => m_Lightmap = value;
            }

            public Texture2D dirLightmap
            {
                get => m_DirLightmap;
                set => m_DirLightmap = value;
            }

            public Texture2D shadowMask
            {
                get => m_ShadowMask;
                set => m_ShadowMask = value;
            }
        }
    }
}
