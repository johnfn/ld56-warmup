using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

#nullable enable
#pragma warning disable 8019

record class PackedSceneResource {
  public string? Id { get; set; }
  public string? Uid { get; set; }
  public string? ResPath { get; set; }
}

class Scene {
  public List<Script> Scripts { get; set; } = new List<Script>();
  public List<Node> Nodes { get; set; } = new List<Node>();
  public List<PackedSceneResource> PackedSceneResources { get; set; } = new List<PackedSceneResource>();

  public Node? RootNode { get => Nodes.FirstOrDefault(x => x?.IsRoot ?? false, null); }

  public AnimationLibrary? AnimationLibrary { get; set; }

  public string ResPath { get; set; } = "";
  public string FilePath { get; set; } = "";
}

record class Script {
  public string? Id { get; set; }

  /** 
    * Like `res://Scripts/PlayerPanel.cs` 
    */
  public string? ScriptResPath { get; set; }

  /** 
    * Like `res://Scenes/MyScene.tscn` 
    */
  public string? SceneResPath { get; set; }

  public string FilePath {
    get {
      return ScriptResPath?.Replace("res://", "") ?? "";
    }
  }

  public string ClassName {
    get {
      return Path.GetFileNameWithoutExtension(FilePath);
    }
  }
}

record class AnimationLibrary {
  public string? Id { get; set; }
  public List<string>? Animations { get; set; }
}

record class Node {
  public string? Name { get; set; }
  public string? Type { get; set; }
  public string? Parent { get; set; }
  public bool IsRoot { get => Parent == null; }
  public Script? Script { get; set; }

  public string FullPath {
    get {
      if (Parent == null) {
        return "";
      }

      if (Parent == ".") {
        return Name ?? "";
      }

      return $"{Parent}/{Name}";
    }
  }
}

class MyCodegen {
  public Dictionary<string, Script> GetAllScriptsInFile(string path) {
    var lines = File.ReadAllLines(path);
    var scripts = new Dictionary<string, Script>();
    var regex = new Regex(@"\[ext_resource type=""Script"" path=""(.*)"" id=""(.*)""\]");

    if (!path.StartsWith("./")) {
      throw new Exception("Path does not start with ./");
    }

    var sceneResPath = path.Substring(2);
    sceneResPath = $"res://{sceneResPath}";

    foreach (var line in lines) {
      var match = regex.Match(line);

      if (match.Success) {
        var scriptPath = match.Groups[1].Value;
        var id = match.Groups[2].Value;

        scripts.Add(id, new Script {
          Id = id,
          ScriptResPath = scriptPath,
          SceneResPath = sceneResPath,
        });
      }
    }

    return scripts;
  }

  public List<Node> GetAllNodesInFile(string path, Dictionary<string, Script> scripts) {
    var blocks = File.ReadAllText(path).Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
    var nodes = new List<Node>();

    foreach (var block in blocks) {
      var regex = new Regex(@"\[node name=""(.*?)"" type=""(.*?)""( parent=""(.*?)""|)( groups=\[""(.*?)""\]|)\]");
      var normalNodeMatch = regex.Match(block);

      if (normalNodeMatch.Success) {
        var name = normalNodeMatch.Groups[1].Value;
        var type = normalNodeMatch.Groups[2].Value;
        var parent = normalNodeMatch.Groups[4].Value;

        if (type == "GPUParticles2D") {
          type = "GpuParticles2D";
        }

        if (parent == "") {
          parent = null;
        }

        // Match something like this:
        // script = ExtResource("1_uhqsl")
        // and extract the id

        var scriptRegex = new Regex(@"script = ExtResource\(""(.*?)""\)");
        var scriptMatch = scriptRegex.Match(block);

        if (scriptMatch.Success) {
          var id = scriptMatch.Groups[1].Value;

          if (scripts.TryGetValue(id, out var script)) {
            nodes.Add(new Node {
              Name = name,
              Type = type,
              Parent = parent,
              Script = script
            });
          }
          else {
            Console.WriteLine("Script not found for id {0}", id);
          }
        }
        else {
          nodes.Add(new Node {
            Name = name,
            Type = type,
            Parent = parent
          });
        }
      }
    }

    return nodes;
  }

  public AnimationLibrary? GetAnimationLibrary(string path) {
    var blocks = File.ReadAllText(path).Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);
    var nodes = new List<Node>();
    var regex = new Regex(@"\[sub_resource type=""AnimationLibrary"" id=""(.*?)""\]");

    foreach (var block in blocks) {
      var match = regex.Match(block);

      if (match.Success) {
        var id = match.Groups[1].Value;

        var animationLibrary = new AnimationLibrary {
          Id = id
        };

        var animationRegex = new Regex(@"(.*?): SubResource\(""(.*?)""\),?");
        var matches = animationRegex.Matches(block);
        var animations = new List<string>();

        foreach (Match m in matches) {
          var animationName = m.Groups[1].Value;

          // remove first and last " from animationName
          animationName = animationName.Substring(1, animationName.Length - 2);

          animations.Add(animationName);
        }

        animationLibrary.Animations = animations;

        return animationLibrary;
      }
    }

    return null;
  }

  public void GenerateBindings(
    List<Scene> allScenes
  ) {
    foreach (var scene in allScenes) {
      var rootScript = scene.RootNode?.Script;

      if (rootScript == null) {
        continue;
      }

      CreateBindings(
        scene,
        allScenes,
        rootScript.FilePath.Replace(".cs", ".bindings.cs")
      );
    }
  }

  public void GenerateBindingsForFile(string tscnPath, List<Scene> allScenes) {
    var scene = allScenes.First(s => s.FilePath == tscnPath);
    var rootScript = scene.RootNode?.Script;

    if (rootScript == null) {
      Console.WriteLine("No root script found for {0}", scene.FilePath);
      return;
    }

    var bindingFilePath = rootScript.FilePath.Replace(".cs", ".bindings.cs");

    CreateBindings(
      scene,
      allScenes,
      bindingFilePath
    );
  }

  public List<PackedSceneResource> GetAllPackedScenesInFile(string path) {
    var lines = File.ReadAllLines(path);
    var packedScenes = new List<PackedSceneResource>();

    // match something like this, and extract the id and path:
    // [ext_resource type="PackedScene" uid="uid://dgh6lsus6gbvk" path="res://Scenes/InteractArea.tscn" id="2_3cxuh"]

    var regex = new Regex(@"\[ext_resource type=""PackedScene"" uid=""uid://(.*?)"" path=""(.*?)"" id=""(.*?)""\]");

    foreach (var line in lines) {
      var match = regex.Match(line);

      if (match.Success) {
        var uid = match.Groups[1].Value;
        var scenePath = match.Groups[2].Value;
        var id = match.Groups[3].Value;

        packedScenes.Add(new PackedSceneResource {
          Id = id,
          ResPath = scenePath,
          Uid = uid
        });
      }
    }

    return packedScenes;
  }

  public List<Node> GetInstancedNodesInScene(
    Scene scene,
    List<Scene> allScenes
  ) {
    var nodes = new List<Node>();
    var blocks = File.ReadAllText(scene.FilePath).Split(new[] { "\n\n" }, StringSplitOptions.RemoveEmptyEntries);

    // Match something like this:
    // [node name="InteractArea" parent="." instance=ExtResource("2_3cxuh")]

    foreach (var block in blocks) {
      var instanceNodeRegex = new Regex(@"\[node name=""(.*?)"" parent=""(.*?)"" instance=ExtResource\(""(.*?)""\)\]");
      var instanceNodeMatch = instanceNodeRegex.Match(block);

      if (instanceNodeMatch.Success) {
        var name = instanceNodeMatch.Groups[1].Value;
        var parent = instanceNodeMatch.Groups[2].Value;
        var id = instanceNodeMatch.Groups[3].Value;

        var associatedPackedSceneResPath = scene.PackedSceneResources.First(p => p.Id == id).ResPath;
        var associatedScene = allScenes.First(s => s.ResPath == associatedPackedSceneResPath);

        if (associatedScene.RootNode?.Script == null) {
          // nodes.Add(new Node {
          //   Name = name,
          //   Parent = parent,
          //   Script = associatedScene.RootNode?.Script
          // });
        }
        else {
          nodes.Add(new Node {
            Name = name,
            Parent = parent,
            Script = associatedScene.RootNode.Script,
            Type = associatedScene.RootNode.Script?.ClassName
          });
        }
      }
    }

    return nodes;
  }

  public Scene ParseScene(string file) {
    var scripts = GetAllScriptsInFile(file);
    var packedScenes = GetAllPackedScenesInFile(file);
    var nodes = GetAllNodesInFile(file, scripts);
    var animationLibrary = GetAnimationLibrary(file);
    var sceneResPath = file.Substring(2);
    sceneResPath = $"res://{sceneResPath}";

    return new Scene {
      Nodes = nodes,
      Scripts = [.. scripts.Values],
      AnimationLibrary = animationLibrary,
      PackedSceneResources = packedScenes,
      ResPath = sceneResPath,
      FilePath = file
    };
  }

  public static string GetNamespace(string fileContents) {
    var lines = fileContents.Split(new[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
    var namespaceLine = lines.FirstOrDefault(line => line.TrimStart().StartsWith("namespace"));

    if (namespaceLine == null) {
      return "";
    }

    if (namespaceLine.Contains('{')) {
      // Instead return something like this:
      // "namespace LD56;"
      return namespaceLine.Trim().Replace("{", ";");
    }

    return namespaceLine.Trim();
  }

  public static void CreateBindings(
    Scene scene,
    List<Scene> allScenes,
    string outputPath
  ) {
    var rootScript = scene.RootNode?.Script;

    if (rootScript == null) {
      Console.WriteLine("No root script found for {0}", scene.FilePath);
      return;
    }

    var scriptPath = rootScript.FilePath;
    var fileContents = File.ReadAllText(scriptPath);
    var className = rootScript.ClassName;
    var regex = new Regex(@"public partial class (?<" + className + @">.+) : (?<superClassName>.+) {");
    var match = regex.Match(fileContents);

    if (!match.Success) {
      Console.WriteLine("Couldn't find class name in {0}", scriptPath);
      return;
    }

    var superClassName = match.Groups["superClassName"].Value;

    var newFileContent = "";

    newFileContent += "using Godot;\n";
    newFileContent += GetNamespace(fileContents);
    newFileContent += "";
    newFileContent += $"public partial class {className} : {superClassName} {{\n";
    newFileContent += $"  public static {className} New() {{\n";
    newFileContent += $"    return GD.Load<PackedScene>(\"{rootScript.SceneResPath}\").Instantiate<{className}>();\n";
    newFileContent += $"  }}\n";

    newFileContent += $"  public {className}() {{\n";
    newFileContent += $"    foreach (var @interface in GetType().GetInterfaces()) {{\n";
    newFileContent += $"      AddToGroup(@interface.Name);\n";
    newFileContent += $"    }}\n";
    newFileContent += $"  }}\n";
    newFileContent += $"\n";

    newFileContent += $"  public partial class {className}Nodes {{\n";
    newFileContent += $"    {className} parent;\n";
    newFileContent += $"";
    newFileContent += $"    public {className}Nodes({className} parent) {{\n";
    newFileContent += $"      this.parent = parent;\n";
    newFileContent += $"    }}\n";

    foreach (var node in scene.Nodes) {
      var normalizedPath = node.FullPath.Replace("/", "_");

      if (normalizedPath.Trim() == "") {
        continue;
      }

      var instanceName = normalizedPath;

      if (normalizedPath == className) {
        instanceName += "_";
      }

      if (node.Type == "AnimationPlayer" && scene.AnimationLibrary != null && scene.AnimationLibrary.Animations != null) {
        newFileContent += $"    private {node.Type}? _{instanceName};\n";
        newFileContent += $"    public {node.Type} {instanceName} {{\n";
        newFileContent += $"      get {{\n";
        newFileContent += $"        return _{instanceName} ??= parent.GetNode<{node.Type}>(\"{node.FullPath}\");\n";
        newFileContent += $"      }}\n";
        newFileContent += $"    }}\n";
        newFileContent += $"\n";

        foreach (var animationName in scene.AnimationLibrary.Animations) {
          newFileContent += $@"    public void {instanceName}_Play{animationName}() {{
      {instanceName}.Play(""{animationName}"");
    }}
";
        }
      }
      else {
        newFileContent += $"    private {node.Type}? _{instanceName};\n";
        newFileContent += $"    public {node.Type} {instanceName} {{\n";
        newFileContent += $"      get {{\n";
        newFileContent += $"        return _{instanceName} ??= parent.GetNode<{node.Type}>(\"{node.FullPath}\");\n";
        newFileContent += $"      }}\n";
        newFileContent += $"    }}\n";
        newFileContent += $"\n";
      }
    }

    newFileContent += "  }\n";
    newFileContent += "\n";
    newFileContent += $"  public {className}Nodes? _Nodes;\n";
    newFileContent += $"  public {className}Nodes Nodes {{\n";
    newFileContent += $"    get {{\n";
    newFileContent += $"      return _Nodes ??= new {className}Nodes(this);\n";
    newFileContent += $"    }}\n";
    newFileContent += $"  }}\n";
    newFileContent += "}\n";

    Console.WriteLine("Write: {0}", outputPath);
    File.WriteAllText(outputPath, newFileContent);
  }

  public List<Scene> ParseAllScenes() {
    var files = Directory.GetFiles(".", "*.tscn", SearchOption.AllDirectories);
    var allScenes = files.Select(x => ParseScene(x)).ToList();

    foreach (var scene in allScenes) {
      var instancedNodes = GetInstancedNodesInScene(scene, allScenes);

      scene.Nodes = scene.Nodes.Concat(instancedNodes).ToList();
    }

    return allScenes;
  }

  public void WatchForChanges() {
    var allScenes = ParseAllScenes();

    GenerateBindings(allScenes);

    // Watch for recursive file changes

    var watcher = new FileSystemWatcher(".", "*.tscn") {
      IncludeSubdirectories = true,
      EnableRaisingEvents = true,
      NotifyFilter = NotifyFilters.Attributes |
      NotifyFilters.CreationTime |
      NotifyFilters.FileName |
      NotifyFilters.LastAccess |
      NotifyFilters.LastWrite |
      NotifyFilters.Size |
      NotifyFilters.Security
    };

    var lastChange = DateTime.MinValue;

    watcher.Changed += (sender, args) => {
      if (DateTime.Now - lastChange < TimeSpan.FromMilliseconds(500)) {
        return;
      }

      lastChange = DateTime.Now;

      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("Change: {0}", args.FullPath);
      Console.ResetColor();

      GenerateBindingsForFile(args.FullPath, allScenes);
    };

    watcher.Deleted += (sender, args) => {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("Delete: {0}", args.FullPath);
      Console.ResetColor();
    };

    watcher.Created += (sender, args) => {
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine("Create: {0}", args.FullPath);
      Console.ResetColor();

      allScenes = ParseAllScenes();

      GenerateBindingsForFile(args.FullPath, allScenes);
    };

    watcher.EnableRaisingEvents = true;

    // wait for a key press to exit

    Console.ReadKey();
  }
}
