// tools/generate_api_reference.csx
#r "nuget: Microsoft.CodeAnalysis.CSharp, 4.0.1"
#r "nuget: Microsoft.CodeAnalysis.Analyzers, 3.3.3"

using System;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

var corePath = Path.Combine(Directory.GetCurrentDirectory(), "RecallCore");
var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "API_REFERENCE.md");

var csFiles = Directory.GetFiles(corePath, "*.cs", SearchOption.AllDirectories);

var classes = csFiles.SelectMany(file =>
{
    var text = File.ReadAllText(file);
    var syntaxTree = CSharpSyntaxTree.ParseText(text);
    var root = syntaxTree.GetRoot();

    return root.DescendantNodes()
        .OfType<ClassDeclarationSyntax>()
        .Where(cls => cls.Modifiers.Any(m => m.Text == "public"))
        .Select(cls =>
        {
            var methods = cls.Members.OfType<MethodDeclarationSyntax>()
                .Where(m => m.Modifiers.Any(mod => mod.Text == "public"))
                .Select(m => $"{m.ReturnType} {m.Identifier}({string.Join(", ", m.ParameterList.Parameters.Select(p => $"{p.Type} {p.Identifier}"))})");

            var props = cls.Members.OfType<PropertyDeclarationSyntax>()
                .Where(p => p.Modifiers.Any(mod => mod.Text == "public"))
                .Select(p => $"{p.Type} {p.Identifier}");

            return new
            {
                Name = cls.Identifier.Text,
                Methods = methods.ToList(),
                Properties = props.ToList()
            };
        });
});

using (var writer = new StreamWriter(outputPath))
{
    writer.WriteLine("# RecallCore API Reference");
    writer.WriteLine();
    foreach (var cls in classes)
    {
        writer.WriteLine($"## Class `{cls.Name}`");
        if (cls.Properties.Any())
        {
            writer.WriteLine("**Properties:**");
            foreach (var p in cls.Properties)
                writer.WriteLine($"- `{p}`");
        }
        if (cls.Methods.Any())
        {
            writer.WriteLine("**Methods:**");
            foreach (var m in cls.Methods)
                writer.WriteLine($"- `{m}`");
        }
        writer.WriteLine();
    }
}

Console.WriteLine($"API_REFERENCE.md updated: {outputPath}");
