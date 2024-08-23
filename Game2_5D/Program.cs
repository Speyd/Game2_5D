using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.InteropServices;
using MapLib;

[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int key);

// Коды клавиш
const int VK_W = 0x57;
const int VK_S = 0x53;
const int VK_A = 0x41;
const int VK_D = 0x44;
const int VK_LEFT = 0x25;
const int VK_RIGHT = 0x27;






const int ScreenWidth = 200;
const int ScreenHeight = 50;
double playerFov = Math.PI / 3;
double playerY = 5;
double playerX = 5;
double playerA = 0;
const double Depth = 16;


Map map = new Map(32, 32);
map.addBlockToMap(2, 2);
map.addBlockToMap(2, 5);
map.addBlockToMap(7, 7);


char[]  Screen = new char[ScreenWidth * ScreenHeight];

Console.SetWindowSize(ScreenWidth, ScreenHeight);
Console.SetBufferSize(ScreenWidth, ScreenHeight);
Console.CursorVisible =  false;

char c = ' ';
DateTime from = DateTime.Now;
while (true)
{

    DateTime dateTime = DateTime.Now;
    double elapsed = (dateTime - from).TotalSeconds;
    from = DateTime.Now;

    bool isWPressed = (GetAsyncKeyState(VK_W) & 0x8000) != 0;
    bool isSPressed = (GetAsyncKeyState(VK_S) & 0x8000) != 0;
    bool isAPressed = (GetAsyncKeyState(VK_A) & 0x8000) != 0;
    bool isDPressed = (GetAsyncKeyState(VK_D) & 0x8000) != 0;
    bool isLeftArrowPressed = (GetAsyncKeyState(VK_LEFT) & 0x8000) != 0;
    bool isRightArrowPressed = (GetAsyncKeyState(VK_RIGHT) & 0x8000) != 0;



    double moveSpeed = 0.003;
    //double turnSpeed = 0.003;
    if (isWPressed)
    {
        playerX += Math.Sin(playerA) * 3 * moveSpeed;
        playerY += Math.Cos(playerA) * 3 * moveSpeed;
    }
    if (isSPressed)
    {
        playerX -= Math.Sin(playerA) * 3 * moveSpeed;
        playerY -= Math.Cos(playerA) * 3 * moveSpeed;
    }
    if (isAPressed)
    {
        playerX += 2 * moveSpeed * Math.Sin(playerA + Math.PI / 2);
        playerY += 2 * moveSpeed * Math.Cos(playerA + Math.PI / 2);
    }
    if (isDPressed)
    {
        playerX -= 2 * moveSpeed * Math.Sin(playerA + Math.PI / 2);
        playerY -= 2 * moveSpeed * Math.Cos(playerA + Math.PI / 2);
    }


    if (isLeftArrowPressed)
        playerA += elapsed;
    if (isRightArrowPressed)
        playerA -= elapsed;


    for (int x = 0; x < ScreenWidth; x++)
    {
        double player = playerA + (playerFov / 2) - ((x * playerFov) / ScreenWidth);

        double rayX = Math.Sin(player);
        double rayY = Math.Cos(player);

        double distanceWall = 0;
        bool hitWall = false;

        while (!hitWall && distanceWall < Depth)
        {
            distanceWall += 0.1;
            int testX = (int)(playerX + rayX * distanceWall);
            int testY = (int)(playerY + rayY * distanceWall);

            if (testX < 0 || testX >= Depth + playerX || testY < 0 || testY >= Depth + playerY)
            {
                hitWall = true;
                distanceWall = Depth;
            }
            else
            {
                char testSell = map.MapStr[testY * map.MapWidth + testX];
                if (testSell == '#')
                    hitWall = true;
            }
        }

        int ceiling = (int)(ScreenHeight / 2d - ScreenHeight / distanceWall);
        int floor = ScreenHeight - ceiling;
        for (int y = 0; y < ScreenHeight; y++)
        {
            if (y <= ceiling)
            {
                Screen[y * ScreenWidth + x] = ' ';
            }
            else if (y > ceiling && y <= floor)
            {
                Screen[y * ScreenWidth + x] = '#';
            }
            else
            {
                Screen[y * ScreenWidth + x] = '.';
            }
        }
    }

    Console.SetCursorPosition(0, 0);
    // Console.WriteLine($"X: {playerX}  | Y: {playerY}  |  playerA: {playerA}");
    Console.Write(Screen);
}
map.printMap();