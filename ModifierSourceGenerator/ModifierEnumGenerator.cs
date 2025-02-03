using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ModifierSourceGenerator
{
    [Generator]
    public class ModifierEnumGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            IncrementalValuesProvider<StructDeclarationSyntax> valuesProvider = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: IsSyntaxTargetForGeneration,
                    transform: GetSemanticTargetForGeneration)
                .Where(m => m is { });

            context.RegisterSourceOutput(valuesProvider, Execute);
        }

        private void Execute(SourceProductionContext context, StructDeclarationSyntax source)
        {
            StringBuilder sourceTextBuilder = new StringBuilder(@"
namespace ModifierSourceGenerator {
    public class ModifierHelper{
        public static string StructFound  = @"""); sourceTextBuilder.Append(source.ToString()); sourceTextBuilder.Append(@""";
    }
}");

            string sourceText = sourceTextBuilder.ToString();
            context.AddSource("ModifiableStats.g.cs", sourceText);

            Directory.CreateDirectory("Temp/ModifierSourceGenerator/GeneratedCode");
            File.WriteAllText("Temp/ModifierSourceGenerator/GeneratedCode/ModifiableStats.txt", sourceText);
        }

        public static bool IsSyntaxTargetForGeneration(SyntaxNode syntaxNode, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            // Is Struct
            if (syntaxNode is not StructDeclarationSyntax structDeclarationSyntax)
                return false;

            // Has Base List
            if (structDeclarationSyntax.BaseList == null)
                return false;

            //  for debugging within Unity
            string tracker = structDeclarationSyntax.ToString() + '\n';

            // Has IJobEntity identifier
            var hasIJobEntityIdentifier = false;
            foreach (var baseType in structDeclarationSyntax.BaseList.Types)
            {
                tracker += baseType.Type.ToString() + '\n';
                if (baseType.Type is IdentifierNameSyntax { Identifier.ValueText: "IComponentData" })
                {
                    hasIJobEntityIdentifier = true;
                    break;
                }
            }

            //  output to file which structs have been found
            Directory.CreateDirectory("Temp/ModifierSourceGenerator/Logs");
            File.WriteAllText($"Temp/ModifierSourceGenerator/Logs/StructFound_{structDeclarationSyntax.Identifier}.txt", tracker);

            if (!hasIJobEntityIdentifier)
                return false;

            return true;
        }

        public static StructDeclarationSyntax GetSemanticTargetForGeneration(GeneratorSyntaxContext ctx, CancellationToken cancellationToken)
        {
            var structDeclarationSyntax = (StructDeclarationSyntax)ctx.Node;
            foreach (var baseTypeSyntax in structDeclarationSyntax.BaseList!.Types)
                if (ctx.SemanticModel.GetTypeInfo(baseTypeSyntax.Type).Type.ToDisplayString(QualifiedFormat).Contains("IComponentData"))
                    return structDeclarationSyntax;
            return null;
        }

        static SymbolDisplayFormat QualifiedFormat { get; } =
            new(
                typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
                globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
                genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
                miscellaneousOptions:
                SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
                SymbolDisplayMiscellaneousOptions.UseSpecialTypes);
    }
}
