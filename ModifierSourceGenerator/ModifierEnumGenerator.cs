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
    public partial class ModifierHelper {
        public static string StructFound_"); sourceTextBuilder.Append(source.Identifier.Text);
            sourceTextBuilder.Append(@" = @"""); sourceTextBuilder.Append(source.ToString()); sourceTextBuilder.Append(@""";
    }
}");

            string hintName = source.SyntaxTree.GetGeneratedSourceFileName("ModifierEnumGenerator", source, source.Identifier.Text);
            sourceTextBuilder.AppendLine($"/* {hintName} */");

            string sourceText = sourceTextBuilder.ToString();
            context.AddSource(hintName, sourceText);

            string folderPath = Path.GetDirectoryName(source.SyntaxTree.FilePath);
            folderPath = Path.Combine(folderPath, "Temp~/ModifierSourceGenerator/GeneratedCode");
            folderPath = Path.GetFullPath(folderPath);

            string filePath = Path.Combine(folderPath, $"ModifiableStats_{source.Identifier.Text}.txt");
            filePath = Path.GetFullPath(filePath);

            Directory.CreateDirectory(folderPath);
            File.WriteAllText(filePath, sourceText);
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

            var folderPath = Path.GetDirectoryName(syntaxNode.SyntaxTree.FilePath);
            folderPath = Path.Combine(folderPath, "Temp~/ModifierSourceGenerator/Logs");
            folderPath = Path.GetFullPath(folderPath);

            var filePath = Path.Combine(folderPath, $"StructFound__{structDeclarationSyntax.Identifier.Text}.txt");
            filePath = Path.GetFullPath(filePath);

            //  output to file which structs have been found
            Directory.CreateDirectory(folderPath);
            File.WriteAllText(filePath, tracker);

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

    public static class SyntaxNodeExtensions
    {
        public static string GetGeneratedSourceFileName(this SyntaxTree syntaxTree, string generatorName, SyntaxNode node, string typeName)
            => GetGeneratedSourceFileName(syntaxTree, generatorName, node.GetLineNumber(), typeName);

        public static string GetGeneratedSourceFileName(this SyntaxTree syntaxTree, string generatorName, int salting, string typeName)
        {
            var (isSuccess, fileName) = TryGetFileNameWithoutExtension(syntaxTree);
            var stableHashCode = syntaxTree.GetStableHashCode();

            var postfix = generatorName.Length > 0 ? $"__{generatorName}" : string.Empty;

            if (string.IsNullOrWhiteSpace(typeName) == false)
            {
                postfix = $"__{typeName}{postfix}";
            }

            if (isSuccess)
                fileName = $"{fileName}{postfix}_{stableHashCode}_{salting}.g.cs";
            else
                fileName = Path.Combine($"{Path.GetRandomFileName()}{postfix}", ".g.cs");

            return fileName;
        }

        public static int GetLineNumber(this SyntaxNode node)
            => node.GetLocation().GetLineSpan().StartLinePosition.Line + 1;

        public static int GetStableHashCode(this SyntaxTree syntaxTree)
            => GetStableHashCode(syntaxTree.FilePath) & 0x7fffffff;

        public static (bool IsSuccess, string FileName) TryGetFileNameWithoutExtension(this SyntaxTree syntaxTree)
        {
            var fileName = Path.GetFileNameWithoutExtension(syntaxTree.FilePath);
            return (IsSuccess: true, fileName);
        }

        public static int GetStableHashCode(string str)
        {
            unchecked
            {
                var hash1 = 5381;
                var hash2 = hash1;

                for (var i = 0; i < str.Length && str[i] != '\0'; i += 2)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ str[i];
                    if (i == str.Length - 1 || str[i + 1] == '\0')
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ str[i + 1];
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}
