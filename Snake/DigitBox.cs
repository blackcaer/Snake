using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Snake
{
    public class DigitBox : TextBox
    {
        public static readonly DependencyProperty AllowedSeparatorProperty =
            DependencyProperty.Register(
                "AllowedSeparator",
                typeof(SeparatorType),
                typeof(DigitBox),
                new PropertyMetadata(SeparatorType.None, new PropertyChangedCallback(AllowSeparator)));
        public SeparatorType AllowedSeparator
        {
            get => (SeparatorType)GetValue(AllowedSeparatorProperty);
            set => SetValue(AllowedSeparatorProperty, value);
        }

        private const string RegexAllowedNone = "^[0-9]*$";
        private const string RegexAllowedPeriod = "^[0-9.]*$";
        private string RegexAllowed = RegexAllowedNone;
        private Key OtherAcceptedKeys = Key.Return;
        public enum SeparatorType { None, Period, PeriodAndConvertToPeriod };
        public new string Text
        {
            get => base.Text;
            set => base.Text = HandleTextInput(value);
        }

        public DigitBox()
        {
            TextChanged += new TextChangedEventHandler(OnTextChanged);
            KeyDown += new KeyEventHandler(OnKeyDown);
            RegexAllowed = RegexAllowedNone;    // default
        }

        private static void AllowSeparator(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DigitBox currDigitBox = d as DigitBox;

            switch (e.NewValue)
            {
                case SeparatorType.None:
                    currDigitBox.RegexAllowed = RegexAllowedNone;
                    return;
                case SeparatorType.Period:
                    currDigitBox.RegexAllowed = RegexAllowedPeriod;
                    currDigitBox.OtherAcceptedKeys |= Key.OemPeriod;
                    break;
                case SeparatorType.PeriodAndConvertToPeriod:
                    currDigitBox.RegexAllowed = RegexAllowedPeriod;
                    currDigitBox.OtherAcceptedKeys |= Key.OemPeriod;
                    currDigitBox.OtherAcceptedKeys |= Key.OemComma;
                    currDigitBox.OtherAcceptedKeys |= Key.Decimal;
                    break;
                default:
                    goto case SeparatorType.None;
            }
        }
        protected void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Text = Text; // Invoking setter with HandleTextInput function to avoid redundancy;
        }
        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            Key key = e.Key;

            if (!CheckThisKeyFuther(key, e))
            {
                return;
            }

            if (!IsKeyNumber(key) || e.KeyboardDevice.Modifiers != ModifierKeys.None)
            {
                e.Handled = true;   // If not number and not OtherAcceptedKeys, ignore
                return;
            }
        }
        private string HandleTextInput(string text)
        {
            text = LeaveOnlyAllowedCharacters(text);

            if (NumOfPeriodsInText() > 1)
            {
                text = LeaveOnlyFirstPeriod(text);
            }

            if (text is "" or ".")
            {
                text = "0";
            }

            return text;
        }
        private static string LeaveOnlyFirstPeriod(string text)
        {
            return text.Replace(".", "").Insert(text.IndexOf("."), ".");
        }
        private string LeaveOnlyAllowedCharacters(string text)
        {
            // Replace comma with period
            text = text.Replace(',', '.');

            // Remove not allowed characters
            foreach (char c in text)
            {
                if (!Regex.IsMatch(c.ToString(), RegexAllowed))
                {
                    text = text.Replace(c.ToString(), "");
                }
            }
            return text;
        }
        /// <summary>
        /// Checks for special characters (OtherAcceptedKeys) and if they can be used (i.e. is period already in Text)
        /// </summary>
        /// <returns> False when all the job is done inside that method, 
        /// true if key needs futher checking </returns>
        private bool CheckThisKeyFuther(Key key, KeyEventArgs e)
        {
            if ((OtherAcceptedKeys & key) != key)
            {
                return true;    // numbers/letters etc
            }

            if (((Key.OemPeriod | Key.OemComma | Key.Decimal) & key) != key) // if not separator there's no need for futher checking
            {
                return false;
            }

            if (NumOfPeriodsInText() != 0) // Period is already in text so mark as handled
            {
                e.Handled = true;
                return false;
            }

            if (key is Key.OemComma or Key.Decimal)
            {
                // Handle comma event and replace with period:
                e.Handled = true;
                _ = TextCompositionManager.StartComposition(
                new TextComposition(InputManager.Current, this, "."));
                return false;
            }

            return false; // If just period
        }
        private static bool IsKeyNumber(Key key)
        {
            if (key is < Key.D0 or > Key.D9)
            {
                if (key is < Key.NumPad0 or > Key.NumPad9)
                {
                    return false;
                }
            }
            return true;
        }
        private int NumOfPeriodsInText()
        {
            int i = 0;
            foreach (char c in Text)
            {
                if (c == '.')
                {
                    i += 1;
                }
            }
            return i;
        }
    }
}

