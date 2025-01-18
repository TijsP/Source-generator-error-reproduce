using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.IO;
using System.Text;

namespace ModifierSourceGenerator
{
    [Generator]
    public class ModifierEnumGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            context.RegisterPostInitializationOutput(ctx => {
                ctx.AddSource("ModifiableStatsAttribute.g.cs", SourceText.From(GeneratorHelper.ModifierAttributeSourceText, Encoding.UTF8));
                //ctx.AddSource("RecalculateModifiers.g.cs", SourceText.From(GeneratorHelper.RecalculateModifiersEnableableComponentSourceText, Encoding.UTF8));
                //ctx.AddSource("RecalculateModifiersAuthoring.g.cs", SourceText.From(GeneratorHelper.RecalculateModifiersAuthoringSourceText, Encoding.UTF8));
            });

            Directory.CreateDirectory(GeneratorHelper.ModifierAttributeSourcePath);
            File.WriteAllText(GeneratorHelper.ModifierAttributeSourcePath + "/ModifiableStatsAttribute.g.cs", SourceText.From(GeneratorHelper.ModifierAttributeSourceText, Encoding.UTF8).ToString());
        }
    }
}
