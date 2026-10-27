// Polyfill for System.Runtime.CompilerServices.IsExternalInit on netstandard2.0
// Allows C# 9+ record types to compile on target frameworks that do not have this type defined.
namespace System.Runtime.CompilerServices
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal static class IsExternalInit
    {
    }
}

