using System.Numerics;
using System.Runtime.CompilerServices;

namespace _7math;

internal static class Helpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static double Clamp(double value, double min, double max) => Math.Max(min, Math.Min(max, value));
}