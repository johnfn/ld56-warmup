#load "CreateBindingsScriptInner.cs"

// Run this like so:
// dotnet-script CreateBindingsScript.csx

// CSX files were giving me stupid syntax errors, so I just put the code in a
// function and called it here.

void GenerateBindings() {
  var codegen = new MyCodegen();
  codegen.WatchForChanges();
}

GenerateBindings();