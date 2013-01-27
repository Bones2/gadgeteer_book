using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace SketchPad
{
    public partial class Program
    {
        Color drawingColor;

        Window mainWindow;
        Canvas layout;
        Image background;


        int sideBarWidth = 40;
        int buttonWidth = 35;
        int buttonHeight = 30;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            drawingColor = GT.Color.White;
            SetupUI();
            background.TouchMove += new Microsoft.SPOT.Input.TouchEventHandler(mainWindow_TouchMove);
        }

        void mainWindow_TouchMove(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            TouchInput[] touches = e.Touches;
            int oldX = -1;
            int oldY = -1;
            foreach (TouchInput touch in touches)
            {
                int x = touch.X - sideBarWidth;
                int y = touch.Y;
                if (oldX != -1)
                {
                    background.Bitmap.DrawLine(drawingColor, 3, oldX, oldY, x, y);
                }
                oldX = x;
                oldY = y;
            }
            background.Invalidate();
        }


        void SetupUI()
        {
            Border whiteButton;
            Border yellowButton;
            Border redButton;
            Border greenButton;
            Border blueButton;
            Border blackButton;
            Text clearButton;
            // initialize window
            mainWindow = display.WPFWindow;

            // setup the layout
            layout = new Canvas();
            background = new Image();
            background.Bitmap = new Bitmap(320 - sideBarWidth, 240);
            background.Height = 240;
            background.Width = 320 - sideBarWidth;

            layout.Children.Add(background);
            Canvas.SetLeft(background, sideBarWidth);
            Canvas.SetTop(background, 0);

            whiteButton = new Border();
            SetupButton(whiteButton, Colors.White, 0);
            whiteButton.TouchUp += new Microsoft.SPOT.Input.TouchEventHandler(whiteButton_TouchUp);

            yellowButton = new Border();
            SetupButton(yellowButton, Colors.Yellow, 1);
            yellowButton.TouchUp += new Microsoft.SPOT.Input.TouchEventHandler(yellowButton_TouchUp);

            redButton = new Border();
            SetupButton(redButton, Colors.Red, 2);
            redButton.TouchUp += new Microsoft.SPOT.Input.TouchEventHandler(redButton_TouchUp);

            greenButton = new Border();
            SetupButton(greenButton, Colors.Green, 3);
            greenButton.TouchUp += new Microsoft.SPOT.Input.TouchEventHandler(greenButton_TouchUp);

            blueButton = new Border();
            SetupButton(blueButton, Colors.Blue, 4);
            blueButton.TouchUp += new Microsoft.SPOT.Input.TouchEventHandler(blueButton_TouchUp);

            blackButton = new Border();
            SetupButton(blackButton, Colors.Black, 5);
            blackButton.TouchUp += new Microsoft.SPOT.Input.TouchEventHandler(blackButton_TouchUp);

            clearButton = SetupClearButton();

            mainWindow.Child = layout;
        }

        private Text SetupClearButton()
        {
            Text clearButton;
            clearButton = new Text("Clear");
            clearButton.Height = buttonHeight;
            clearButton.Width = buttonWidth;
            clearButton.ForeColor = Colors.Black;
            clearButton.Font = Resources.GetFont(Resources.FontResources.NinaB);
            clearButton.TextAlignment = TextAlignment.Center;
            layout.Children.Add(clearButton);
            Canvas.SetLeft(clearButton, 2);
            Canvas.SetTop(clearButton, 210);
            clearButton.TouchUp += new Microsoft.SPOT.Input.TouchEventHandler(clearButton_TouchUp);
            return clearButton;
        }

        private void SetupButton(Border button, Color color, int position)
        {
            button.Height = buttonHeight;
            button.Width = buttonWidth;
            button.Background = new SolidColorBrush(color);
            layout.Children.Add(button);
            Canvas.SetLeft(button, 2);
            Canvas.SetTop(button, 2 + position * (buttonHeight + 2));
        }

        void whiteButton_TouchUp(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            drawingColor = GT.Color.White;
        }
        void yellowButton_TouchUp(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            drawingColor = GT.Color.Yellow;
        }
        void redButton_TouchUp(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            drawingColor = GT.Color.Red;
        }
        void greenButton_TouchUp(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            drawingColor = GT.Color.Green;
        }
        void blueButton_TouchUp(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            drawingColor = GT.Color.Blue;
        }
        void blackButton_TouchUp(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            drawingColor = GT.Color.Black;
        }
        void clearButton_TouchUp(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            background.Bitmap = new Bitmap(320 - sideBarWidth, 240);
            background.Invalidate();
        }
    }
}
