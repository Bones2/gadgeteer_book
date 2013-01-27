using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;

using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using System.Threading;

namespace SpyCamera
{
    public partial class Program
    {
        int pictureIndex = 0;
        int intervalPosition = 1;
        int[] intervals = {0, 5, 10, 20, 30, 60};
        GT.Timer timer;
        GT.Timer hideMessageTimer;
        Window mainWindow;
        GT.Picture lastPicture = null;
        
        Border imageDisplay;
        Text label;

        void ProgramStarted()
        {
            SetupUI();

            camera.PictureCaptured += new Camera.PictureCapturedEventHandler(camera_PictureCaptured);
            setTimerInterval(intervalPosition);
        }

        void SetupUI()
        {
            // 1) init window
            mainWindow = display.WPFWindow;
            mainWindow.TouchDown += new Microsoft.SPOT.Input.TouchEventHandler(mainWindow_TouchDown);

            // 2) setup the layout
            Canvas layout = new Canvas();
            Border background = new Border();
            background.Background = new SolidColorBrush(Colors.Black);
            background.Height = 240;
            background.Width = 320;

            layout.Children.Add(background);
            Canvas.SetLeft(background, 0);
            Canvas.SetTop(background, 0);

            // 3) add the image display
            imageDisplay = new Border();
            imageDisplay.Height = 240;
            imageDisplay.Width = 320;
            
            layout.Children.Add(imageDisplay);
            Canvas.SetLeft(imageDisplay, 0);
            Canvas.SetTop(imageDisplay, 0);

            // 4) add the text label
            label = new Text("testing");
            label.Height = 240;
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

        void setTimerInterval(int newIntervalPosition)
        {
            intervalPosition = newIntervalPosition;
            TimeSpan interval = new TimeSpan(0, 0, 0, 0, intervals[intervalPosition] * 1000);
            if (timer != null)
            {
                timer.Stop();
            }
            else
            {
                timer = new GT.Timer(interval);
                timer.Tick += new GT.Timer.TickEventHandler(timer_Tick);
            }

            if (intervalPosition == 0)
            {
                DisplayMessage("Camera Off");
            }
            else
            {
                timer.Interval = interval;
                timer.Start();

                DisplayMessage("Interval Set to: " + intervals[intervalPosition] + " seconds");
            }
        }

        void mainWindow_TouchDown(object sender, Microsoft.SPOT.Input.TouchEventArgs e)
        {
            intervalPosition++;
            if (intervalPosition == intervals.Length)
            {
                intervalPosition = 0;
            }
            setTimerInterval(intervalPosition);
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
            lastPicture = picture;
        }

        void timer_Tick(GT.Timer timer)
        {
            // save the last image captured.
            // The new one probably isn't ready yet
            SaveLastImage();
            if (camera.CameraReady)
            {
                camera.TakePicture();
            }
        }

        void SaveLastImage()
        {
            if (lastPicture == null)
            {
                return;
            }
            try
            {
                String filename = "picture_" + pictureIndex + ".bmp";
                DisplayMessage("Saving .....");
                sdCard.GetStorageDevice().WriteFile(filename, lastPicture.PictureData);
                DisplayMessage("Photo Saved to: " + filename);
                pictureIndex++;
            }
            catch (Exception ex)
            {
                DisplayMessage("SD Card Error");
            }
        }

    }
}
