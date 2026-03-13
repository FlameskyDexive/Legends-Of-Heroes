using System.Collections.Generic;

namespace ET
{
    [BehaviorTreeNodeDescriptor]
    public sealed class BehaviorTreeLogNodeDescriptor : ABehaviorTreeNodeDescriptor
    {
        public override string TypeId => BehaviorTreeBuiltinNodeTypes.Log;

        public override BehaviorTreeNodeKind NodeKind => BehaviorTreeNodeKind.Action;

        public override string MenuPath => "Behaviors/Common/Log";

        public override string HandlerName => "Log";

        public override string Description => "输出一条调试日志，适合验证树执行顺序。";

        public override IReadOnlyList<BehaviorTreeNodeParameterDefinition> Parameters => new List<BehaviorTreeNodeParameterDefinition>
        {
            new()
            {
                Name = "message",
                DisplayName = "Message",
                ValueType = BehaviorTreeValueType.String,
                DefaultValue = new BehaviorTreeSerializedValue
                {
                    ValueType = BehaviorTreeValueType.String,
                    StringValue = "BehaviorTree Log",
                },
                EditorHint = BehaviorTreeNodeParameterEditorHint.MultilineText,
            },
        };
    }

    [BehaviorTreeNodeDescriptor]
    public sealed class BehaviorTreeSetBlackboardNodeDescriptor : ABehaviorTreeNodeDescriptor
    {
        public override string TypeId => BehaviorTreeBuiltinNodeTypes.SetBlackboard;

        public override BehaviorTreeNodeKind NodeKind => BehaviorTreeNodeKind.Action;

        public override string MenuPath => "Behaviors/Blackboard/Set Value";

        public override string HandlerName => "SetBlackboard";

        public override string Description => "写入或删除黑板键值。";

        public override IReadOnlyList<BehaviorTreeNodeParameterDefinition> Parameters => new List<BehaviorTreeNodeParameterDefinition>
        {
            new()
            {
                Name = "key",
                DisplayName = "Blackboard Key",
                ValueType = BehaviorTreeValueType.String,
                DefaultValue = new BehaviorTreeSerializedValue
                {
                    ValueType = BehaviorTreeValueType.String,
                    StringValue = string.Empty,
                },
                EditorHint = BehaviorTreeNodeParameterEditorHint.BlackboardKey,
            },
            new()
            {
                Name = "remove",
                DisplayName = "Remove",
                ValueType = BehaviorTreeValueType.Boolean,
                DefaultValue = new BehaviorTreeSerializedValue
                {
                    ValueType = BehaviorTreeValueType.Boolean,
                    BoolValue = false,
                },
            },
            new()
            {
                Name = "value",
                DisplayName = "Value",
                ValueType = BehaviorTreeValueType.None,
                DefaultValue = new BehaviorTreeSerializedValue
                {
                    ValueType = BehaviorTreeValueType.String,
                    StringValue = string.Empty,
                },
            },
        };
    }

    [BehaviorTreeNodeDescriptor]
    public sealed class BehaviorTreeBlackboardExistsNodeDescriptor : ABehaviorTreeNodeDescriptor
    {
        public override string TypeId => BehaviorTreeBuiltinNodeTypes.BlackboardExists;

        public override BehaviorTreeNodeKind NodeKind => BehaviorTreeNodeKind.Condition;

        public override string MenuPath => "Conditions/Blackboard/Is Set";

        public override string HandlerName => "BlackboardExists";

        public override string Description => "判断指定黑板键当前是否已存在。";

        public override IReadOnlyList<BehaviorTreeNodeParameterDefinition> Parameters => new List<BehaviorTreeNodeParameterDefinition>
        {
            new()
            {
                Name = "key",
                DisplayName = "Blackboard Key",
                ValueType = BehaviorTreeValueType.String,
                DefaultValue = new BehaviorTreeSerializedValue
                {
                    ValueType = BehaviorTreeValueType.String,
                    StringValue = string.Empty,
                },
                EditorHint = BehaviorTreeNodeParameterEditorHint.BlackboardKey,
            },
        };
    }

    [BehaviorTreeNodeDescriptor]
    public sealed class BehaviorTreeBlackboardCompareNodeDescriptor : ABehaviorTreeNodeDescriptor
    {
        public override string TypeId => BehaviorTreeBuiltinNodeTypes.BlackboardCompare;

        public override BehaviorTreeNodeKind NodeKind => BehaviorTreeNodeKind.Condition;

        public override string MenuPath => "Conditions/Blackboard/Compare";

        public override string HandlerName => "BlackboardCompare";

        public override string Description => "读取黑板值并按照比较符执行判断。";

        public override IReadOnlyList<BehaviorTreeNodeParameterDefinition> Parameters => new List<BehaviorTreeNodeParameterDefinition>
        {
            new()
            {
                Name = "key",
                DisplayName = "Blackboard Key",
                ValueType = BehaviorTreeValueType.String,
                DefaultValue = new BehaviorTreeSerializedValue
                {
                    ValueType = BehaviorTreeValueType.String,
                    StringValue = string.Empty,
                },
                EditorHint = BehaviorTreeNodeParameterEditorHint.BlackboardKey,
            },
            new()
            {
                Name = "operator",
                DisplayName = "Operator",
                ValueType = BehaviorTreeValueType.Integer,
                DefaultValue = new BehaviorTreeSerializedValue
                {
                    ValueType = BehaviorTreeValueType.Integer,
                    IntValue = (int)BehaviorTreeCompareOperator.Equal,
                },
                EditorHint = BehaviorTreeNodeParameterEditorHint.CompareOperator,
            },
            new()
            {
                Name = "value",
                DisplayName = "Compare Value",
                ValueType = BehaviorTreeValueType.None,
                DefaultValue = new BehaviorTreeSerializedValue
                {
                    ValueType = BehaviorTreeValueType.String,
                    StringValue = string.Empty,
                },
            },
        };
    }
}
