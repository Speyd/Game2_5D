using MapLib;
using ScreenLib;
using SFML.System;
using SFML.Window;
using System;
using System.Drawing;
using System.Reflection.Metadata;
using EntityLib;
using ControlLib.Pressed;
using MiniMapLib.SettingMap;
using MapLib.Obstacles.DiversityObstacle;
using Render.InterfaceRender;
using SFML.Graphics;
using MapLib.Obstacles;
using SixLabors.ImageSharp;
using System.Collections.Generic;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Render.ResultAlgorithm;

namespace ControlLib
{
    public class HitPoint
    {
        public Vector2f UV { get; set; } // UV-координаты на текстуре
        public double Distance { get; set; } // Расстояние до точки пересечения

        public HitPoint(Vector2f uv, double distance)
        {
            UV = uv;
            Distance = distance;
        }
    }
    public class Control
    {
        private Setting setting;
        private CheckPressed checkPressed = new CheckPressed();
        private Collision collision;
        private Screen screen;
        private Map map;
        private MiniMapLib.SettingMap.Setting settingMiniMap;

        public Control(Map map, Screen screen, MiniMapLib.SettingMap.Setting settingMiniMap,
            float minDistanceFromWall = 50, float mouseSensitivity = 0.001f)
        {
            this.screen = screen;
            this.settingMiniMap = settingMiniMap;
            this.map = map;

            setting = new Setting(minDistanceFromWall, mouseSensitivity);
            collision = new Collision(screen, map, setting);

            screen.Window.SetMouseCursorVisible(false);
            screen.Window.MouseMoved += OnMouseMoved;
        }

        #region Mouse
        private void setAngleMouse(Vector2i currentMousePosition)
        {
            int actualMousePositionX = currentMousePosition.X - screen.Setting.HalfWidth;
            setting.angle += actualMousePositionX * setting.mouseSensitivity;
        }
        private void setVerticalAngleMouse(Vector2i currentMousePosition)
        {
            int actualMousePositionY = currentMousePosition.Y - screen.Setting.HalfHeight;
            setting.verticalAngle = (float)Math.Clamp(setting.verticalAngle, -Math.PI / 2, Math.PI / 2);
            setting.verticalAngle += actualMousePositionY * setting.mouseSensitivity;
        }

        private void OnMouseMoved(object sender, MouseMoveEventArgs e)
        {
            if (!setting.isMouseCaptured)
                return;

            Vector2i currentMousePosition = new Vector2i(e.X, e.Y);
            Mouse.SetPosition(new Vector2i(screen.Setting.HalfWidth, screen.Setting.HalfHeight), screen.Window);

            setAngleMouse(currentMousePosition);
            setVerticalAngleMouse(currentMousePosition);
        }
        #endregion
        private void move(Entity entity, double directionX, double directionY)
        {
            double rx = Math.Cos(entity.getEntityA()) * directionX - Math.Sin(entity.getEntityA()) * directionY;
            double ry = Math.Sin(entity.getEntityA()) * directionX + Math.Cos(entity.getEntityA()) * directionY;

            collision.isCollision(rx * setting.moveSpeed, ry * setting.moveSpeed, entity);
        }
    
        private void turnAngle(ref double playerA, int direction)
        {
            playerA -= setting.moveSpeedAngel * direction;
        }
        //public (TexturedWall? wall, float textureX, float dist)? CastRay(float startX, float startY, float angle, Entity entity)
        //{
        //    // Направление луча
        //    float dx = (float)Math.Cos(angle);
        //    float dy = (float)Math.Sin(angle);

        //    // Размер тайла (ScreenTile)
        //    int tileSize = screen.Setting.Tile;
        //   // Console.WriteLine(tileSize);

        //    // Преобразуем координаты игрока в кратные размеру тайла
        //    int x = (int)(startX / tileSize) * tileSize;
        //    int y = (int)(startY / tileSize) * tileSize;

        //    // Определяем шаги по X и Y
        //    int stepX = (dx > 0) ? tileSize : -tileSize;
        //    int stepY = (dy > 0) ? tileSize : -tileSize;

        //    // tMaxX и tMaxY — время до первой границы тайла
        //    float tMaxX = (stepX > 0 ? (x + tileSize - startX) : (startX - x)) / Math.Abs(dx);
        //    float tMaxY = (stepY > 0 ? (y + tileSize - startY) : (startY - y)) / Math.Abs(dy);

        //    // tDeltaX и tDeltaY — шаги времени для каждой оси
        //    float tDeltaX = tileSize / Math.Abs(dx);
        //    float tDeltaY = tileSize / Math.Abs(dy);

        //    // Храним точку пересечения для вычисления текстурной координаты
        //    float hitX = startX, hitY = startY;

        //    // Трассировка луча
        //    while (true)
        //    {
        //        // Проверяем, есть ли стена на текущих координатах
        //        if (map.Obstacles.TryGetValue((x, y), out Obstacle obstacle))
        //        {
        //            if (obstacle is TexturedWall wall)
        //            {
        //                // Если попали в стену, вычисляем точку пересечения
        //                if (tMaxX < tMaxY)
        //                {
        //                    hitX = x + (stepX > 0 ? 0 : tileSize);  // Левая или правая граница
        //                    hitY = startY + (tMaxX * dy);
        //                }
        //                else
        //                {
        //                    hitX = startX + (tMaxY * dx);
        //                    hitY = y + (stepY > 0 ? 0 : tileSize);  // Верхняя или нижняя граница
        //                }

        //                // Вычисляем текстурную координату X
        //                float dist = (float)Math.Sqrt(Math.Pow(wall.X - (float)entity.getEntityX(), 2) + Math.Pow(wall.Y - (float)entity.getEntityY(), 2));
        //                float textureX;
        //                if (tMaxX < tMaxY)
        //                {
        //                    // Пересечение с вертикальной стеной
        //                    textureX = ((hitY ) % tileSize) / tileSize;
        //                    Console.WriteLine("$$");
        //                    Console.WriteLine($"{textureX}");
        //                    textureX *= wall.TextureObst.TextureWidth / screen.Setting.Scale;
        //                    textureX = Math.Clamp(textureX, 0f, wall.TextureObst.TextureWidth);

        //                   // textureX = wall.TextureObst.TextureWidth - textureX;

        //                }
        //                else
        //                {
        //                    // Пересечение с горизонтальной стеной
        //                    textureX = ((hitX / 2.75f) % tileSize) / tileSize;
        //                    textureX *= wall.TextureObst.TextureWidth / screen.Setting.Scale;
        //                    textureX = Math.Clamp(textureX, 0f, wall.TextureObst.TextureWidth);
        //                    Console.WriteLine("||");
        //                    Console.WriteLine(dist);
        //                }

        //                // Преобразуем текстурную координату X в диапазон [0, TextureWidth]
        //                //textureX *= wall.TextureObst.TextureWidth / screen.Setting.Scale;

        //               // textureX = Math.Clamp(textureX, 0f, wall.TextureObst.TextureWidth);

        //                //double dist = Math.Sqrt(Math.Pow(wall.X - (float)entity.getEntityX(), 2) + Math.Pow(wall.Y - (float)entity.getEntityY(), 2));
        //                //Console.WriteLine($"Попали в стену на: ({x}, {y}), Текстурная X: {textureX}");

        //                return (wall, textureX, (float)dist);
        //            }
        //        }

        //        // Продвигаем луч
        //        if (tMaxX < tMaxY)
        //        {
        //            tMaxX += tDeltaX;
        //            x += stepX;
        //        }
        //        else
        //        {
        //            tMaxY += tDeltaY;
        //            y += stepY;
        //        }

        //        // Проверяем выход за границы карты
        //        if (x < 0 || y < 0 ||
        //            x >= map.Setting.MapWidth * tileSize ||
        //            y >= map.Setting.MapHeight * tileSize)
        //        {
        //            return null;  // Луч вышел за пределы карты
        //        }
        //    }
        //}
        static float xx = 0; 
        static float yy = 0;
        static float tY = 1;
        static float ss = 1;
        public static float Lerp(float a, float b, float t)
        {
            return a + (b - a) * t;
        }
        //public (TexturedWall? wall, float textureX, float dist)? CastRay(float startX, float startY, float angle, Entity entity)
        //{
        //    float dx = (float)Math.Cos(angle);
        //    float dy = (float)Math.Sin(angle);

        //    int tileSize = screen.Setting.Tile;
        //    int x = (int)(startX / tileSize) * tileSize;
        //    int y = (int)(startY / tileSize) * tileSize;

        //    int stepX = (dx > 0) ? tileSize : -tileSize;
        //    int stepY = (dy > 0) ? tileSize : -tileSize;

        //    float tMaxX = (stepX > 0 ? (x + tileSize - startX) : (startX - x)) / Math.Abs(dx);
        //    float tMaxY = (stepY > 0 ? (y + tileSize - startY) : (startY - y)) / Math.Abs(dy);

        //    float tDeltaX = tileSize / Math.Abs(dx);
        //    float tDeltaY = tileSize / Math.Abs(dy);

        //    float hitX = startX, hitY = startY;

        //    while (true)
        //    {
        //        if (map.Obstacles.TryGetValue((x, y), out Obstacle obstacle))
        //        {
        //            if (obstacle is TexturedWall wall)
        //            {
        //                float dist = (float)Math.Sqrt(Math.Pow(wall.X - entity.getEntityX(), 2) +
        //                                              Math.Pow(wall.Y - entity.getEntityY(), 2));
        //                float textureX;

        //                if (tMaxX < tMaxY) // Пересечение с вертикальной стеной
        //                {
        //                    hitX = x + (stepX > 0 ? 0 : tileSize);     
        //                    hitY = startY + (tMaxX * dy);
        //                    yy = hitY;
        //                    // Если луч идет справа налево, инвертируем текстурную координату
        //                    //textureX = (dy > 0) ? 1 - ((hitY - xx) % tileSize) / tileSize
        //                    //
        //                    if (stepX > 0)
        //                        textureX = 1 - (((hitY) % tileSize) / tileSize);
        //                    else
        //                        textureX = (((hitY - xx) % tileSize) / tileSize) / (dist / map.Setting.ScreenTile / 2);
        //                    //if (textureX < 0.5f)
        //                    //    textureX *= 2f;
        //                    tY = textureX;
        //                    Console.WriteLine("$$");

        //                }
        //                else // Пересечение с горизонтальной стеной
        //                {
        //                    hitX = startX + (tMaxY * dx);
        //                    hitY = y + (stepY > 0 ? 0 : tileSize);

        //                    xx = hitX;
        //                    Console.WriteLine("||");
        //                    ss = stepY;
        //                    if (stepY > 0)
        //                    {
        //                        textureX = ((hitX - yy) % tileSize) / tileSize;
        //                        if (yy > hitX)
        //                            textureX = 1 - textureX;
        //                    }
        //                    else
        //                        textureX = ((hitX - yy) % tileSize) / tileSize;
        //                }

        //                textureX *= wall.TextureObst.TextureWidth / screen.Setting.Scale;
        //                textureX /= (dist / map.Setting.ScreenTile);
        //                textureX = Math.Clamp(textureX, 0f, wall.TextureObst.TextureWidth);

        //                return (wall, textureX, dist);
        //            }
        //        }

        //        if (tMaxX < tMaxY)
        //        {
        //            tMaxX += tDeltaX;
        //            x += stepX;
        //        }
        //        else
        //        {
        //            tMaxY += tDeltaY;
        //            y += stepY;
        //        }

        //        if (x < 0 || y < 0 ||
        //            x >= map.Setting.MapWidth * tileSize ||
        //            y >= map.Setting.MapHeight * tileSize)
        //        {
        //            return null;
        //        }
        //    }
        //}
        //public (TexturedWall? wall, float textureX, float dist)? CastRay(float startX, float startY, float angle, Entity entity)
        //{
        //    float dx = (float)Math.Cos(angle);
        //    float dy = (float)Math.Sin(angle);
        //    float length = (float)Math.Sqrt(dx * dx + dy * dy);
        //    dx /= length;
        //    dy /= length;


        //    int tileSize = screen.Setting.Tile;
        //    int x = (int)(startX / tileSize) * tileSize;
        //    int y = (int)(startY / tileSize) * tileSize;

        //    int stepX = (dx > 0) ? tileSize : -tileSize;
        //    int stepY = (dy > 0) ? tileSize : -tileSize;

        //    float tMaxX = (stepX > 0 ? (x + tileSize - startX) : (startX - x)) / Math.Abs(dx);
        //    float tMaxY = (stepY > 0 ? (y + tileSize - startY) : (startY - y)) / Math.Abs(dy);


        //    float tDeltaX = tileSize / Math.Abs(dx);
        //    float tDeltaY = tileSize / Math.Abs(dy);

        //    float hitX = startX, hitY = startY;

        //    while (true)
        //    {
        //        if (map.Obstacles.TryGetValue((x, y), out Obstacle obstacle))
        //        {
        //            if (obstacle is TexturedWall wall)
        //            {
        //                // Расчет точки пересечения и расстояния
        //                float dist;
        //                float textureX;

        //                if (tMaxX < tMaxY) // Пересечение с вертикальной стеной
        //                {
        //                    hitX = x + (stepX > 0 ? 0 : tileSize);
        //                    hitY = startY + tMaxX * dy;
        //                    // Проекционное расстояние до пересечения с вертикальной стеной
        //                    dist = Math.Abs((hitX - startX) / dx);

        //                    // Текстурная координата по вертикальной границе
        //                    textureX = (hitY % tileSize) / tileSize;
        //                    if (stepX > 0) textureX = 1 - textureX;  // Инвертируем при необходимости

        //                }
        //                else // Пересечение с горизонтальной стеной
        //                {
        //                    hitX = startX + tMaxY * dx;
        //                    hitY = y + (stepY > 0 ? 0 : tileSize);
        //                    // Проекционное расстояние до пересечения с горизонтальной стеной
        //                    dist = Math.Abs((hitY - startY) / dy);

        //                    // Текстурная координата по горизонтальной границе
        //                    textureX = (hitX % tileSize) / tileSize;
        //                    if (stepY < 0) textureX = 1 - textureX;  // Инвертируем при необходимости

        //                }
        //                float a = dist / map.Setting.ScreenTile;
        //                //textureX *= (wall.TextureObst.TextureWidth / a) / (screen.Setting.Scale / a);
        //                textureX *= (wall.TextureObst.TextureWidth ) / (screen.Setting.Scale);
        //                textureX = Math.Clamp(textureX, 0f, wall.TextureObst.TextureWidth);

        //                Console.WriteLine($"Current Position: ({startX}, {startY})");
        //                Console.WriteLine($"Direction: (dx: {dx}, dy: {dy})");
        //                Console.WriteLine($"tMaxX: {tMaxX}, tMaxY: {tMaxY}");
        //                Console.WriteLine($"Hit Position: ({hitX}, {hitY})");
        //                Console.WriteLine($"Distance: {dist}");
        //                Console.WriteLine($"Texture X: {textureX}");
        //                return (wall, textureX, dist);
        //            }
        //        }

        //        // Продолжаем следовать лучом
        //        if (tMaxX < tMaxY)
        //        {
        //            tMaxX += tDeltaX;
        //            x += stepX;
        //        }
        //        else
        //        {
        //            tMaxY += tDeltaY;
        //            y += stepY;
        //        }



        //        // Проверка выхода за границы карты
        //        if (x < 0 || y < 0 ||
        //            x >= map.Setting.MapWidth * tileSize ||
        //            y >= map.Setting.MapHeight * tileSize)
        //        {
        //            return null;
        //        }
        //    }
        //}

        //private Vector2f calculateTextureHitPoint(Entity player, TexturedWall wall)
        //{
        //    // Получаем координаты игрока и его угол направления
        //    double playerX = player.getEntityX();
        //    double playerY = player.getEntityY();
        //    double playerAngle = player.getEntityA(); // Угол направления игрока в радианах

        //    // Координаты стены
        //    float wallX = (float)wall.X;
        //    float wallY = (float)wall.Y;

        //    // Вычисляем разницу между игроком и стеной
        //    float deltaX = wallX - (float)playerX;
        //    float deltaY = wallY - (float)playerY;

        //    // Угол между направлением игрока и стеной
        //    double angleToWall = Math.Atan2(deltaY, deltaX);
        //    double relativeAngle = playerAngle - angleToWall;

        //    // Находим дистанцию от игрока до точки пересечения с поверхностью стены
        //    double distanceToWall = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

        //    // Находим точку пересечения на поверхности стены в мировых координатах
        //    float hitX = (float)(playerX + Math.Cos(playerAngle) * distanceToWall);
        //    float hitY = (float)(playerY + Math.Sin(playerAngle) * distanceToWall);

        //    // Рассчитываем локальные координаты на текстуре
        //    float localX = (hitX - wallX) / screen.Setting.Tile;
        //    float localY = (hitY - wallY) / screen.Setting.Tile;

        //    // Приводим к диапазону [0, 1] для координат текстуры (UV-координаты)
        //    float u = Math.Clamp(localX, 0f, 1f);
        //    float v = Math.Clamp(localY, 0f, 1f);

        //    return new Vector2f(u, v); // Возвращаем UV-координаты на текстуре
        //}
        private enum WallSide
        {
            Error,
            Left,//
            Right,
            Top,//
            Bottom
        }

        // Определение стороны стены, на которую смотрит игрок
        private WallSide DetermineWallSide(Entity player, TexturedWall wall)
        {
            // Получаем координаты игрока
            double playerX = player.getEntityX();
            double playerY = player.getEntityY();

            // Координаты стен
            float wallLeft = (float)wall.X; // Левый край
            float wallRight = (float)wall.X + screen.Setting.Tile; // Правый край
            float wallTop = (float)wall.Y; // Верхний край
            float wallBottom = (float)wall.Y + screen.Setting.Tile; // Нижний край

            // Угол направления игрока
            double playerAngle = player.getEntityA(); // Угол в радианах
            double directionX = Math.Cos(playerAngle);
            double directionY = Math.Sin(playerAngle);

            // Проверка на какую сторону игрок смотрит
            // Используем угол взгляда и сравниваем с границами стены

            // Если игрок смотрит вниз и его Y ниже верхней границы стены
            if (playerY < wallBottom && playerY > wallTop)
            {
                if (directionX > 0 && playerX < wallRight) return WallSide.Right; // Смотрит вправо
                if (directionX < 0 && playerX > wallLeft) return WallSide.Left;   // Смотрит влево
            }

            // Если игрок смотрит в сторону Y
            if (playerX < wallRight && playerX > wallLeft)
            {
                if (directionY > 0 && playerY < wallBottom) return WallSide.Bottom; // Смотрит вниз
                if (directionY < 0 && playerY > wallTop) return WallSide.Top;       // Смотрит вверх
            }
            return WallSide.Error;
        }
        private Vector2f CalculateTextureHitPoint(Entity player, TexturedWall wall)
        {
            // Получаем координаты игрока и его угол направления
            double playerX = player.getEntityX();
            double playerY = player.getEntityY();
            double playerAngle = player.getEntityA(); // Угол направления игрока в радианах

            // Границы стены
            float wallLeft = (float)wall.X; // Левый край
            float wallRight = (float)wall.X + screen.Setting.Tile; // Правый край
            float wallTop = (float)wall.Y; // Верхний край
            float wallBottom = (float)wall.Y + screen.Setting.Tile; // Нижний край


            // Уравнение линии взгляда
            float t = float.MaxValue; // Начальное значение для t

            // Проверка пересечения с вертикальными границами
            if (Math.Cos(playerAngle) != 0) // Избегаем деления на ноль
            {
                float tVerticalLeft = (wallLeft - (float)playerX) / (float)Math.Cos(playerAngle);
                float tVerticalRight = (wallRight - (float)playerX) / (float)Math.Cos(playerAngle);

                float hitYLeft = (float)(playerY + tVerticalLeft * Math.Sin(playerAngle));
                float hitYRight = (float)(playerY + tVerticalRight * Math.Sin(playerAngle));

                // Проверяем, находится ли точка на стене
                if (hitYLeft >= wallTop && hitYLeft <= wallBottom && tVerticalLeft >= 0)
                {
                    t = tVerticalLeft; // Сохраняем t для левой границы
                }

                if (hitYRight >= wallTop && hitYRight <= wallBottom && tVerticalRight >= 0)
                {
                    t = Math.Min(t, tVerticalRight); // Сохраняем минимальное значение t для правой границы
                }
            }

            // Проверка пересечения с горизонтальными границами
            if (Math.Sin(playerAngle) != 0) // Избегаем деления на ноль
            {
                float tHorizontalTop = (wallTop - (float)playerY) / (float)Math.Sin(playerAngle);
                float tHorizontalBottom = (wallBottom - (float)playerY) / (float)Math.Sin(playerAngle);

                float hitXTop = (float)(playerX + tHorizontalTop * Math.Cos(playerAngle));
                float hitXBottom = (float)(playerX + tHorizontalBottom * Math.Cos(playerAngle));

                // Проверяем, находится ли точка на стене
                if (hitXTop >= wallLeft && hitXTop <= wallRight && tHorizontalTop >= 0)
                {
                    t = Math.Min(t, tHorizontalTop); // Сохраняем минимальное значение t для верхней границы
                }

                if (hitXBottom >= wallLeft && hitXBottom <= wallRight && tHorizontalBottom >= 0)
                {
                    t = Math.Min(t, tHorizontalBottom); // Сохраняем минимальное значение t для нижней границы
                }
            }

            // Если t осталось максимально, значит нет пересечения
            if (t == float.MaxValue)
            {
                return new Vector2f(-1, -1); // Значит, пересечение не найдено
            }

            // Рассчитываем точку пересечения
            float hitX = (float)(playerX + t * Math.Cos(playerAngle));
            float hitY = (float)(playerY + t * Math.Sin(playerAngle));

            // Логика для получения UV-координат на текстуре...

            // Возвращаем UV-координаты
            return new Vector2f(hitX, hitY);
        }

        private HitPoint calculateTextureHitPoint(Entity player, TexturedWall wall)
        {
            // Получаем координаты игрока и его угол направления
            double playerX = player.getEntityX();
            double playerY = player.getEntityY();
            double playerAngle = player.getEntityA(); // Угол направления игрока в радианах

            // Координаты стены
            float wallX = (float)wall.X;
            float wallY = (float)wall.Y;

            // Вычисляем разницу между игроком и стеной
            float deltaX = wallX - (float)playerX;
            float deltaY = wallY - (float)playerY;

            // Угол между направлением игрока и стеной
            double angleToWall = Math.Atan2(deltaY, deltaX);
            double relativeAngle = playerAngle - angleToWall;

            // Находим дистанцию до точки пересечения с поверхностью стены
            double distanceToWall = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

            WallSide w = DetermineWallSide(player, wall);

            if (w == WallSide.Top || w == WallSide.Left)
                distanceToWall -= map.Setting.ScreenTile;

            Console.WriteLine(w.ToString());
            // Находим точку пересечения на поверхности стены в мировых координатах
            float hitX = (float)(playerX + Math.Cos(playerAngle) * distanceToWall);
            float hitY = (float)(playerY + Math.Sin(playerAngle) * distanceToWall);



            // Рассчитываем локальные координаты на текстуре
            float localX = hitX - wallX; // Корректируем локальные координаты по X
            float localY = hitY - wallY; // Корректируем локальные координаты по Y

            // Обрабатываем локальные координаты, чтобы избежать отрицательных значений
            float textureWidth = screen.Setting.Tile; // Ширина текстуры
            float textureHeight = screen.Setting.Tile; // Высота текстуры

            // Учитываем сторону стен

            // Корректируем локальные координаты
            if (w == WallSide.Left || w == WallSide.Right)
            {
                // Инвертируем Y для вертикальных стен
                localY = (localY % textureHeight + textureHeight) % textureHeight;
            }
            else
            {
                // Инвертируем X для горизонтальных стен
                localX = (localX % textureWidth + textureWidth) % textureWidth;
            }

            // Приводим локальные координаты к диапазону [0, 1] для UV-координат
            float u = (localX / textureWidth);
            float v = (localY / textureHeight);
           
            // В зависимости от стороны стены
            if (w == WallSide.Left)
            {
                u = 1 - u; // Инвертируем u для левой стороны
            }
            else if (w == WallSide.Right)
            {
                // Здесь можно оставить как есть или применить дополнительные преобразования
            }
            else if (w == WallSide.Top)
            {
                u = (localX % textureWidth + textureWidth) % textureWidth / textureWidth;
                v = 0;
            }
            else if (w == WallSide.Bottom)
            {

            }
            else
            {
                Vector2f r = CalculateTextureHitPoint(player, wall);

                if (r.X > r.Y)
                {
                    r.X = (r.X - 700) / 100;
                    r.Y = r.Y - 700;

                    if (r.X < r.Y)
                    {
                        r.Y /= 100;
                        r.X = 0;
                    }
                }
                else
                {
                    r.Y = (r.Y - 700) / 100;
                    r.X = r.X - 700;

                    if(r.X > r.Y)
                    {
                        r.X /= 100;
                        r.Y = 0;
                    }
                }
                return new HitPoint(r, distanceToWall);
            }



            Console.WriteLine(w.ToString()); 
            return new HitPoint(new Vector2f(u, v), distanceToWall); // Возвращаем UV-координаты на текстуре
        }



        private (float u, float v) CalculateTextureCoordinatesWithAngle(
    double hitX, double hitY, bool isVertical, TexturedWall wall)
        {
            // Локальные координаты попадания
            float localX = (float)(hitX - wall.X);
            float localY = (float)(hitY - wall.Y);

            // Выбор текстурной координаты в зависимости от пересечения
            float textureCoordinate = isVertical ? localY : localX;

            // Применяем корректировку для отрицательных значений
            float tileSize = screen.Setting.Tile;
            textureCoordinate = (textureCoordinate % tileSize + tileSize) % tileSize;

            // Преобразуем в UV-координаты (0.0 - 1.0)
            float u = textureCoordinate / tileSize;
            float v = (localY % tileSize + tileSize) % tileSize / tileSize;

            return (u, v);
        }

        public (double hitX, double hitY, bool isVertical) Raycast(
    double playerX, double playerY, double rayAngle, TexturedWall wall)
        {
            double rayDirX = Math.Cos(rayAngle);
            double rayDirY = Math.Sin(rayAngle);

            // Расчет дистанции до ближайших граней
            double deltaX = (wall.X - playerX) / rayDirX;
            double deltaY = (wall.Y - playerY) / rayDirY;

            bool isVertical = deltaX < deltaY;

            // Берем ближайшее пересечение
            double tHit = isVertical ? deltaX : deltaY;

            // Вычисляем точные координаты пересечения
            double hitX = playerX + tHit * rayDirX;
            double hitY = playerY + tHit * rayDirY;

            return (hitX, hitY, isVertical);
        }
        private void calculateHitPoint(Entity entity)
        {
            double playerX = entity.getEntityX();
            double playerY = entity.getEntityY();

            // Угол центрального луча
            double angle = entity.getEntityA();


            



            if (map.Obstacles[(700, 700)] is TexturedWall wall)
            {
                (double hitX, double hitY, bool isVertical) coo = Raycast(playerX, playerY, angle, wall);

                //(float X, float Y) vec = CalculateTextureCoordinatesWithAngle(coo.hitX, coo.hitY, coo.isVertical, wall);
                ////wall.renderTexture.Draw();
                //Console.WriteLine($"X: {vec.X},  Y: {vec.Y}");
                //float textureX = vec.X;
                //textureX *= (wall.TextureObst.TextureWidth) / (screen.Setting.Scale);

                HitPoint vec = calculateTextureHitPoint(entity, wall);
                //Console.WriteLine($"X: {vec.X},  Y: {vec.Y}");

                float textureX = vec.UV.X > vec.UV.Y ? vec.UV.X : vec.UV.Y;
                textureX *= (wall.TextureObst.TextureWidth) / (screen.Setting.Scale);


                float deltaX = (float)wall.X - (float)playerX;
                float deltaY = (float)wall.Y - (float)playerY;

                float distanceToWall = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                // 2. Высота проекции на экране
                float ProjHeight = Math.Min((float)(entity.ProjCoeff / vec.Distance), 8 * screen.ScreenHeight);

                // 3. Коэффициент масштабирования для текстуры
                float scaleY = ProjHeight / wall.TextureObst.TextureHeight;

                float adjustedDistance = (float)vec.Distance / map.Setting.ScreenTile;
                float mult = (adjustedDistance * adjustedDistance) / 2.5f;
                float a = (float)entity.getEntityVerticalA();
                if (a > 0)
                {
                    mult += 0.2f;
                    mult /= a + (float)Math.Floor((Math.PI / 2) * 10) / 10;
                }


                float adjustedA = a / adjustedDistance;

                float Y = ProjHeight * a * mult;

                // Если нужно, можно добавить еще одно влияние от distanceToWall
                /* Y *= (1.0f / (1 + adjustedDistance));*/ // Увеличиваем смещение

                // Y += 45;


                //if (a > 0 && a < 0.2f)
                //    Y += 30 * a;
                // 45 это радиус точки(30) + поовина от радиуса(15)
                // 5. Вертикальная координата на текстуре с поправкой на масштаб
                //float textureY = (wall.TextureObst.TextureHeight * (Y / ProjHeight));

                Console.WriteLine(mult);
                Vector2f dotPosition = new Vector2f(textureX, wall.TextureObst.TextureHeight / 2 + Y);

                CircleShape dot = new CircleShape(30)
                {
                    FillColor = SFML.Graphics.Color.Black,
                    Position = dotPosition
                };
                wall.renderTexture.Draw(dot);
                wall.renderTexture.Display();
               // Console.WriteLine(textureX);
            }
            //(TexturedWall? wall, float textureX, float dist)? temp = CastRay((float)playerX, (float)playerY, (float)angle, entity);
            //if (temp is not null && temp.Value.wall is not null)
            //{
            //   DrawBlackDot(temp.Value.textureX, temp.Value.dist, entity, temp.Value.wall); // Передаем UV-координаты в метод рисования
            //}
        }

        float SmoothStep(float edge0, float edge1, float t)
        {
            // Сжимаем t в диапазон [0,1]
            t = Math.Clamp((t - edge0) / (edge1 - edge0), 0f, 1f);
            // Применяем нелинейное замедление (квадратичная функция)
            return t * t * (3f - 2f * t);
        }
        float LogarithmicFactor(float value, float scale = 10f)
        {
            return (float)Math.Log(1 + value) / (float)Math.Log(1 + scale);
        }

        private void DrawBlackDot(float x, float dist, Entity entity, TexturedWall wall)
        {
            float heiht = 1308;
            float dynamicRatioY;
            if (heiht < wall.TextureObst.TextureHeight)
            {
                dynamicRatioY = (wall.TextureObst.TextureHeight / heiht);
                dynamicRatioY = dynamicRatioY > 1.5f ? dynamicRatioY / (dynamicRatioY - 0.1f) : dynamicRatioY + 0.05f;
            }
            else
                dynamicRatioY = heiht / wall.TextureObst.TextureHeight;



            int width = 1920;

            
            float centerY = wall.TextureObst.TextureHeight / 2f; // Центр Y текстуры
            float centerX = wall.TextureObst.TextureWidth / 2f;
            float a = (float)entity.getEntityVerticalA();// Math.Clamp((float)entity.getEntityVerticalA(), -1f, 1f);
            // Увеличиваем скорость изменения вертикальной позиции с учетом угла взгляда игрока

            float speedFactor = 10.15f / dynamicRatioY;

            if (a > 0)
                speedFactor *= a + 1;

            float distanceFactor = (((float)entity.ProjCoeff / dist) / (wall.TextureObst.TextureHeight)) / dynamicRatioY;

            float offsetY = (float)(a / speedFactor * (wall.TextureObst.TextureHeight * dynamicRatioY) * (dist / map.Setting.ScreenTile) / (Math.PI * distanceFactor));
            float yC = offsetY * dynamicRatioY + centerY;

            //float distanceFactorX = (float)wall.TextureObst.TextureScale * (dist / map.Setting.ScreenTile) / ( wall.TextureObst.TextureWidth);
            //Console.WriteLine(x);
            float mul = centerX > x ? 0.8f : 0.8f;
            //if (ss > 0)
            //   x += (x / (tY / 0.5f)));
            //Console.WriteLine(x);
            Vector2f dotPosition = new Vector2f(x - 30, yC - 30);

            CircleShape dot = new CircleShape(30)
            {
                FillColor = SFML.Graphics.Color.Black,
                Position = dotPosition
            };

            wall.renderTexture.Draw(dot);
            wall.renderTexture.Display();

            
        
    }



        public void makePressed(double deltaTime, Entity entity)
        {
            double tempMoveSpeed = (100 * deltaTime);

            entity.getEntityA() = setting.angle % (2 * Math.PI);
            entity.getEntityVerticalA() = setting.verticalAngle;

            setting.moveSpeed = (float)(tempMoveSpeed - Math.Min(tempMoveSpeed - 0.6, (screen.Setting.AmountRays / screen.ScreenWidth)));
            setting.moveSpeedAngel = 1 * deltaTime;

            checkPressed.check();
            if (checkPressed.CurrentDirection.Forward)
                move(entity, 1, 0);
            if (checkPressed.CurrentDirection.Backward)
                move(entity, -1, 0);
            if (checkPressed.CurrentDirection.Left)
                move(entity, 0, -1);
            if (checkPressed.CurrentDirection.Right)
                move(entity, 0, 1);

            if (checkPressed.CurrentDirection.TurnLeft)
                turnAngle(ref setting.angle, -1);
            if (checkPressed.CurrentDirection.TurnRight)
                turnAngle(ref setting.angle, 1);

            if (checkPressed.CurrentDirection.ZoomMiniMap)
                settingMiniMap.Zoom += 0.01f;
            if (checkPressed.CurrentDirection.ReduceMiniMap)
                settingMiniMap.Zoom -= 0.01f;

            if (Mouse.IsButtonPressed(Mouse.Button.Left))
            {
                calculateHitPoint(entity);
            }

            if (checkPressed.CurrentDirection.Exit)
                screen.Window.Close();

        }
    }
}
