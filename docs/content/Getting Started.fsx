(**
---
category: end-users
categoryindex: 1
index: 1
---

# Getting started using analyzers

## Premise

We assume the analyzers you want to use are distributed as a nuget package.

## Using analyzers in a single project

First, we need to add the [fsharp-analyzers](https://www.nuget.org/packages/fsharp-analyzers) dotnet tool to the tool-manifest.
```shell
dotnet tool install fsharp-analyzers
```

Next, add the `PackageReference` pointing to your favorite analyzers to the `.fsproj` file of the project you want to analyzse:

```xml
<PackageReference Include="G-Research.FSharp.Analyzers" Version="0.1.6">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>build</IncludeAssets>
</PackageReference>
```

Finally, add a custom MSBuild target to the `.fsproj` file for easy invocation of the analyzer:

```xml
<Target
    Name="AnalyzeProject" 
    DependsOnTargets="Restore;ResolveAssemblyReferencesDesignTime;ResolveProjectReferencesDesignTime;ResolvePackageDependenciesDesignTime;FindReferenceAssembliesForReferences;_GenerateCompileDependencyCache;_ComputeNonExistentFileProperty;BeforeBuild;BeforeCompile;CoreCompile">

    <Message Importance="High" Text="Analyzing $(MSBuildProjectFile)"/>
    <Exec
        ContinueOnError="true"
        Command="dotnet fsharp-analyzers --project &quot;$(MSBuildProjectFile)&quot; --analyzers-path &quot;$(PkgG-Research_FSharp_Analyzers)\analyzers\dotnet\fs&quot; --exclude-analyzer PartialAppAnalyzer --fail-on-warnings GRA-STRING-001 GRA-STRING-002 GRA-STRING-003 GRA-UNIONCASE-001 --verbose --report &quot;$(MSBuildProjectName)-analysis.sarif&quot;">
        <Output TaskParameter="ExitCode" PropertyName="LastExitCode" />
    </Exec>
    <Error Condition="'$(LastExitCode)' == '-2'" Text="Problems were found $(MSBuildProjectFile)" />
</Target>
```

You may need to adjust the `Command` to be compatible with your specific analyzer. Think about how you want warnings to be treated.

At last, you can run the analyzer from the project folder:

```shell
dotnet msbuild /t:AnalyzeProject
```

## Using analyzers in a solution

First, we need to add the [fsharp-analyzers](https://www.nuget.org/packages/fsharp-analyzers) dotnet tool to the tool-manifest.
```shell
dotnet tool install fsharp-analyzers
```

Next, add the `PackageReference` pointing to your favorite analyzers to the [Directory.Build.props](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory?view=vs-2022) file:

```xml
<ItemGroup>
    <PackageReference Include="G-Research.FSharp.Analyzers" Version="0.1.6">
    <PrivateAssets>all</PrivateAssets>
    <IncludeAssets>build</IncludeAssets>
    </PackageReference>
</ItemGroup>
```

Add the following custom target to the [Directory.Solution.targets](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-solution-build?view=vs-2022) file to be able to invoke analysis of the whole solution:

```xml
<Project>

    <ItemGroup>
        <ProjectsToAnalyze Include="src\**\*.fsproj" />
    </ItemGroup>

    <Target Name="AnalyzeSolution">
        <Exec Command="dotnet build -c Release $(SolutionFileName)" />
        <MSBuild
                Projects="@(ProjectsToAnalyze)"
                Targets="AnalyzeProject"
                Properties="DesignTimeBuild=True;Configuration=Release;ProvideCommandLineArgs=True;SkipCompilerExecution=True" />
    </Target>

</Project>
```

Finally, add the following custom target to the [Directory.Build.targets](https://learn.microsoft.com/en-us/visualstudio/msbuild/customize-by-directory?view=vs-2022) file. This takes care of invoking analysis of single projects:
```xml
<Project>

    <Target
        Name="AnalyzeProject" 
        DependsOnTargets="Restore;ResolveAssemblyReferencesDesignTime;ResolveProjectReferencesDesignTime;ResolvePackageDependenciesDesignTime;FindReferenceAssembliesForReferences;_GenerateCompileDependencyCache;_ComputeNonExistentFileProperty;BeforeBuild;BeforeCompile;CoreCompile">
        
        <Message Importance="normal" Text="fsc arguments: @(FscCommandLineArgs)" />
        <Message Importance="High" Text="Analyzing $(MSBuildProjectFile)"/>
        <Exec
            ContinueOnError="true"
            Command="dotnet fsharp-analyzers --fsc-args &quot;@(FscCommandLineArgs)&quot; --analyzers-path &quot;$(PkgG-Research_FSharp_Analyzers)\analyzers\dotnet\fs&quot; --exclude-analyzer PartialAppAnalyzer --fail-on-warnings GRA-STRING-001 GRA-STRING-002 GRA-STRING-003 GRA-UNIONCASE-001 --verbose --report &quot;$(MSBuildProjectName)-analysis.sarif&quot;">
            <Output TaskParameter="ExitCode" PropertyName="LastExitCode" />
        </Exec>
        <Error Condition="'$(LastExitCode)' == '-2'" Text="Problems were found $(MSBuildProjectFile)" />

    </Target>

</Project>
```

You may need to adjust the `Command` to be compatible with your specific analyzer. Think about how you want warnings to be treated.

⚠️ Note: This uses a feature from .NET 8 to get hold of the `FscCommandLineArgs`.

At last, you can run the analyzer from the solution folder:

```shell
dotnet msbuild /t:AnalyzeSolution
```
*)

(**

[Next]({{fsdocs-next-page-link}})

*)
