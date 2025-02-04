﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Linq;
using System.Text;

namespace SourceGenerators;

[Generator]
public class SoundsSourceGenerator : ISourceGenerator {
    public void Initialize(GeneratorInitializationContext context) {
    }

    public void Execute(GeneratorExecutionContext context) {
        var files = Utilities.GetFiles(context, "ogg", GetAssetFile);
        context.AddSource("AequusSounds.cs", SourceText.From(
            $$"""
                using Terraria.Audio;
                using System.Runtime.CompilerServices;

                namespace Aequus;

                /// <summary>(Total Sounds: {{files.Count}})</summary>
                [CompilerGenerated]
                public partial class AequusSounds {            
                    {{string.Join("\n", files.Select((file) =>
                    $"""
                        /// <summary>Full Path: {file.Path}</summary>
                        public static readonly SoundStyle {file.Name} = new("Aequus/{file.Path}");
                    """))}}
                }
                """, Encoding.UTF8)
        );
    }

    private static AssetFile GetAssetFile(string name, string path) {
        if (name.Contains('.')) {
            return null;
        }

        return new AssetFile(name, path);
    }
}