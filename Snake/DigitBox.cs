using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


namespace Snake
{
    public class DigitBox : TextBox
    {
        private Key keysToBePassedFuther = Key.Return;
        public enum allowedSeparator { None, Dot, PeriodAndCommaToPeriod};
        public DigitBox()
        {
            TextChanged += new TextChangedEventHandler(OnTextChanged);
            KeyDown += new KeyEventHandler(OnKeyDown);
            AllowSeparator(allowedSeparator.PeriodAndCommaToPeriod);
        }

        public void AllowSeparator(allowedSeparator alsep)
        {
            switch (alsep)
            {
                case allowedSeparator.None:
                    return; 
                case allowedSeparator.Dot:
                    keysToBePassedFuther |= Key.OemPeriod;
                    break;
                case allowedSeparator.PeriodAndCommaToPeriod:
                    keysToBePassedFuther |= Key.OemPeriod;
                    keysToBePassedFuther |= Key.OemComma;
                    break;
            }
        }

        protected void OnTextChanged(object sender, TextChangedEventArgs e)
        {

        }
        protected void OnKeyDown(object sender, KeyEventArgs e)
        {
            Key key = e.Key;
            if ((keysToBePassedFuther & key) == key)
            {
                if (key == Key.OemComma)    // replace comma
                {
                    e.Handled = true;
                    TextCompositionManager.StartComposition(
                    new TextComposition(InputManager.Current, this, "."));
                }
                return; 
            }

            if (!isKeyNumber(key))
            {
                //Trace.WriteLine(key + " failed");
                e.Handled = true;   // If not number and not keysToBePassedFuther, ignore
            }
        }

        private bool isKeyNumber(Key key)
        {
            if (key < Key.D0 || key>Key.D9)
            {
                if (key < Key.NumPad0 || key > Key.NumPad9)
                {
                    return false;
                }
            }
            return true;
        }

    }
}

