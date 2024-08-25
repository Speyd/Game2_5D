using System.IO;
using System.Collections.Concurrent;
using System.Threading;
using System.Runtime.InteropServices;
using MapLib;
using ScreenLib;
using ControlLib;
using RenderLib;
using EntityLib;



Map map = new Map(32, 32);
map.addBlockToMap(2, 2);
map.addBlockToMap(2, 5);
map.addBlockToMap(7, 7);

Entity entity = new Entity(Math.PI / 3, 5, 5, 0, 16);

Screen screen = new Screen(350, 70);
//Screen screen = new Screen(200, 50);

Control control = new Control(screen.ScreenWidth, screen.ScreenHeight);
Render render = new Render(screen, map, entity, true, false);

DateTime dateTimeFrom = DateTime.Now;


while (true)
{
    DateTime dateTimeTo = DateTime.Now;
    double elapsedTime = (dateTimeTo - dateTimeFrom).TotalSeconds;
    dateTimeFrom = DateTime.Now;


    control.makePressed(entity);
    map.resetMap();

    render.render(elapsedTime);
}