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
/// <summary>Rendering HitBox</summary>
public static class Render
{    
    /// <summary>Hitbox Edges Array</summary>
    public static int[,] Edges { get; } = new int[,]
    {
            {0, 1}, {1, 3}, {3, 2}, {2, 0},

            {4, 5}, {5, 7}, {7, 6}, {6, 4},

            {0, 4}, {1, 5}, {2, 6}, {3, 7},
    };
    /// <summary>Amount peaks in hitbox</summary>
    public const int maxCountPeaks = 8;


    private const float baseScreenHeightForHitBox = 600f;
    private const float baseScreenWidthForHitBox = 1000f;
    /// <summary>Multiplier to normalize hitbox height on different screens</summary>
    public static float MultHeight { get; private set; }
    private static void SetNewMult()
    {
        MultHeight = (Screen.ScreenHeight / baseScreenHeightForHitBox);
        MultHeight /= (Screen.ScreenWidth / baseScreenWidthForHitBox);

        MultHeight = Screen.ScreenHeight / (baseScreenHeightForHitBox * MultHeight);
    }
    static Render()
    {
        SetNewMult();
        Screen.HeightChangesFun += SetNewMult;
        Screen.WidthChangesFun += SetNewMult;
    }


    private static List<Vector3f> GetCoordinatesParallelepiped(RenderInfo objectHitBox, Box currentBox, ObserverInfo observer, ref Vector2f center)
    {
        float minX = (float)(currentBox[CoordinatePlane.X, SideSize.Smaller]?.Side ?? 0);
        float maxX = (float)(currentBox[CoordinatePlane.X, SideSize.Larger]?.Side ?? 0);
        float minY = (float)(currentBox[CoordinatePlane.Y, SideSize.Smaller]?.Side ?? 0);
        float maxY = (float)(currentBox[CoordinatePlane.Y, SideSize.Larger]?.Side ?? 0);

        float minZ = (float)(currentBox[CoordinatePlane.Z, SideSize.Smaller]?.Side ?? 0);
        float maxZ = (float)(currentBox[CoordinatePlane.Z, SideSize.Larger]?.Side ?? 0);

        float mult = objectHitBox.position.X == observer.position.X &&
                    objectHitBox.position.Y == observer.position.Y &&
                    (minZ + maxZ) / 2 == observer.position.Z ?
                    MultHeight : 1;
        minZ = (float)(minZ * MultHeight - observer.position.Z * mult);
        maxZ = (float)(maxZ * MultHeight - observer.position.Z * mult);

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
    private static Vector2f GetPositionForAngle(RenderInfo objectHitBox, Box currentBox, Vector3f vertex, Vector2f center)
    {
        switch (currentBox.HeightRenderMode)
        {
            case RenderHeightMode.EdgeBased:
                return new Vector2f(vertex.Z, vertex.X);
            case RenderHeightMode.CenterBased:
                return new Vector2f(center.X, center.Y);
            default:
                return new Vector2f();
        }
    }
    private static List<Vector2f> GetHitboxCoordinatesOnScreen(ref int countNonRender, RenderInfo objectHitBox, Box currentBox, ObserverInfo observer)
    {
        Vector2f center = new Vector2f();
        List<Vector3f> vertices = GetCoordinatesParallelepiped(objectHitBox, currentBox, observer, ref center);


        List<Vector2f> screenVertices = new List<Vector2f>();
        foreach (var vertex in vertices)
        {
            Vector2f position = GetPositionForAngle(objectHitBox, currentBox, vertex, center);
            float dist = MathUtils.CalculateDistance(position, observer.position);
            float safeDistance = MathF.Max(dist, 0.1f);

            double angleDistance = MathUtils.CalculateAngleToTarget(new Vector2f(vertex.Z, vertex.X), observer.position);
            double normalizedAngle = MathUtils.NormalizeAngleDifference(observer.angle, angleDistance);

            float screenX = objectHitBox.worldToScreenX(normalizedAngle, observer.deltaAngle);
            float screenY = objectHitBox.worldToScreenY(vertex.Y, safeDistance, observer.verticalAngle, normalizedAngle);


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


    private static VertexArray VertexToArray(RenderInfo objectHitBox, Box currentBox, List<Vector2f> vertices, int countNonRender)
    {
        Vertex[] line = new Vertex[2];
        VertexArray vertexArray = new VertexArray(PrimitiveType.Lines);

        if (countNonRender == maxCountPeaks)
            return vertexArray;

        for (int i = 0; i < Edges.GetLength(0); i++)
        {
            int startIndex = Edges[i, 0];
            int endIndex = Edges[i, 1];

            line[0] = new Vertex(vertices[startIndex], currentBox.RenderColor);
            line[1] = new Vertex(vertices[endIndex], currentBox.RenderColor);

            vertexArray.Append(line[0]);
            vertexArray.Append(line[1]);
        }

        return vertexArray;
    }
    private static VertexArray BuildBoxMesh(RenderInfo objectHitBox, Box currentBox, ObserverInfo observer)
    {

        int countNonRender = 0;
        List<Vector2f> coordinateScreen = GetHitboxCoordinatesOnScreen(ref countNonRender, objectHitBox, currentBox, observer);

        return VertexToArray(objectHitBox, currentBox, coordinateScreen, countNonRender);
    }
    /// <summary>Building the edges of a hitbox</summary>
    /// <param name="objectHitBox">Info about render hitBox</param>
    /// <param name="observer">Info about observer</param>
    public static List<VertexArray> BuildHitBoxMesh(RenderInfo objectHitBox, ObserverInfo observer)
    {
        List<VertexArray> vertexArrays = new List<VertexArray>();
        HitBox hitBox = objectHitBox.hitBox;

        vertexArrays.Add(BuildBoxMesh(objectHitBox, hitBox.MainHitBox, observer));
        hitBox.SegmentedHitbox.ForEach(b => vertexArrays.Add(BuildBoxMesh(objectHitBox, b, observer)));

        return vertexArrays;
    }
}
