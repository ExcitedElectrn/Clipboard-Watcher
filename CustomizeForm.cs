using System;
using System.Drawing;
using System.Windows.Forms;

namespace ToastMessageWhenCopied
{
    internal class CustomizeForm : Form
    {
        private readonly Button colorPreview;
        private readonly NumericUpDown fontSizeInput;
        private readonly NumericUpDown cornerRadiusInput;

        public PopupStyleSettings SelectedStyle { get; private set; }

        public CustomizeForm(PopupStyleSettings currentStyle)
        {
            SelectedStyle = new PopupStyleSettings
            {
                PopupColor = currentStyle.PopupColor,
                FontSize = currentStyle.FontSize,
                CornerRadius = currentStyle.CornerRadius
            };

            Text = "Customize Popup";
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(320, 210);

            Label colorLabel = new Label { Text = "Popup color", Left = 20, Top = 24, AutoSize = true };
            colorPreview = new Button { Left = 120, Top = 18, Width = 160, Height = 30, BackColor = SelectedStyle.PopupColor, Text = "Pick color" };
            colorPreview.Click += OnPickColor;

            Label fontLabel = new Label { Text = "Font size", Left = 20, Top = 76, AutoSize = true };
            fontSizeInput = new NumericUpDown
            {
                Left = 120,
                Top = 72,
                Width = 80,
                Minimum = 8,
                Maximum = 18,
                DecimalPlaces = 1,
                Increment = 0.1M,
                Value = (decimal)SelectedStyle.FontSize
            };

            Label cornerLabel = new Label { Text = "Corner radius", Left = 20, Top = 124, AutoSize = true };
            cornerRadiusInput = new NumericUpDown
            {
                Left = 120,
                Top = 120,
                Width = 80,
                Minimum = 10,
                Maximum = 28,
                Value = SelectedStyle.CornerRadius
            };

            Button saveButton = new Button { Text = "Save", Left = 120, Top = 165, Width = 75, DialogResult = DialogResult.OK };
            Button cancelButton = new Button { Text = "Cancel", Left = 205, Top = 165, Width = 75, DialogResult = DialogResult.Cancel };
            saveButton.Click += OnSaveClicked;

            Controls.Add(colorLabel);
            Controls.Add(colorPreview);
            Controls.Add(fontLabel);
            Controls.Add(fontSizeInput);
            Controls.Add(cornerLabel);
            Controls.Add(cornerRadiusInput);
            Controls.Add(saveButton);
            Controls.Add(cancelButton);

            AcceptButton = saveButton;
            CancelButton = cancelButton;
        }

        private void OnPickColor(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                colorDialog.Color = SelectedStyle.PopupColor;
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    SelectedStyle.PopupColor = colorDialog.Color;
                    colorPreview.BackColor = colorDialog.Color;
                }
            }
        }

        private void OnSaveClicked(object sender, EventArgs e)
        {
            SelectedStyle.FontSize = (float)fontSizeInput.Value;
            SelectedStyle.CornerRadius = (int)cornerRadiusInput.Value;
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
