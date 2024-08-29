using ScreenLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityLib;
using RenderLib.InfoRender;
using System.Drawing;
using System.Runtime.CompilerServices;
using MapLib;
using System.Data;
using System.Reflection;

namespace RenderLib.LogicRender
{
    internal class RenderScreenLogic(Screen screen, Entity entity)
    {


        private ValueTuple<double, char> definitionBlockChar(
            ref Dictionary<double, MapLib.Object?> obj,
            ref bool isBound, double distanceToWall)
        {

            if (!obj.Any())
                return (0, ' ');


            var firstKey = obj.First().Key;
            MapLib.Object? renderObject = obj[firstKey];


            if (renderObject is null)
                return (0, ' ');

            if (isBound == true)
                return (firstKey, '|');

            foreach (var symbolValue in ListSymbolRender.symbols)
            {
                if (distanceToWall < entity.Depth / symbolValue.Key)
                {
                    return (firstKey, symbolValue.Value);
                }
            }

            return (firstKey, ' ');

        }

        private void definitionSymbolHigherObject(
            int x, int y,
            ref ValueTuple<double, MapLib.Object> renderObject,
            ref ValueTuple<double, char> symbolForRender
            )
        {
            if (symbolForRender.Item2 == '|' && symbolForRender.Item1 == renderObject.Item1)
            {
                screen.ScreenChr[y * screen.ScreenWidth + x] = symbolForRender.Item2;
                return;
            }
            foreach (var symbolValue in ListSymbolRender.symbols)
            {
                if (renderObject.Item1 < entity.Depth / symbolValue.Key)
                {
                    screen.ScreenChr[y * screen.ScreenWidth + x] = symbolValue.Value;
                    return;
                }
            }
        }

        private int getHeight(int ceiling, double keyObject, MapLib.Object? value)
        {
            return ceiling - (value.HeightExceedingMiddle - (int)(keyObject / 1.5));
        }

        #region Sort
        private ValueTuple<double, MapLib.Object?> definitionHighestObject(
            ref Dictionary<double, MapLib.Object?> objArr)
        {
            var sortObjArr = objArr.OrderByDescending(pair => pair.Value?.HeightExceedingMiddle)
                   .ToDictionary(pair => pair.Key, pair => pair.Value);

            ValueTuple<double, MapLib.Object?> maxHeightObj = new(0, new MapLib.Object(0, 0, ' ', true));
            if (sortObjArr.Count > 0)
            {
                maxHeightObj.Item1 = sortObjArr.First().Key;
                maxHeightObj.Item2 = sortObjArr.First().Value;
            }

            return maxHeightObj;

        }

        private void sortObjectArrayByDistance(
            ref Dictionary<double, MapLib.Object?> addedArray,
            ref Dictionary<double, MapLib.Object?> objArr)
        {
            var sortObjArr = objArr.OrderBy(pair => pair.Key)
                   .ToDictionary(pair => pair.Key, pair => pair.Value);
            ValueTuple<double, MapLib.Object?> maxHeightObj = definitionHighestObject(ref objArr);

            foreach (var sortObj in sortObjArr)
            {
                var valueTuple = new ValueTuple<double, MapLib.Object?>(sortObj.Key, sortObj.Value);

                if (sortObj.Key >= maxHeightObj.Item1)
                {
                    addedArray.Add(maxHeightObj.Item1, maxHeightObj.Item2);
                    break;
                }
                else
                {
                    addedArray.Add(valueTuple.Item1, valueTuple.Item2);
                }
            }
        }

        private void sortObjectArrayByHeight(
            ref Dictionary<ValueTuple<double, MapLib.Object?>, ValueTuple<int, int>> coordinates,
            ref Dictionary<double, MapLib.Object?> objArr,
            ref Dictionary<double, MapLib.Object?> sortDistanceArray,
            int ceiling)
        {
            var sortObjArr = sortDistanceArray
                                .OrderByDescending(pair => pair.Value?.HeightExceedingMiddle)
                                .ToDictionary(pair => pair.Key, pair => pair.Value);


            var keys = new List<double>(sortObjArr.Keys);
            var values = new List<MapLib.Object?>(sortObjArr.Values);
            ValueTuple<int, int> tempCords = new ValueTuple<int, int>();


            for (int count = 0; count < sortObjArr.Count; count++)
            {
                if (count % 2 == 0)
                    tempCords.Item1 = getHeight(ceiling, keys[count], values[count]);
                else if (count == 1)
                    tempCords.Item2 = getHeight(ceiling, keys[count], values[count]);

                if (count > 0 && count < sortObjArr.Count - 1)
                {
                    coordinates.Add((keys[count - 1], values[count - 1]), (tempCords.Item1, tempCords.Item2));
                    tempCords.Item2 = tempCords.Item1;
                }
                else if (count > 0 && count == sortObjArr.Count - 1)
                {
                    coordinates.Add((keys[count - 1], values[count - 1]), (tempCords.Item1, ceiling));
                }
                else if (count == 0 && count == sortObjArr.Count - 1)
                {
                    coordinates.Add((keys[count], values[count]), (tempCords.Item1, ceiling));
                }
            }
        }

        private void variableAssignments(
            ref Dictionary<ValueTuple<double, MapLib.Object?>, ValueTuple<int, int>> coordinates,
            ref ValueTuple<double, MapLib.Object?> renderObject,
            ref int blockTop, int ceiling,
            ref int blockBottom, int floor
            )
        {
            if (coordinates.Count > 0)
            {
                blockTop -= coordinates.First().Key.Item2.HeightExceedingMiddle - (int)(coordinates.First().Key.Item1 / 1.5);
                renderObject = coordinates.First().Key;
            }
        }

        #endregion

        private void fillingScreenWithSymbol(
            ref Dictionary<double, MapLib.Object?> obj, 
            ref ValueTuple<double, char> blockChar,
            int x,
            int ceiling, int floor)
        {
            Dictionary<ValueTuple<double, MapLib.Object?>, ValueTuple<int, int>> coordinate =
                new Dictionary<ValueTuple<double, MapLib.Object?>, ValueTuple<int, int>>();

            Dictionary<double, MapLib.Object?> tempCoordinate = new Dictionary<double, MapLib.Object?>();

            ValueTuple<double, MapLib.Object?> renderObject = new(0, new MapLib.Object(0, 0, ' ', true));
            int blockTop = ceiling;
            int blockBottom = floor;


            sortObjectArrayByDistance(ref tempCoordinate, ref obj);
            sortObjectArrayByHeight(ref coordinate, ref obj, ref tempCoordinate, ceiling);

            variableAssignments(ref coordinate, ref renderObject, ref blockTop, ceiling, ref blockBottom, floor);


            for (int y = 0; y < screen.ScreenHeight; y++)
            {
                if (y < blockTop)
                {
                    screen.ScreenChr[y * screen.ScreenWidth + x] = SymbolsFillingSurfaces.symbols[TypesOfSurfaces.CEILING];
                }
                else if (coordinate.ContainsKey(renderObject) && y >= coordinate[renderObject].Item1 && y < coordinate[renderObject].Item2 ||
                         coordinate.ContainsKey(renderObject) && y >= coordinate[renderObject].Item2 && y < ceiling)
                {
                    definitionSymbolHigherObject(x, y, ref renderObject, ref blockChar);
                }
                else if (y >= ceiling && y <= blockBottom)
                {
                    screen.ScreenChr[y * screen.ScreenWidth + x] = blockChar.Item2;
                }
                else
                {
                    screen.ScreenChr[y * screen.ScreenWidth + x] = SymbolsFillingSurfaces.symbols[TypesOfSurfaces.FLOOR];
                }

                if (coordinate.ContainsKey(renderObject) && y == coordinate[renderObject].Item2)
                {
                    var allKeys = coordinate.Keys.ToList();
                    int currentIndex = allKeys.IndexOf(renderObject);

                    if (currentIndex >= 0 && currentIndex < allKeys.Count - 1)
                        renderObject = allKeys[currentIndex + 1];
                    else
                        renderObject = new ValueTuple<double, MapLib.Object>(0, new MapLib.Object(0, 0, ' ', true));
                }
            }
        }

        public void render(ref Dictionary<double, MapLib.Object?> obj, int x, ref bool isBound, ref double distanceToWall)
        {
            int ceiling = (int)(screen.ScreenHeight / 2d - screen.ScreenHeight * entity.EntityFov / distanceToWall);
            int floor = screen.ScreenHeight - ceiling;

            ValueTuple<double, char> blockChar = definitionBlockChar(ref obj, ref isBound, distanceToWall);
            fillingScreenWithSymbol(ref obj, ref blockChar, x, ceiling, floor);
        }
    }
}
