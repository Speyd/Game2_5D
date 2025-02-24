using EntityLib.Player;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using System;
using ScreenLib;


namespace TextField;
public class InputField
{
   //----------------Text--------------
    private Font font;
    private Text inputText;
    private RectangleShape inputBackground;
    private string userInput = "";

    //--------------Setting-------------
    public bool IsOpen { get; set; } = false;
    private List<string> dontShow = new List<string>() { "\r", "\x1b" };

    //----------------Render-------------
    private RenderTexture renderTexture;
    public Sprite RenderedSprite { get; private set; }
    



    public InputField(string pathFont,
        uint posWidth, uint posHeight, 
        uint widthField, uint heightField)
    {
        if (!System.IO.File.Exists(pathFont))
            throw new System.IO.FileNotFoundException($"Font file not found: {pathFont}");


        renderTexture = new RenderTexture(widthField, heightField);


        font = new Font(pathFont);
        inputText = new Text("", font, 20)
        {
            FillColor = Color.White,
            Position = new Vector2f(10, 10)
        };

        inputBackground = new RectangleShape(new Vector2f(widthField, heightField))
        {
            FillColor = new Color(0, 0, 0, 128) 
        };


        RenderedSprite = new Sprite(renderTexture.Texture)
        {
            Position = new Vector2f(posWidth, posHeight),
        };

        Screen.Window.TextEntered += HandleTextEntered;
        Screen.Window.KeyPressed += HandleKeyPressed;
    }

    private void HandleTextEntered(object sender, TextEventArgs e)
    {

        if (e.Unicode == "\b") // Backspace
        {
            IsOpen = true;
            if (userInput.Length > 0)
                userInput = userInput.Substring(0, userInput.Length - 1);
        }
        else if(!dontShow.Contains(e.Unicode) && IsOpen)
        {
            IsOpen = true;
            userInput += e.Unicode;
        }

        inputText.DisplayedString = userInput;
    }
    private void HandleKeyPressed(object sender, KeyEventArgs e)
    {
        if (e.Code == Keyboard.Key.Escape)
        {
            IsOpen = false;
            userInput = "";

            Screen.Window.TextEntered -= HandleTextEntered;
        }
        else if (e.Code == Keyboard.Key.Enter)
        {
            if (userInput == "" && IsOpen == false)
            {
                IsOpen = true;
                Screen.Window.TextEntered += HandleTextEntered;
                userInput = "";
            }
            else if(userInput != "" || userInput == "" && IsOpen == true)
            {
                IsOpen = false;
                Screen.Window.TextEntered -= HandleTextEntered;
                userInput = "";
            }
            inputText.DisplayedString = userInput;
        }
    }

    public void Draw()
    {         
        renderTexture.Clear(Color.Transparent);
        renderTexture.Draw(inputBackground);
        renderTexture.Draw(inputText);
        renderTexture.Display();

        Screen.OutputPriority.AddToPriority(RenderPriority.Enter, RenderedSprite);
    }
}
