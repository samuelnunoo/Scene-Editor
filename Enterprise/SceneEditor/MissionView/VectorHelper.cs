using TaleWorlds.Library;

internal static class VectorHelper
{
    public static Vec3 Project(in this Vec3 position, in Vec3 direction, float distance)
    {
        return position + direction * distance;
    }
}