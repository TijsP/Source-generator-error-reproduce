namespace ModifierSourceGenerator
{
    internal static class GeneratorHelper
    {
        public const string ModifierAttributeSourcePath = @"Temp/GeneratedCode/ModifierSourceGenerator";
        public const string ModifierAttributeSourceText =
@"#line 1 ""Temp/GeneratedCode/ModifierSourceGenerator/ModifiableStatsAttribute.g.cs""
#pragma warning disable 0219
namespace ModifierSourceGenerator
{
    [System.AttributeUsage(System.AttributeTargets.Struct)]
    public class ModifiableStatsAttribute : System.Attribute
    {
    }
}
";

        public const string RecalculateModifiersEnableableComponentSourceText = @"
#line 1 ""Temp/GeneratedCode/ModifierSourceGenerator""
namespace ModifierSourceGenerator
{
    internal struct RecalculateModifiers : global::Unity.Entities.IComponentData, global::Unity.Entities.IEnableableComponent { }
}
";

        public const string RecalculateModifiersAuthoringSourceText = @"
#line 1 ""Temp/GeneratedCode/ModifierSourceGenerator""
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