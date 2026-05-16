using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace WPFTowerDefense
{
    public static class UIElements
    {
        public static Button CreateStartButton(Action onClick)
        {
            Button startButton = new Button
            {
                Content = "Start Game",
                Width = 120,
                Height = 40,
                Margin = new Thickness(10),
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom
            };

            startButton.Click += (s, e) => onClick();
            return startButton;
        }

        //public static TextBlock CreateWaveText()
        //{
        //    return new TextBlock
        //    {
        //        Text = "Wave: 0",
        //        FontSize = 20,
        //        Foreground = Brushes.White,
        //        VerticalAlignment = VerticalAlignment.Bottom,
        //        HorizontalAlignment = HorizontalAlignment.Right,
        //        Margin = new Thickness(150, 0, 10, 10)
        //    };
        //}
    }
}
