using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
namespace ReferenceFinder
{
    public static class RF_Addressable
    {
        [Serializable]
        public class AddressInfo
        {
            public string address;
            public string bundleGroup;
            public HashSet<string> assetGUIDs;
            public HashSet<string> childGUIDs;
        }
        
        public enum ASMStatus
        {
            None,
            AsmNotFound,
            TypeNotFound,
            FieldNotFound,
            AsmOK
        }

        public enum ProjectStatus
        {
            None,
            NoSettings,
            NoGroup,
            Ok
        }

        private static Assembly asm;
        private static Type addressableAssetGroupType;
        private static Type addressableAssetEntryType;
        
        private static PropertyInfo entriesProperty;
        private static PropertyInfo groupNameProperty;
        private static PropertyInfo addressProperty;
        private static PropertyInfo guidProperty;
        private static PropertyInfo settingsProperty;
        private static PropertyInfo groupsProperty;

        public static bool isOk => asmStatus == ASMStatus.AsmOK && projectStatus == ProjectStatus.Ok;
        
        static RF_Addressable()
        {
            Scan();
        }
        
        public static void Scan()
        {
            asm = GetAssembly();
            if (asm == null)
            {
                asmStatus = ASMStatus.AsmNotFound;
                return;
            }
            
            Type addressableSettingsType = GetAddressableType("UnityEditor.AddressableAssets.Settings.AddressableAssetSettings");
            Type addressableSettingsDefaultObjectType = GetAddressableType("UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject");
            addressableAssetGroupType = GetAddressableType("UnityEditor.AddressableAssets.Settings.AddressableAssetGroup");
            addressableAssetEntryType = GetAddressableType("UnityEditor.AddressableAssets.Settings.AddressableAssetEntry");

            if (addressableSettingsType == null || addressableSettingsDefaultObjectType == null || addressableAssetGroupType == null || addressableAssetEntryType == null)
            {
                asmStatus = ASMStatus.TypeNotFound;
                return;
            }

            entriesProperty = addressableAssetGroupType.GetProperty("entries", BindingFlags.Public | BindingFlags.Instance);
            groupNameProperty = addressableAssetGroupType.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
            addressProperty = addressableAssetEntryType.GetProperty("address", BindingFlags.Public | BindingFlags.Instance);
            guidProperty = addressableAssetEntryType.GetProperty("guid", BindingFlags.Public | BindingFlags.Instance);
            settingsProperty = addressableSettingsDefaultObjectType.GetProperty("Settings", BindingFlags.Public | BindingFlags.Static);
            groupsProperty = addressableSettingsType.GetProperty("groups", BindingFlags.Public | BindingFlags.Instance);

            if (entriesProperty == null || groupNameProperty == null || addressProperty == null || guidProperty == null)
            {
                asmStatus = ASMStatus.FieldNotFound;
                return;
            }

            asmStatus = ASMStatus.AsmOK;
            projectStatus = ProjectStatus.None;
        }

        public static ASMStatus asmStatus { get; private set; }
        public static ProjectStatus projectStatus { get; private set; }
        
        private static Assembly GetAssembly()
        {
            const string DLL = "Unity.Addressables.Editor";
            Assembly[] allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly item in allAssemblies)
            {
                if (item.GetName().Name != DLL) continue;
                // Debug.LogWarning($"Found: {item.GetName().Name}");
                return item;
            }

            // Debug.LogWarning($"ASSEMBLY NOT FOUND! <{DLL}");
            return null;
        }

        private static Type GetAddressableType(string typeName)
        {
            return asm == null ? null : asm.GetType(typeName);
        }
        
        /// <summary>
        /// Get a map between address -> AddressInfo (assetGUIDs + childGUIDs)
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, AddressInfo> GetAddresses()
        {
            if (asmStatus != ASMStatus.AsmOK) return null;
            
            // Get the AddressableAssetSettings instance
            object settings = settingsProperty?.GetValue(null);

            if (settings == null)
            {
                // Debug.LogWarning("Addressable Asset Settings could not be found.");
                projectStatus = ProjectStatus.NoSettings;
                return null;
            }

            var addresses = new Dictionary<string, AddressInfo>();
            var groups = groupsProperty?.GetValue(settings) as IEnumerable<object>;

            if (groups == null)
            {
                projectStatus = ProjectStatus.NoGroup; // Debug.LogWarning("No groups found in Addressable Asset Settings.");
                return null;
            }

            projectStatus = ProjectStatus.Ok;

            // Loop through each group
            foreach (object group in groups)
            {
                if (group == null || addressableAssetGroupType == null || addressableAssetEntryType == null) continue;

                // Get the group's 'entries' property
                var entries = entriesProperty?.GetValue(group) as IEnumerable<object>;

                if (entries == null) continue;

                // Get the group's 'Name' property
                var groupName = groupNameProperty?.GetValue(group)?.ToString();

                // Loop through each entry in the group
                foreach (object entry in entries)
                {
                    if (entry == null) continue;

                    // Get the entry's 'address' and 'guid' properties
                    var address = addressProperty?.GetValue(entry)?.ToString();
                    var guid = guidProperty?.GetValue(entry)?.ToString();

                    if (address == null || guid == null) continue;

                    if (!addresses.TryGetValue(address, out var fr2Address))
                    {
                        // New address entry
                        fr2Address = new AddressInfo
                        {
                            address = address,
                            bundleGroup = groupName,
                            assetGUIDs = new HashSet<string>(),
                            childGUIDs = new HashSet<string>()
                        };

                        addresses.Add(address, fr2Address);
                    }

                    if (fr2Address.assetGUIDs.Add(guid)) // folder?
                    {
                        AppendChildGUIDs(fr2Address.childGUIDs, guid);    
                    }
                }
            }

            return addresses;
        }
        
        private static void AppendChildGUIDs(HashSet<string> h, string guid)
        {
            string folderPath = AssetDatabase.GUIDToAssetPath(guid);
            if (!AssetDatabase.IsValidFolder(folderPath)) return;
            string[] allGUIDs = AssetDatabase.FindAssets("*", new[] { folderPath });
            foreach (string child in allGUIDs)
            {
                var asset = RF_Cache.Api.Get(child);
                if (asset.IsExcluded || asset.IsMissing || asset.IsScript || asset.type == RF_AssetType.UNKNOWN) continue;
                if (asset.inEditor || asset.inResources || asset.inStreamingAsset || asset.inPlugins) continue;
                if (asset.extension == ".asmdef") continue;
                if (asset.extension == ".wlt") continue;
                if (asset.IsFolder) continue;
                h.Add(child);
            }
        }
    }
}
