namespace ScreenLib
{
    public class Screen
    {
        public int ScreenWidth {  get; init; }
        public int ScreenHeight { get; init; }

        public char[] ScreenChr { get; init; }

        public Screen(int width, int height)
        {
            Console.SetWindowSize(width, height);
            Console.SetBufferSize(width, height);
            Console.CursorVisible = false;

            ScreenChr = new char[width * height];
            ScreenWidth = width;
            ScreenHeight = height;
        }
    }
}
