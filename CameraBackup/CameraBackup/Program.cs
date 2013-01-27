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
using System.IO;

namespace CameraBackup
{
    public partial class Program
    {

        GT.StorageDevice sdStorageDevice = null;
        GT.StorageDevice usbStorageDevice = null;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            usbHost.USBDriveConnected += new UsbHost.USBDriveConnectedEventHandler(usbHost_USBDriveConnected);
            usbHost.USBDriveDisconnected += new UsbHost.USBDriveDisconnectedEventHandler(usbHost_USBDriveDisconnected);
            sdCard.SDCardMounted += new SDCard.SDCardMountedEventHandler(sdCard_SDCardMounted);
            sdCard.SDCardUnmounted += new SDCard.SDCardUnmountedEventHandler(sdCard_SDCardUnmounted);
            ledSD.TurnRed();
            ledUSB.TurnRed();
        }

        void sdCard_SDCardMounted(SDCard sender, GT.StorageDevice storageDevice)
        {
            ledSD.TurnGreen();
            sdStorageDevice = storageDevice;
            CheckAndTransfer();
        }


        void sdCard_SDCardUnmounted(SDCard sender)
        {
            ledSD.TurnRed();
            sdStorageDevice = null;
        }

        void usbHost_USBDriveConnected(UsbHost sender, GT.StorageDevice storageDevice)
        {
            ledUSB.TurnGreen();
            usbStorageDevice = storageDevice;
            CheckAndTransfer();
        }

        void usbHost_USBDriveDisconnected(UsbHost sender)
        {
            ledUSB.TurnRed();
            usbStorageDevice = null;
        }

        private void CheckAndTransfer()
        {
            if (sdStorageDevice != null && usbStorageDevice != null)
            {
                TransferFiles();
            }
        }

        private void TransferFiles()
        {
            ledSD.BlinkRepeatedly(Colors.Green);
            ledUSB.BlinkRepeatedly(Colors.Green);
            try
            {
                CopyFiles("\\");
                DeepCopy(sdStorageDevice.ListRootDirectorySubdirectories());
                ledSD.TurnOff();
                ledSD.TurnGreen();
                ledUSB.TurnOff();
                ledUSB.TurnGreen();
            }
            catch (Exception)
            {
                ledSD.BlinkRepeatedly(Colors.Red);
                ledUSB.BlinkRepeatedly(Colors.Red);
            }
        }

        private void DeepCopy(string[] sourceDirs)
        {
            foreach (string sourceDir in sourceDirs)
            {
                // ignore folders starting with .
                if (!sourceDir.Substring(0, 1).Equals("."))
                {
                    CopyFiles(sourceDir);
                    DeepCopy(sdStorageDevice.ListDirectories(sourceDir));
                }
            }
        }

        private void CopyFiles(string sourceDir)
        {
            string[] files = sdStorageDevice.ListFiles(sourceDir);
            foreach (string filepath in files)
            {
                if (!filepath.Substring(0, 1).Equals("."))
                {
                    BufferedFileCopy(filepath);
                }
            }
        }

        private void BufferedFileCopy(string filepath)
        {
            int bufferSize = 4096;
            Debug.Print("copying: " + filepath);
            string[] parts = filepath.Split('\\');
            string filename = parts[parts.Length - 1];
            FileStream outStream = usbStorageDevice.OpenWrite(filename);
            FileStream inStream = sdStorageDevice.OpenRead(filepath);
            byte[] buffer = new byte[bufferSize];
            int bytesRead = inStream.Read(buffer, 0, bufferSize);
            while (bytesRead > 0)
            {
                outStream.Write(buffer, 0, bytesRead);
                bytesRead = inStream.Read(buffer, 0, bufferSize);
            }
            outStream.Close();
            inStream.Close();
        }
    }
}
