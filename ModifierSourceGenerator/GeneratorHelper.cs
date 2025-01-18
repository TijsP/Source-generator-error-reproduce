namespace ModifierSourceGenerator
{
    internal static class GeneratorHelper
    {
        public const string ModifierAttributeSourceText = @"
namespace ModifierSourceGenerator
{
    [System.AttributeUsage(System.AttributeTargets.Struct)]
    internal class ModifiableStatsAttribute : System.Attribute
    {
    }
}
";

        public const string RecalculateModifiersEnableableComponentSourceText = @"
namespace ModifierSourceGenerator
{
    internal struct RecalculateModifiers : global::Unity.Entities.IComponentData, global::Unity.Entities.IEnableableComponent { }
}
";

        public const string RecalculateModifiersAuthoringSourceText = @"
using System;

namespace ModifierSourceGenerator
{
    internal class RecalculateModifiersAuthoring : global::UnityEngine.MonoBehaviour
    {
        class Baker : global::Unity.Entities.Baker<RecalculateModifiersAuthoring>
        {
            public override void Bake(RecalculateModifiersAuthoring authoring)
            {
                var entity = GetEntity(global::Unity.Entities.TransformUsageFlags.None);
                AddComponent<RecalculateModifiers>(entity);
                SetComponentEnabled<RecalculateModifiers>(entity, true);
            }
        }
    }
}
";
    }
}