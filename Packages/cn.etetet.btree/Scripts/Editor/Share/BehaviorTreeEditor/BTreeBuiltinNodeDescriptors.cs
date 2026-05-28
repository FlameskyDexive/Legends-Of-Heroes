using System.Collections.Generic;

namespace ET
{
    [BTreeNodeDescriptor]
    public sealed class BTreeLogNodeDescriptor : ABTreeNodeDescriptor
    {
        public override string TypeId => BTreeBuiltinNodeTypes.Log;

        public override BTreeNodeKind NodeKind => BTreeNodeKind.Action;

        public override string MenuPath => "Behaviors/Common/Log";

        public override string HandlerName => "Log";

        public override string Description => "Writes a log message and helps verify behavior tree execution order.";

        public override IReadOnlyList<BTreeNodeParameterDefinition> Parameters => new List<BTreeNodeParameterDefinition>
        {
            new()
            {
                Name = "message",
                DisplayName = "Message",
                ValueType = BTreeValueType.String,
                DefaultValue = new BTreeSerializedValue
                {
                    ValueType = BTreeValueType.String,
                    StringValue = "BehaviorTree Log",
                },
                EditorHint = BTreeNodeParameterEditorHint.MultilineText,
            },
        };
    }

    [BTreeNodeDescriptor]
    public sealed class BTreeSetBlackboardNodeDescriptor : ABTreeNodeDescriptor
    {
        public override string TypeId => BTreeBuiltinNodeTypes.SetBlackboard;

        public override BTreeNodeKind NodeKind => BTreeNodeKind.Action;

        public override string MenuPath => "Behaviors/Blackboard/Set Value";

        public override string HandlerName => "SetBlackboard";

        public override string Description => "Writes a value to the blackboard, or removes the key when Remove is enabled.";

        public override IReadOnlyList<BTreeNodeParameterDefinition> Parameters => new List<BTreeNodeParameterDefinition>
        {
            new()
            {
                Name = "key",
                DisplayName = "Blackboard Key",
                ValueType = BTreeValueType.String,
                DefaultValue = new BTreeSerializedValue
                {
                    ValueType = BTreeValueType.String,
                    StringValue = string.Empty,
                },
                EditorHint = BTreeNodeParameterEditorHint.BlackboardKey,
            },
            new()
            {
                Name = "remove",
                DisplayName = "Remove",
                ValueType = BTreeValueType.Boolean,
                DefaultValue = new BTreeSerializedValue
                {
                    ValueType = BTreeValueType.Boolean,
                    BoolValue = false,
                },
            },
            new()
            {
                Name = "value",
                DisplayName = "Value",
                ValueType = BTreeValueType.None,
                DefaultValue = new BTreeSerializedValue
                {
                    ValueType = BTreeValueType.String,
                    StringValue = string.Empty,
                },
            },
        };
    }

    [BTreeNodeDescriptor]
    public sealed class BTreeSetBlackboardIfMissingNodeDescriptor : ABTreeNodeDescriptor
    {
        public override string TypeId => BTreeBuiltinNodeTypes.SetBlackboardIfMissing;

        public override BTreeNodeKind NodeKind => BTreeNodeKind.Action;

        public override string MenuPath => "Behaviors/Blackboard/Set If Missing";

        public override string HandlerName => "SetBlackboardIfMissing";

        public override string Description => "Writes a value to the blackboard only when the key does not already exist.";

        public override IReadOnlyList<BTreeNodeParameterDefinition> Parameters => new List<BTreeNodeParameterDefinition>
        {
            new()
            {
                Name = "key",
                DisplayName = "Blackboard Key",
                ValueType = BTreeValueType.String,
                DefaultValue = new BTreeSerializedValue
                {
                    ValueType = BTreeValueType.String,
                    StringValue = string.Empty,
                },
                EditorHint = BTreeNodeParameterEditorHint.BlackboardKey,
            },
            new()
            {
                Name = "value",
                DisplayName = "Value",
                ValueType = BTreeValueType.None,
                DefaultValue = new BTreeSerializedValue
                {
                    ValueType = BTreeValueType.String,
                    StringValue = string.Empty,
                },
            },
        };
    }

    [BTreeNodeDescriptor]
    public sealed class BTreeBlackboardExistsNodeDescriptor : ABTreeNodeDescriptor
    {
        public override string TypeId => BTreeBuiltinNodeTypes.BlackboardExists;

        public override BTreeNodeKind NodeKind => BTreeNodeKind.Condition;

        public override string MenuPath => "Conditions/Blackboard/Is Set";

        public override string HandlerName => "BlackboardExists";

        public override string Description => "Checks whether the specified blackboard key currently exists.";

        public override IReadOnlyList<BTreeNodeParameterDefinition> Parameters => new List<BTreeNodeParameterDefinition>
        {
            new()
            {
                Name = "key",
                DisplayName = "Blackboard Key",
                ValueType = BTreeValueType.String,
                DefaultValue = new BTreeSerializedValue
                {
                    ValueType = BTreeValueType.String,
                    StringValue = string.Empty,
                },
                EditorHint = BTreeNodeParameterEditorHint.BlackboardKey,
            },
        };
    }

    [BTreeNodeDescriptor]
    public sealed class BTreeBlackboardCompareNodeDescriptor : ABTreeNodeDescriptor
    {
        public override string TypeId => BTreeBuiltinNodeTypes.BlackboardCompare;

        public override BTreeNodeKind NodeKind => BTreeNodeKind.Condition;

        public override string MenuPath => "Conditions/Blackboard/Compare";

        public override string HandlerName => "BlackboardCompare";

        public override string Description => "Reads a blackboard value and compares it against the configured operand.";

        public override IReadOnlyList<BTreeNodeParameterDefinition> Parameters => new List<BTreeNodeParameterDefinition>
        {
            new()
            {
                Name = "key",
                DisplayName = "Blackboard Key",
                ValueType = BTreeValueType.String,
                DefaultValue = new BTreeSerializedValue
                {
                    ValueType = BTreeValueType.String,
                    StringValue = string.Empty,
                },
                EditorHint = BTreeNodeParameterEditorHint.BlackboardKey,
            },
            new()
            {
                Name = "operator",
                DisplayName = "Operator",
                ValueType = BTreeValueType.Integer,
                DefaultValue = new BTreeSerializedValue
                {
                    ValueType = BTreeValueType.Integer,
                    IntValue = (int)BTreeCompareOperator.Equal,
                },
                EditorHint = BTreeNodeParameterEditorHint.CompareOperator,
            },
            new()
            {
                Name = "value",
                DisplayName = "Compare Value",
                ValueType = BTreeValueType.None,
                DefaultValue = new BTreeSerializedValue
                {
                    ValueType = BTreeValueType.String,
                    StringValue = string.Empty,
                },
            },
        };
    }
}
