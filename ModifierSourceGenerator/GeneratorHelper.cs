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
    internal struct RecalculateModifiers : global::Unity.Entities.IEnableableComponent { }
}
";

        public const string RecalculateModifiersAuthoringSourceText = @"
using System;

namespace ModifiersSourceGenerator
{
    internal class RecalculateModifiersAuthoring : global::UnityEngine.MonoBehaviour
    {
        class Baker : global::Unity.Entities.Baker<RecalculateModifiersAuthoring>
        {
            public override void Bake(RecalculateModifiersAuthoring authoring)
            {
                global::Unity.Entities.Entity entity = global::Unity.Entities.GetEntity(global::Unity.Entities.TransformUsageFlags.None);
                global::Unity.Entities.AddComponent<RecalculateModifiers>(entity);
                global::Unity.Entities.SetComponentEnabled<RecalculateModifiers>(entity, true);
            }
        }
    }
}
";
    }
}