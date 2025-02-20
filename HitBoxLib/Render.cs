using HitBoxLib.PositionObject;
using NGenerics.DataStructures.General;
using ScreenLib;
using SFML.Graphics;
using SFML.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using static SFML.Window.Mouse;

namespace HitBoxLib;
public static class Render
{
    public static int[,] Edges { get; } = new int[,]
    {
            {0, 1}, {1, 3}, {3, 2}, {2, 0},

            {4, 5}, {5, 7}, {7, 6}, {6, 4},

            {0, 4}, {1, 5}, {2, 6}, {3, 7},
    };
    public const int maxCountPeaks = 8;

    private static float GetDistance(Vector2f point, Vector2f target)
    {
        float dx = target.X - point.X;
        float dy = target.Y - point.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }
    public static double GetAngularDistance(Vector2f sprite, Vector2f player)
    {
        double dx = sprite.X - player.X;
        double dy = sprite.Y - player.Y;

        return Math.Atan2(dy, dx);
    }
    public static double CalculationAngle(double playerAngle, double spriteAngle)
    {
        double angleDifference = spriteAngle - playerAngle;

        if (angleDifference > Math.PI)
            angleDifference -= 2 * Math.PI;
        if (angleDifference < -Math.PI)
            angleDifference += 2 * Math.PI;

        return angleDifference;
    }


    private static List<Vector3f> GetCoordinatesParallelepiped(HitboxObjectInfo objectHitBox, ObserverInfo observer, ref Vector2f center)
    {
        Box Body = objectHitBox.body;

        float minX = (float)(Body[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0);
        float maxX = (float)(Body[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0);
        float minY = (float)(Body[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0);
        float maxY = (float)(Body[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0);
        float minZ = (float)(Body[CoordinatePlane.Z, SideSize.Smaller]?.Side ?? 0);
        float maxZ = (float)(Body[CoordinatePlane.Z, SideSize.Larger]?.Side ?? 0);

        center.X = (maxX + minX) / 2;
        center.Y = (maxY + minY) / 2;


        return new List<Vector3f>
        {
            // Front face
            new Vector3f(minY, maxZ, minX),
            new Vector3f(maxY, maxZ, minX),
            new Vector3f(minY, minZ, minX),
            new Vector3f(maxY, minZ, minX),

            // Back edge
            new Vector3f(minY, maxZ, maxX),
            new Vector3f(maxY, maxZ, maxX),
            new Vector3f(minY, minZ, maxX),
            new Vector3f(maxY, minZ, maxX),
        };
    }
    private static Vector2f GetPositionForAngle(HitboxObjectInfo objectHitBox, Vector3f vertex, Vector2f center)
    {
        Vector2f position = objectHitBox.useEdgeForHeight ?
                new Vector2f(vertex.Z, vertex.X) :
                new Vector2f(center.X, center.Y);

        return position;
    }
    private static List<Vector2f> GetHitboxCoordinatesOnScreen(ref int countNonRender, HitboxObjectInfo objectHitBox, ObserverInfo observer)
    {
        Vector2f center = new Vector2f();
        List<Vector3f> vertices = GetCoordinatesParallelepiped(objectHitBox, observer, ref center);


        List<Vector2f> screenVertices = new List<Vector2f>();
        foreach (var vertex in vertices)
        {
            Vector2f position = GetPositionForAngle(objectHitBox, vertex, center);
            float dist = GetDistance(position, observer.position);
            float safeDistance = MathF.Max(dist, 0.1f);

            double angleDistance = GetAngularDistance(new Vector2f(vertex.Z, vertex.X), observer.position);
            double normalizedAngle = CalculationAngle(observer.angle, angleDistance);

            float screenX = objectHitBox.worldToScreenX(normalizedAngle, observer.deltaAngle);
            float screenY = objectHitBox.worldToScreenY(vertex.Y, safeDistance, observer.vertivalAngle, observer.angle, normalizedAngle);


            if (Math.Abs(normalizedAngle) > observer.fov ||
               screenX < 0 && screenX > Screen.ScreenWidth ||
               screenY < 0 && screenY > Screen.ScreenHeight)
            {
                countNonRender++;
            }

            screenVertices.Add(new Vector2f(screenX, screenY));
        }

        return screenVertices;
    }


    private static VertexArray VertexToArray(HitboxObjectInfo objectHitBox, List<Vector2f> vertices, int countNonRender)
    {
        Vertex[] line = new Vertex[2];
        VertexArray vertexArray = new VertexArray(PrimitiveType.Lines);

        if (countNonRender == maxCountPeaks)
            return vertexArray;

        for (int i = 0; i < Edges.GetLength(0); i++)
        {
            int startIndex = Edges[i, 0];
            int endIndex = Edges[i, 1];

            line[0] = new Vertex(vertices[startIndex], objectHitBox.body.RenderColor);
            line[1] = new Vertex(vertices[endIndex], objectHitBox.body.RenderColor);

            vertexArray.Append(line[0]);
            vertexArray.Append(line[1]);
        }

        return vertexArray;
    }
    public static VertexArray BuildHitBoxMesh(HitboxObjectInfo objectHitBox, ObserverInfo observer)
    {
        int countNonRender = 0;
        List<Vector2f> coordinateScreen = GetHitboxCoordinatesOnScreen(ref countNonRender, objectHitBox, observer);

        return VertexToArray(objectHitBox, coordinateScreen, countNonRender);
    }
}
