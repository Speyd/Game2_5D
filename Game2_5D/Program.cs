using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.InteropServices;
using MapLib;
using System.ComponentModel;
using System;
using System.Text;
using Konsole;
using System.Security.Principal;
using System.Linq;
using SFML.Graphics;
using SFML.Window;
using SFML.System;
using System.Numerics;

ConsoleColor GetRandomConsoleColor(Random rand)
{
    // Список доступных цветов консоли
    ConsoleColor[] colors = (ConsoleColor[])ConsoleColor.GetValues(typeof(ConsoleColor));

    // Возвращаем случайный цвет
    return colors[rand.Next(colors.Length)];
}

[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int key);

// Коды клавиш
const int VK_W = 0x57;
const int VK_S = 0x53;
const int VK_A = 0x41;
const int VK_D = 0x44;
const int VK_LEFT = 0x25;
const int VK_RIGHT = 0x27;



void getMapWorld(List<ValueTuple<int, int>> values, int TILE, Map map) 
{
    for(int i = 0; i < map.MapHeight; i++)
    {
        for(int j = 0; j < map.MapWidth; j++)
        {
            if (map.MapStr[i * map.MapWidth + j] == '#')
                values.Add((i * TILE, j * TILE));
        }
        
    }
}


const int ScreenWidth = 1900;
const int ScreenHeight = 800;
const int HELF_WIDTH = ScreenWidth / 2;
const int HELF_HEIGHT = ScreenHeight / 2;
const int TILE = 100; //РАЗМЕР КВАДРАТА КАРТЫ
const int NUM_RAYS = ScreenWidth;
const int MAX_DEPTH = 800;
const int SCALE = ScreenWidth / NUM_RAYS;

const double playerFov = Math.PI / 3;
const double HULF_FOV = playerFov / 2;
const double DELTA_ANGLE = playerFov / NUM_RAYS;

double DIST = NUM_RAYS / (2 * Math.Tan(HULF_FOV));
double PROJ_COEFF = DIST * TILE * 0.5;


double playerY = HELF_HEIGHT;
double playerX = HELF_WIDTH;
double playerA = 0;

const double Depth = 16;


Map map = new Map(12, 12);
map.addBlockToMap(2, 2);
map.addBlockToMap(2, 5);
map.addBlockToMap(7, 7);
map.addBlockToMap(7, 8);
map.addBlockToMap(7, 9);
map.addBlockToMap(7, 10);

map.addBlockToMap(9, 7);
map.addBlockToMap(9, 8);
map.addBlockToMap(9, 9);
map.addBlockToMap(9, 10);



List<ValueTuple<int, int>> values = new List<(int, int)>();
getMapWorld(values, TILE, map);

//char[] Screen = new char[ScreenHeight * ScreenWidth];

//Console.SetWindowSize(ScreenWidth, ScreenHeight);
//Console.SetBufferSize(ScreenWidth, ScreenHeight);
//Console.CursorVisible =  false;

RenderWindow window = new RenderWindow(new VideoMode(ScreenWidth, ScreenHeight), "SFML Window");

DateTime from = DateTime.Now;
VertexArray vertexArray = new VertexArray(PrimitiveType.Quads);
window.SetActive(true);
while (window.IsOpen)
{
    // Обработка событий
    window.DispatchEvents();


    DateTime dateTime = DateTime.Now;
    double elapsed = (dateTime - from).TotalSeconds;
    from = DateTime.Now;
    bool isWPressed = (GetAsyncKeyState(VK_W) & 0x8000) != 0;
    bool isSPressed = (GetAsyncKeyState(VK_S) & 0x8000) != 0;
    bool isAPressed = (GetAsyncKeyState(VK_A) & 0x8000) != 0;
    bool isDPressed = (GetAsyncKeyState(VK_D) & 0x8000) != 0;
    bool isLeftArrowPressed = (GetAsyncKeyState(VK_LEFT) & 0x8000) != 0;
    bool isRightArrowPressed = (GetAsyncKeyState(VK_RIGHT) & 0x8000) != 0;



    double moveSpeed = 0.2 * 20;
    double moveSpeedAngel = 0.003 * 10;

    //double turnSpeed = 0.003;
    double cos_A = Math.Cos(playerA);
    double sin_A = Math.Sin(playerA);

    if (isWPressed)
    {
        playerX += cos_A * moveSpeed;
        playerY += sin_A * moveSpeed;
    }
    if (isSPressed)
    {
        playerX += cos_A * -moveSpeed;
        playerY += sin_A * -moveSpeed;
    }
    if (isAPressed)
    {
        playerX += moveSpeed * sin_A;
        playerY += -moveSpeed * cos_A;
    }
    if (isDPressed)
    {
        playerX += -moveSpeed * sin_A;
        playerY += moveSpeed * cos_A;
    }

    if (isLeftArrowPressed)
        playerA -= moveSpeedAngel;
    if (isRightArrowPressed)
        playerA += moveSpeedAngel;

    window.Clear(Color.Black);




    Color BLUE = new Color(0, 0, 255);
    Color DARKGRAY = new Color(25, 25, 25);

    // Создание прямоугольника для верхней части экрана
    RectangleShape topRect = new RectangleShape(new Vector2f(ScreenWidth, HELF_HEIGHT));
    topRect.FillColor = Color.Blue;
    topRect.Position = new Vector2f(0, 0);

    // Создание прямоугольника для нижней части экрана
    RectangleShape bottomRect = new RectangleShape(new Vector2f(ScreenWidth, HELF_HEIGHT));
    bottomRect.FillColor = DARKGRAY;
    bottomRect.Position = new Vector2f(0, HELF_HEIGHT);

    // Отрисовка прямоугольников
    window.Draw(topRect);
    window.Draw(bottomRect);





    double car_angle = playerA - HULF_FOV;
    double x, y;
    double sin_a, cos_a;
    vertexArray.Clear();
    for (int i = 0; i < NUM_RAYS; i++)
    {
        sin_a = Math.Sin(car_angle);
        cos_a = Math.Cos(car_angle);

        for (double j = 0; j < MAX_DEPTH; j++)
        {
            x = playerX + j * cos_a;
            y = playerY + j * sin_a;

            if (values.Contains(((int)(x / TILE) * TILE, (int)(y / TILE) * TILE)) == true)
            {

                j *= Math.Cos(playerA - car_angle);
                double proj_height = PROJ_COEFF / j;

                byte c = (byte)Math.Min(255, 255 / (1 + j * j * 0.0001));
                Color color = new Color((byte)(c / 2), c, (byte)(c / 3));


                Vertex v1 = new Vertex(new Vector2f(i * SCALE, (float)(ScreenHeight / 2 - proj_height / 2)), color);
                Vertex v2 = new Vertex(new Vector2f(i * SCALE, (float)(ScreenHeight / 2 + proj_height / 2)), color);
                Vertex v3 = new Vertex(new Vector2f((i + 1) * SCALE, (float)(ScreenHeight / 2 + proj_height / 2)), color);
                Vertex v4 = new Vertex(new Vector2f((i + 1) * SCALE, (float)(ScreenHeight / 2 - proj_height / 2)), color);

                vertexArray.Append(v1);
                vertexArray.Append(v2);
                vertexArray.Append(v3);
                vertexArray.Append(v4);


                //window.Draw(vertexArray);
                break;
            }

        }

        car_angle += DELTA_ANGLE;

    }

    if (vertexArray.VertexCount > 0)
    {
        window.Draw(vertexArray);
    }
    window.Display();
}
