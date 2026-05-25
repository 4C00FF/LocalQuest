using dnlib.DotNet;
using dnlib.DotNet.Emit;

const string OldUrl = "http://recconned.gabethefirst.com/";
const string NewUrl = "http://127.0.0.1:16512/";

if (args.Length == 0 || args[0] is "-h" or "--help")
{
    Console.WriteLine("EqTlsPatcher -- patches eq_tls.dll to point at LocalQuest instead of EpicQuest's server");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  EqTlsPatcher <path\\to\\eq_tls.dll>");
    Console.WriteLine();
    Console.WriteLine("Output:");
    Console.WriteLine("  eq_tls_local.dll alongside the original -- drop this into your BepInEx/plugins folder");
    Console.WriteLine("  and remove or rename the original eq_tls.dll.");
    Console.WriteLine();
    Console.WriteLine("Manual alternative (dnSpy):");
    Console.WriteLine("  1. Open dnSpy and load eq_tls.dll.");
    Console.WriteLine("  2. Expand the assembly and navigate to the class containing the Prefix method.");
    Console.WriteLine("  3. Right-click Prefix > Edit Method (C#).");
    Console.WriteLine($"  4. Change \"{OldUrl}\" to \"{NewUrl}\".");
    Console.WriteLine("  5. Click Compile, then File > Save Module.");
    return 0;
}

string inputPath = args[0];

if (!File.Exists(inputPath))
{
    Console.Error.WriteLine($"Error: file not found: {inputPath}");
    return 1;
}

string outputPath = Path.Combine(
    Path.GetDirectoryName(Path.GetFullPath(inputPath)) ?? ".",
    Path.GetFileNameWithoutExtension(inputPath) + "_local.dll");

ModuleDefMD module;
try
{
    module = ModuleDefMD.Load(inputPath);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: could not load assembly: {ex.Message}");
    Console.Error.WriteLine("Make sure the file is a managed .NET DLL (open it in dnSpy to verify).");
    return 1;
}

int patchCount = 0;

foreach (TypeDef type in module.GetTypes())
{
    foreach (MethodDef method in type.Methods)
    {
        if (!method.HasBody)
            continue;

        foreach (Instruction instr in method.Body.Instructions)
        {
            if (instr.OpCode == OpCodes.Ldstr && instr.Operand is string s && s == OldUrl)
            {
                instr.Operand = NewUrl;
                patchCount++;
                Console.WriteLine($"  Patched ldstr in {type.FullName}::{method.Name}");
            }
        }
    }
}

if (patchCount == 0)
{
    Console.Error.WriteLine($"Error: string \"{OldUrl}\" not found in any method.");
    Console.Error.WriteLine("The DLL may already be patched, or this is the wrong file.");
    return 1;
}

try
{
    module.Write(outputPath);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: could not write output: {ex.Message}");
    return 1;
}

Console.WriteLine();
Console.WriteLine($"Patched {patchCount} occurrence(s).");
Console.WriteLine($"Output: {outputPath}");
Console.WriteLine();
Console.WriteLine("Next steps:");
Console.WriteLine("  1. Copy eq_tls_local.dll into your BepInEx/plugins folder.");
Console.WriteLine("  2. Remove or rename the original eq_tls.dll in that folder.");
Console.WriteLine("  3. Start LocalQuest, then launch the game.");
return 0;
