using SFML.Graphics;
using System.Reflection.Metadata;
using EntityLib;
using ScreenLib;
using SFML.System;
using SFML.Window;

namespace TempBot
{
    public class Class1
    {
        private Image image = new Image(@"D:\C++ проекты\Game2_5D\576.jpg");
        private Texture text = new Texture(@"D:\C++ проекты\Game2_5D\576.jpg");

        public void Render(Entity player)
        {
            float cameraZ = 0.5f * Screen.ScreenHeight;
            byte[] floorPixels = new byte[Screen.ScreenWidth * Screen.ScreenHeight * 4];


            for (int y = Screen.ScreenHeight / 2; y < Screen.ScreenHeight; y++)
            {
                // Направления лучей для левой и правой стороны
                Vector2f rayDirLeft = player.Direction - player.Plane;
                Vector2f rayDirRight = player.Direction + player.Plane;

                // Расстояние до текущей строки
                float rowDistance = cameraZ / ((float)y - Screen.ScreenHeight / 2f);

                Vector2f floorStep = rowDistance * (rayDirRight - rayDirLeft) / Screen.ScreenWidth;

                // Начальная позиция на строке
               // Vector2f floor = player.Position + rowDistance * rayDirLeft;

                for (int x = 0; x < Screen.ScreenWidth; x++)
                {
                    // Координаты текущей клетки на полу
                    //Vector2i cell = (Vector2i)floor;


                    // Вычисление текстурных координат
                    //Vector2i texCoords = (Vector2i)(text.Size.X * (floor - (Vector2f)cell));


                    // Обрезаем координаты, чтобы они не вышли за границы текстуры
                    //texCoords.X &= (int)text.Size.X - 1;
                    //texCoords.Y &= (int)text.Size.X - 1;

                    //// Получаем цвет из текстуры
                    //Color color = image.GetPixel((uint)texCoords.X, (uint)texCoords.Y);

                    //// Заполняем массив пикселей
                    //// int pixelIndex = (x + y * Screen.ScreenWidth) * 4;
                    //floorPixels[(x + y * Screen.ScreenWidth) * 4 + 0] = color.R;
                    //floorPixels[(x + y * Screen.ScreenWidth) * 4 + 1] = color.G;
                    //floorPixels[(x + y * Screen.ScreenWidth) * 4 + 2] = color.B;
                    //floorPixels[(x + y * Screen.ScreenWidth) * 4 + 3] = color.A;

                    //// Переход к следующему пикселю
                    //floor += floorStep;
                }
            }

            //// Создаем объект Image из массива пикселей
            Image floorImage = new Image((uint)Screen.ScreenWidth, (uint)Screen.ScreenHeight, floorPixels);

            // Создаем текстуру из Image
            Texture floorTexture = new Texture(floorImage);

            // Рисуем спрайт на основе текстуры
            Sprite floorSprite = new Sprite(floorTexture);
            Screen.Window.Draw(floorSprite);

            //float ray_direction_end_x, ray_direction_end_y;
            //float ray_direction_start_x, ray_direction_start_y;

            //// Получаем направление взгляда игрока
            //float player_angle = (float)player.Angle;
            //float start_stripe_x = -(float)Math.Tan(0.5f * player.HalfFov);
            //float end_stripe_x = (float)Math.Tan((0.5f * player.HalfFov)) * (1 - 2f / Screen.ScreenWidth);
            //// Вычисляем направления для лучей
            //ray_direction_end_x = (float)player.Direction.X + end_stripe_x * (float)Math.Cos(player_angle - Math.PI / 2);
            //ray_direction_end_y = -(float)player.Direction.Y - end_stripe_x * (float)Math.Sin(player_angle - Math.PI / 2);
            //ray_direction_start_x = (float)player.Direction.X + start_stripe_x * (float)Math.Cos(player_angle - Math.PI / 2);
            //ray_direction_start_y = -(float)player.Direction.Y - start_stripe_x * (float)Math.Sin(player_angle - Math.PI / 2);

            //ushort pitch = (ushort)Math.Round(0.5f * Screen.ScreenHeight * Math.Tan(player.Direction.Y));

            //ushort floor_start_y = (ushort)Math.Clamp(pitch + 0.5f * Screen.ScreenHeight, 0, Screen.ScreenHeight);
            //Image floor_buffer_image = new Image((uint)Screen.ScreenWidth, (uint)Screen.ScreenHeight - floor_start_y);
            //for (int a = 0; a < Screen.ScreenHeight; a++)
            //{
            //    float floor_step_x;
            //    float floor_step_y;
            //    float floor_x;
            //    float floor_y;
            //    float row_distance;

            //    short row_y = (short)(a - cameraZ);

            //    // Вычисление расстояния от игрока до текущего ряда
            //    row_distance = (row_y == 0) ? float.MaxValue : cameraZ / row_y;
            //    byte shade = (byte)(255 * Math.Clamp(1 - row_distance / 1000, 0, 1));
            //    if (row_distance > 0)
            //    {
            //        // Вычисляем шаги для каждого тайла на полу
            //        floor_step_x = row_distance * (ray_direction_end_x - ray_direction_start_x) / Screen.ScreenWidth;
            //        floor_step_y = row_distance * (ray_direction_end_y - ray_direction_start_y) / Screen.ScreenWidth;

            //        // Вычисляем координаты для текущего тайла
            //        floor_x = 0.5f + player.Position.X + ray_direction_start_x * row_distance;
            //        floor_y = 0.5f + player.Position.Y + ray_direction_start_y * row_distance;

            //        for (int b = 0; b < Screen.ScreenWidth; b++)
            //        {
            //            // Получаем пиксель текстуры для пола
            //            var floor_image_pixel = image.GetPixel((uint)(floor_x * image.Size.X) & image.Size.X - 1, (uint)(floor_y * image.Size.Y) & image.Size.Y -1);
            //            floor_image_pixel *= new Color(shade, shade, shade);

            //            floor_buffer_image.SetPixel((uint)b, (uint)(a - floor_start_y), floor_image_pixel);

            //            // Применяем шаги
            //            floor_x += floor_step_x;
            //            floor_y += floor_step_y;

            //        }
            //    }
            //}

            //Screen.Window.Draw(new Sprite(new Texture(floor_buffer_image)));
        }
    }
}
