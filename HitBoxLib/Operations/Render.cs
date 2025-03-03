using DataPipes;
using HitBoxLib.Data.HitBoxObject;
using HitBoxLib.Data.Observer;
using HitBoxLib.HitBoxSegment;
using HitBoxLib.PositionObject;
using HitBoxLib.Segment.SignsTypeSide;
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


namespace HitBoxLib.Operations;
public static class Render
{
    public static int[,] Edges { get; } = new int[,]
    {
            {0, 1}, {1, 3}, {3, 2}, {2, 0},

            {4, 5}, {5, 7}, {7, 6}, {6, 4},

            {0, 4}, {1, 5}, {2, 6}, {3, 7},
    };
    public const int maxCountPeaks = 8;


    private static List<Vector3f> GetCoordinatesParallelepiped(RenderInfo objectHitBox, ObserverInfo observer, ref Vector2f center)
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
    private static Vector2f GetPositionForAngle(RenderInfo objectHitBox, Vector3f vertex, Vector2f center)
    {
        switch (objectHitBox.body.HeightRenderMode)
        {
            case RenderHeightMode.EdgeBased:
                return new Vector2f(vertex.Z, vertex.X);
            case RenderHeightMode.CenterBased:
                return new Vector2f(center.X, center.Y);
            default:
                return new Vector2f();
        }
    }
    private static List<Vector2f> GetHitboxCoordinatesOnScreen(ref int countNonRender, RenderInfo objectHitBox, ObserverInfo observer)
    {
        Vector2f center = new Vector2f();
        List<Vector3f> vertices = GetCoordinatesParallelepiped(objectHitBox, observer, ref center);


        List<Vector2f> screenVertices = new List<Vector2f>();
        foreach (var vertex in vertices)
        {
            Vector2f position = GetPositionForAngle(objectHitBox, vertex, center);
            float dist = MathUtils.CalculateDistance(position, observer.position);
            float safeDistance = MathF.Max(dist, 0.1f);

            double angleDistance = MathUtils.CalculateAngleToTarget(new Vector2f(vertex.Z, vertex.X), observer.position);
            double normalizedAngle = MathUtils.NormalizeAngleDifference(observer.angle, angleDistance);

            float screenX = objectHitBox.worldToScreenX(normalizedAngle, observer.deltaAngle);
            float screenY = objectHitBox.worldToScreenY(vertex.Y, safeDistance, observer.vertivalAngle, normalizedAngle);


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


    private static VertexArray VertexToArray(RenderInfo objectHitBox, List<Vector2f> vertices, int countNonRender)
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
    public static VertexArray BuildHitBoxMesh(RenderInfo objectHitBox, ObserverInfo observer)
    {
        int countNonRender = 0;
        List<Vector2f> coordinateScreen = GetHitboxCoordinatesOnScreen(ref countNonRender, objectHitBox, observer);

        return VertexToArray(objectHitBox, coordinateScreen, countNonRender);
    }
}
