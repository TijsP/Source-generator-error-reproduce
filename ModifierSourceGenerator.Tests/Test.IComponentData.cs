using Unity.Entities;

namespace ModifierSourceGenerator.Tests
{
    [ModifierSourceGenerator.ModifiableStats]
    public struct TestComponent1 : IComponentData
    {
        public int TestInt;
    }
}