using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace User.Windows.Forms
{
    public enum ProgressBarDisplayMode
    {
        NoText,
        Percentage,
        CurrProgress,
        CustomText,
        TextAndPercentage,
        TextAndCurrProgress,
        TextAndPercentageIsolation,
        TextAndCurrProgressIsolation,
    }

    public class TextProgressBar : ProgressBar
    {
        private ContentAlignment _textAlignValue = ContentAlignment.MiddleCenter;
        [DefaultValue(ContentAlignment.MiddleCenter), Category("Additional Options"), Description("Align value of text on ProgressBar")]
        public ContentAlignment TextAlignValue
        {
            get => _textAlignValue;
            set { 
                _textAlignValue = value;
                Invalidate();
            }
        }

        private ContentAlignment _customTextAlignValue = ContentAlignment.MiddleLeft;
        [DefaultValue(ContentAlignment.MiddleLeft), Category("Additional Options"), Description("Align value of custom text on ProgressBar when the VisualMode is isolation")]
        public ContentAlignment CustomTextAlignValue
        {
            get => _customTextAlignValue;
            set
            {
                _customTextAlignValue = value;
                Invalidate();
            }
        }

        [Description("Font of the text on ProgressBar"), Category("Additional Options")]
        public Font TextFont { get; set; } = new Font(FontFamily.GenericSerif, 11, FontStyle.Bold | FontStyle.Italic);

        private SolidBrush _textColourBrush = (SolidBrush)Brushes.Black;
        [Category("Additional Options")]
        public Color TextColor
        {
            get => _textColourBrush.Color;
            set
            {
                _textColourBrush.Dispose();
                _textColourBrush = new SolidBrush(value);
            }
        }

        private SolidBrush _progressColourBrush = (SolidBrush)Brushes.LightGreen;
        [Category("Additional Options"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public Color ProgressColor
        {
            get => _progressColourBrush.Color;
            set
            {
                _progressColourBrush.Dispose();
                _progressColourBrush = new SolidBrush(value);
            }
        }

        private ProgressBarDisplayMode _visualMode = ProgressBarDisplayMode.CurrProgress;
        [Category("Additional Options"), Browsable(true)]
        public ProgressBarDisplayMode VisualMode
        {
            get => _visualMode;
            set
            {
                _visualMode = value;
                Invalidate();//redraw component after change value from VS Properties section
            }
        }

        private string _text = string.Empty;

        [Description("If it's empty, % will be shown"), Category("Additional Options"), Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public string CustomText
        {
            get => _text;
            set
            {
                _text = value;
                Invalidate();//redraw component after change value from VS Properties section
            }
        }

        private string _textToDraw
        {
            get
            {
                string text = CustomText;

                switch (VisualMode)
                {
                    case (ProgressBarDisplayMode.Percentage):
                    case (ProgressBarDisplayMode.TextAndPercentageIsolation):
                        text = _percentageStr;
                        break;
                    case (ProgressBarDisplayMode.CurrProgress):
                    case (ProgressBarDisplayMode.TextAndCurrProgressIsolation):
                        text = _currProgressStr;
                        break;
                    case (ProgressBarDisplayMode.TextAndCurrProgress):
                        text = $"{CustomText}: {_currProgressStr}";
                        break;
                    case (ProgressBarDisplayMode.TextAndPercentage):
                        text = $"{CustomText}: {_percentageStr}";
                        break;
                }

                return text;
            }
            set { }
        }

        private string _percentageStr { get => $"{(int)((float)Value - Minimum) / ((float)Maximum - Minimum) * 100 } %"; }

        private string _currProgressStr { get => $"{Value}/{Maximum}"; }

        public TextProgressBar()
        {
            Value = Minimum;
            FixComponentBlinking();
        }

        private void FixComponentBlinking()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            DrawProgressBar(g);

            DrawStringIfNeeded(g);
        }

        private void DrawProgressBar(Graphics g)
        {
            Rectangle rect = ClientRectangle;

            ProgressBarRenderer.DrawHorizontalBar(g, rect);

            rect.Inflate(-3, -3);

            if (Value > 0)
            {
                Rectangle clip = new Rectangle(rect.X, rect.Y, (int)Math.Round(((float)Value / Maximum) * rect.Width), rect.Height);

                g.FillRectangle(_progressColourBrush, clip);
            }
        }

        private void DrawStringIfNeeded(Graphics g)
        {
            if (VisualMode != ProgressBarDisplayMode.NoText)
            {
                string text = _textToDraw;
                SizeF len = g.MeasureString(text, TextFont);
                int x, y;
                if (_textAlignValue.ToString().Contains("Left")) x = 0;
                else if (_textAlignValue.ToString().Contains("Right")) x = Width - (int)len.Width;
                else x = Width / 2 - (int)len.Width / 2;

                if (_textAlignValue.ToString().Contains("Top")) y = 0;
                else if (_textAlignValue.ToString().Contains("Bottom")) y = Height - (int)len.Height;
                else y = Height / 2 - (int)len.Height / 2;

                Point location = new Point(x, y);

                g.DrawString(text, TextFont, (Brush)_textColourBrush, location);

                if (VisualMode.ToString().Contains("Isolation"))
                {
                    len = g.MeasureString(CustomText, TextFont);
                    if (_customTextAlignValue.ToString().Contains("Left")) x = 0;
                    else if (_customTextAlignValue.ToString().Contains("Right")) x = Width - (int)len.Width;
                    else x = Width / 2 - (int)len.Width / 2;

                    if (_customTextAlignValue.ToString().Contains("Top")) y = 0;
                    else if (_customTextAlignValue.ToString().Contains("Bottom")) y = Height - (int)len.Height;
                    else y = Height / 2 - (int)len.Height / 2;

                    location = new Point(x, y);

                    g.DrawString(CustomText, TextFont, (Brush)_textColourBrush, location);
                }
            }
        }

        public new void Dispose()
        {
            _textColourBrush.Dispose();
            _progressColourBrush.Dispose();
            base.Dispose();
        }
    }
}
