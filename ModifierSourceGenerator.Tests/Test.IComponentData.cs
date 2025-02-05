using Unity.Entities;

namespace ModifierSourceGenerator.Tests
{
    [ModifierSourceGenerator.ModifiableStats]
    public struct TestComponent1 : IComponentData
    {
        public int TestInt;
    }

    [ModifierSourceGenerator.ModifiableStats]
    public struct TestComponent2 : IComponentData
    {
        public float TestFloat;
    }
}