using ScreenLib;
using SFML.Graphics;
using SFML.System;
using ProtoRender.Map;
using ProtoRender.Object;

namespace MiniMapLib.ObjectInMap.Obstacles;
partial class ObstacleOutput
{
    private bool IsOutOfBounds(RenderTexture window, RectangleShape rect)
    {
        Vector2f pos = rect.Position;
        Vector2f size = rect.Size;
        Vector2u windowSize = window.Size;

        return (pos.X + size.X < 0 || pos.Y + size.Y < 0 ||
                pos.X > windowSize.X || pos.Y > windowSize.Y);
    }
    private void UpdateMaterial(RectangleShape RectangleShape, IMiniMapRenderable obstacle)
    {
        switch (OutputRenderMethod)
        {
            case OutputRenderMethod.Texture:
                obstacle.FillingTextureShape(RectangleShape); break;
            case OutputRenderMethod.Color:
                obstacle.FillingColorShape(RectangleShape, OutLine); break;
        }
    }



    private void SetRenderDelegate()
    {
        switch (_renderMode)
        {
            case DisplayRenderMode.EntireArea:
                RenderDelegate = RenderEntireArea; break;
            case DisplayRenderMode.SpecificArea:
                RenderDelegate = RenderSpecificArea; break;
            case DisplayRenderMode.UnitVisibilityArea:
                RenderDelegate = RenderUnitVisibilityArea; break;
            default:
                RenderDelegate = RenderEntireArea; break;
        }
    }
    private void SetMapСoordinatesUnit(IUnit unit)
    {
        MapPlayer.X = (int)(unit.X.Axis / Setting.Scale);
        MapPlayer.Y = (int)(unit.Y.Axis / Setting.Scale);
    }
    private void SetRectangleShape(RenderTexture Window,
        Vector2f mapObstacle, IMiniMapRenderable obstacle,
        Vector2f normalizator, bool IsParallel = false)
    {
        Vector2f sizeNormalization = obstacle.SizeOffsetMap(normalizator);
        if (float.IsNaN(sizeNormalization.X) || float.IsNaN(sizeNormalization.Y) ||
            sizeNormalization.X <= 0 || sizeNormalization.Y <= 0)
            return;

        Vector2f positionNormalization = obstacle.CoordinatesOffsetMap(normalizator);
        if (float.IsNaN(positionNormalization.X) || float.IsNaN(positionNormalization.Y))
            return;

        var shape = new RectangleShape();
        shape.Size = sizeNormalization;
        shape.Position = new Vector2f(
            (float)(Setting.CenterX - (mapObstacle.X - MapPlayer.X) - positionNormalization.X),
            (float)(Setting.CenterY - (mapObstacle.Y - MapPlayer.Y) - positionNormalization.Y)
        );

        if (IsOutOfBounds(Window, shape))
            return;

        UpdateMaterial(shape, obstacle);

        if (IsParallel)
            renderQueue.Add(shape);
        else
            Window.Draw(shape);
    }



    private void RenderSpecificArea(IUnit unit, RenderTexture Window, Vector2f mapObstacle, IMiniMapRenderable obstacle, Vector2f sizeNormalization, bool IsParallel = false)
    {
        double distance = Math.Sqrt(Math.Pow(mapObstacle.X - MapPlayer.X, 2) + Math.Pow(mapObstacle.Y - MapPlayer.Y, 2));

        if (distance <= unit.MaxRenderTile / Screen.Setting.Tile * Setting.MapTileX ||
            distance <= unit.MaxRenderTile / Screen.Setting.Tile * Setting.MapTileY)
            SetRectangleShape(Window, mapObstacle, obstacle, sizeNormalization, IsParallel);
    }
    private void RenderEntireArea(IUnit unit, RenderTexture Window, Vector2f mapObstacle, IMiniMapRenderable obstacle, Vector2f sizeNormalization, bool IsParallel = false)
    {
        SetRectangleShape(Window, mapObstacle, obstacle, sizeNormalization, IsParallel);
    }
    private void RenderUnitVisibilityArea(IUnit unit, RenderTexture Window, Vector2f mapObstacle, IMiniMapRenderable obstacle, Vector2f sizeNormalization, bool IsParallel = false)
    {
        double angleToObstacle = DataPipes.MathUtils.NormalizeAngleDifference(unit.Angle, DataPipes.MathUtils.CalculateAngleToTarget(mapObstacle, MapPlayer));

        if (Math.Abs(angleToObstacle) <= unit.HalfFov)
            RenderSpecificArea(unit, Window, mapObstacle, obstacle, sizeNormalization, IsParallel);
    }
}
