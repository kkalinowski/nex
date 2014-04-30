using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using lib12.WPF.Core;
using WinFormsPoint = System.Drawing.Point;

namespace nex.Utilities
{
    public static class Utility
    {
        #region Fields
        private static int windowStackIndex = 0;
        #endregion

        #region Keyboard
        public static bool IsKeyLetterOrNumber(Key key)
        {
            return char.IsLetterOrDigit(ConvertKeyToChar(key));
        }

        public static char ConvertKeyToChar(Key key)
        {
            string keyID = key.ToString();
            if (keyID.Length == 1)//letter
                return char.ToLower(keyID[0]);
            else if (keyID.StartsWith("D") && keyID.Length == 2 && char.IsDigit(keyID[1]))
                return keyID[1];
            else if (keyID.StartsWith("NumPad"))
                return keyID[6];
            else
                return char.MinValue;
        } 
        #endregion

        #region Convert
        public static WinFormsPoint ConvertPoint(Point point)
        {
            return new WinFormsPoint((int)point.X, (int)point.Y);
        }

        public static string ConvertByteArrayToHex(byte[] array)
        {
            string hex = BitConverter.ToString(array);
            return hex.Replace("-", "").ToLower();
        } 
        #endregion

        #region Path
        public static string GetApplicationPath()
        {
            return Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
        } 
        #endregion

        #region Image
        public static Image LoadIcon(string iconName)
        {
            var image = new Image();
            image.Source = LoadImageSource(iconName);
            image.Width = 16;
            image.Height = 16;

            return image;
        }

        public static ImageSource LoadImageSource(string iconName)
        {
            return (ImageSource)Application.Current.FindResource(iconName);
        } 
        #endregion

        #region Fade
        public static void FadeIn()
        {
            if (windowStackIndex == 0)
                WpfUtilities.ThreadSafeInvoke(() => Application.Current.MainWindow.Opacity = 0.7);
            windowStackIndex++;
        }

        public static void FadeOut()
        {
            windowStackIndex--;
            if (windowStackIndex == 0)
                WpfUtilities.ThreadSafeInvoke(() => Application.Current.MainWindow.Opacity = 1);
        }
        #endregion
    }
}