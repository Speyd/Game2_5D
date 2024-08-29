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
map.addObjectToMap(2, 2, map.block);
map.addObjectToMap(2, 5, map.block);
map.addObjectToMap(10, 10, new MapLib.Object(5, 0, '$', false));
map.addObjectToMap(10, 10, new MapLib.Object(10, 0, '$', false));
map.addObjectToMap(12, 12, new MapLib.Object(0, 10, '?', false));

map.addObjectToMap(7, 13, map.block);
map.addObjectToMap(7, 11, new MapLib.Object(10, 0, '$', false));
map.addObjectToMap(7, 9, new MapLib.Object(10, 0, 'l', false));
map.addObjectToMap(7, 7, new MapLib.Object(15, 00, '!', false));


Entity entity = new Entity(Math.PI / 3, 4, 7, 0, 16);

Screen screen = new Screen(350, 70);
//Screen screen = new Screen(200, 50);

Control control = new Control(map, screen.ScreenWidth);
Render render = new Render(screen, map, entity, true, true);

DateTime dateTimeFrom = DateTime.Now;

try
{
    while (true)
    {
        DateTime dateTimeTo = DateTime.Now;
        double elapsedTime = (dateTimeTo - dateTimeFrom).TotalSeconds;
        dateTimeFrom = DateTime.Now;


        control.makePressed(entity);
        map.resetMap();

        render.render(elapsedTime);
    }
}
catch(Exception e)
{
    Console.WriteLine(e.Message);
    Console.WriteLine(e.StackTrace);

}