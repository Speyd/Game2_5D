using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.InteropServices;
using MapLib;
using ScreenLib;
using ControlLib;









double playerFov = Math.PI / 3;
double playerY = 5;
double playerX = 5;
double playerA = 0;
const double Depth = 16;


Map map = new Map(32, 32);
map.addBlockToMap(2, 2);
map.addBlockToMap(2, 5);
map.addBlockToMap(7, 7);


Screen screen = new Screen(200, 50);
Control control = new Control();


char c = ' ';
while (true)
{
    control.makePressed(ref playerX, ref playerY, ref playerA);


    for (int x = 0; x < screen.ScreenWidth; x++)
    {
        double player = playerA + (playerFov / 2) - ((x * playerFov) / screen.ScreenWidth);

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

        int ceiling = (int)(screen.ScreenHeight / 2d - screen.ScreenHeight / distanceWall);
        int floor = screen.ScreenHeight - ceiling;
        for (int y = 0; y < screen.ScreenHeight; y++)
        {
            if (y <= ceiling)
            {
                screen.ScreenChr[y * screen.ScreenWidth + x] = ' ';
            }
            else if (y > ceiling && y <= floor)
            {
                screen.ScreenChr[y * screen.ScreenWidth + x] = '#';
            }
            else
            {
                screen.ScreenChr[y * screen.ScreenWidth + x] = '.';
            }
        }
    }

    Console.SetCursorPosition(0, 0);
    //Console.WriteLine($"X: {playerX}  | Y: {playerY}  |  playerA: {playerA}");
    Console.Write(screen.ScreenChr);
}
map.printMap();