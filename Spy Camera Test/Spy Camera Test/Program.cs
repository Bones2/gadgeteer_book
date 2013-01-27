using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using System.Threading;

namespace Spy_Camera_Test
{
    public partial class Program
    {
        GT.Timer timer = new GT.Timer(5000);
        GT.Timer hideMessageTimer;
        Window mainWindow;
        Border imageDisplay;
        Text label;

        void ProgramStarted()
        {
            SetupUI();
            camera.PictureCaptured += new Camera.PictureCapturedEventHandler(camera_PictureCaptured);
            timer.Tick +=new GT.Timer.TickEventHandler(timer_Tick);
            timer.Start();
        }

        void SetupUI()
        {
            mainWindow = display.WPFWindow;

            // setup the layout and background
            Canvas layout = new Canvas();
            Border background = new Border();
            background.Background = new SolidColorBrush(Colors.Black);
            background.Height = 240;
            background.Width = 320;
            layout.Children.Add(background);
            Canvas.SetLeft(background, 0);
            Canvas.SetTop(background, 0);

            // add the image display
            imageDisplay = new Border();
            imageDisplay.Height = 240;
            imageDisplay.Width = 320;
            layout.Children.Add(imageDisplay);
            Canvas.SetLeft(imageDisplay, 0);
            Canvas.SetTop(imageDisplay, 0);

            // add the text label
            label = new Text("");
            label.Height = 50;
            label.Width = 320;
            label.ForeColor = Colors.White;
            label.Font = Resources.GetFont(Resources.FontResources.NinaB);
            label.TextAlignment = TextAlignment.Center;
            label.Visibility = Visibility.Collapsed;
            layout.Children.Add(label);
            Canvas.SetLeft(label, 0);
            Canvas.SetTop(label, 0);

            mainWindow.Child = layout;
        }


        private void DisplayMessage(string message)
        {
            // hide image
            imageDisplay.Visibility = Visibility.Collapsed;

            // show label
            label.Visibility = Visibility.Visible;
            label.TextContent = message;
            label.UpdateLayout();

            if (hideMessageTimer == null)
            {
                hideMessageTimer = new GT.Timer(1000);
                hideMessageTimer.Tick += new GT.Timer.TickEventHandler(hideMessage_Tick);
                hideMessageTimer.Start();
            }
            else
            {
                hideMessageTimer.Restart();
            }
        }

        void hideMessage_Tick(GT.Timer timer)
        {
            timer.Stop();
            label.Visibility = Visibility.Collapsed;
            imageDisplay.Visibility = Visibility.Visible;
        }

        void camera_PictureCaptured(Camera sender, GT.Picture picture)
        {
            imageDisplay.Background = new ImageBrush(picture.MakeBitmap());
            DisplayMessage("Photo Taken");
        }

        void timer_Tick(GT.Timer timer)
        {
            if (camera.CameraReady)
            {
                camera.TakePicture();
            }
        }

    }
}
