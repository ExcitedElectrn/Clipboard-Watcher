using System;
using System.Drawing;
using ToastMessageWhenCopied.Properties;

namespace ToastMessageWhenCopied
{
    public class PopupStyleSettings
    {
        public Color PopupColor { get; set; }
        public float FontSize { get; set; }
        public int CornerRadius { get; set; }

        public static PopupStyleSettings Load()
        {
            return new PopupStyleSettings
            {
                PopupColor = Color.FromArgb(Settings.Default.PopupColorArgb),
                FontSize = Clamp(Settings.Default.PopupFontSize, 8f, 18f),
                CornerRadius = Clamp(Settings.Default.PopupCornerRadius, 10, 28)
            };
        }

        public void Save()
        {
            Settings.Default.PopupColorArgb = PopupColor.ToArgb();
            Settings.Default.PopupFontSize = Clamp(FontSize, 8f, 18f);
            Settings.Default.PopupCornerRadius = Clamp(CornerRadius, 10, 28);
            Settings.Default.Save();
        }

        private static int Clamp(int value, int min, int max)
        {
            return Math.Max(min, Math.Min(max, value));
        }

        private static float Clamp(float value, float min, float max)
        {
            return Math.Max(min, Math.Min(max, value));
        }
    }
}
