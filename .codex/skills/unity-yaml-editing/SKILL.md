---
name: unity-yaml-editing
description: Unity YAML text serialization editing workflow. Use when Codex needs to directly inspect, modify, create, or repair Unity serialized YAML files such as .unity scenes, .prefab assets, .asset ScriptableObject/config files, .mat, .controller, or other text-serialized Unity assets, especially when AIBridge inspector/prefab/scene APIs do not support the requested Prefab, Scene, ScriptableObjectTable, or custom asset operation.
---

# Unity YAML Editing

Use this Skill only after considering Unity/AIBridge APIs. UnityYAML is fragile; direct text edits are the fallback for unsupported serialized asset operations, deterministic repair work, and rare text-serialized asset authoring that AIBridge/Unity APIs cannot express.

## Decision Order

1. Prefer `aibridge` Inspector/SerializedProperty commands for single readable field edits on scene objects, prefab assets, and `.asset` files.
2. Prefer `inspector set_properties` for small batched field edits on one target.
3. Prefer `aibridge-prefab-patch` for complex Prefab child/component/property/array/reference edits it supports.
4. Prefer Unity Editor scripts when generating high-level assets that Unity can create more safely than text.
5. Use direct UnityYAML editing when AIBridge/Unity APIs cannot express the operation, for example:
   - Scene `.unity` structure/object creation or unsupported scene edits.
   - New Prefab asset authoring, paired `.meta` creation, or Prefab/Variant structures not covered by `prefab patch`.
   - ScriptableObjectTable, custom ScriptableObject `.asset`, `.mat`, `.controller`, or other serialized assets requiring unsupported structural changes.
   - Repairing malformed text serialization while preserving existing IDs and references.

## Required Workflow

1. Read `references/unity-yaml-reference.md` before editing.
2. Inspect a same-project example for every Unity component/class you will create; never write component schemas from memory.
3. Make a small, reviewable patch; preserve ordering, indentation, document headers, anchors, `m_Script`, GUIDs, and local fileIDs unless a new object must be created.
4. For new assets, create or preserve the paired `.meta` file and ensure its GUID does not already appear in the project.
5. For new MonoBehaviour/ScriptableObject documents, resolve the script `.meta` GUID first and use it in `m_Script`.
6. For new documents, allocate local fileIDs that do not exist in the same file and update every owning reference consistently.
7. Validate with Unity import/compile and targeted AIBridge hierarchy/properties inspection when possible.

## Hard Rules

- Do not use generic YAML formatters or parsers to rewrite UnityYAML files.
- Do not change `%YAML`, `%TAG`, `--- !u!<classID> &<fileID>` headers, class IDs, GUIDs, or fileIDs without a clear reference update plan.
- Do not invent Unity schema fields. Copy shape from an existing nearby object of the same class/component/script when possible.
- Prefer decimal floats in hand edits. Avoid editing Unity-generated hexadecimal float values unless preserving existing text unchanged.
- Keep `.meta` GUIDs stable. Creating a new asset requires creating/retaining the paired `.meta` file through Unity when possible.
- When adding Prefab nodes/components, update all reciprocal references in the same patch: parent `m_Children`, child `m_Father`, `GameObject.m_Component`, and component `m_GameObject`.

## Reference

- `references/unity-yaml-reference.md`: detailed UnityYAML format, editing patterns, safety checks, and validation checklist.
