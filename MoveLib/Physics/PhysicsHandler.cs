
using ProtoRender.Physics;
using System.Collections.Concurrent;

namespace MoveLib;
public static class PhysicsHandler
{
    private static readonly CancellationTokenSource _cts = new();
    private static readonly Thread PhysicsThread;

    private static readonly ConcurrentDictionary<IPhysicsObject, byte> PhysicsObjects = new();

    public static int delayMs = 10;
    static PhysicsHandler()
    {
        PhysicsThread = new Thread(PhysicsLoop)
        {
            IsBackground = true,
            Name = "Physics Processing Thread"
        };
        PhysicsThread.Start();
    }


    public static void Register(IPhysicsObject physicsObject)
    {
        PhysicsObjects.TryAdd(physicsObject, 0);
    }
    public static void Unregister(IPhysicsObject physicsObject)
    {
        PhysicsObjects.TryRemove(physicsObject, out _);
    }

    private static void PhysicsLoop()
    {
        var stopwatch = new System.Diagnostics.Stopwatch();
        stopwatch.Start();

        while (!_cts.Token.IsCancellationRequested)
        {
            foreach (var physicsObject in PhysicsObjects)
            {
                physicsObject.Key.UpdatePhysics();
            }

            Thread.Sleep(delayMs);
        }
    }

    public static void Stop()
    {
        _cts.Cancel();
        PhysicsThread.Join();
    }
}
