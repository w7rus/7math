using System.Numerics;
using System.Runtime.CompilerServices;

namespace _7math;

internal static class Helpers
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static float Clamp(float value, float min, float max) => MathF.Max(min, MathF.Min(max, value));
}