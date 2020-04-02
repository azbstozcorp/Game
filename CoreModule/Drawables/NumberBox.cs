using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModule.Drawables {
    public class NumberBox : TextBox {
        public int Value { get; set; }
        public int Min { get; private set; }
        public int Max { get; private set; }

        int _origCenterX, _origCenterY;

        public NumberBox(int num, int centerX, int centerY, int min = 0, int max = int.MaxValue, bool startAtEnd = true) : base(num.ToString(), centerX, centerY, startAtEnd) {
            Value = num;
            Min = min;
            Max = max;

            _origCenterX = centerX;
            _origCenterY = centerY;

            Pressed += NumberBox_Pressed;
            TextChanged += NumberBox_TextChanged;
        }

        private void NumberBox_Pressed(Button sender) {
            location = Text.Length;
        }

        private void NumberBox_TextChanged(string newText) {
            if (newText.Length <= 0) {
                Value = Min;
                Text = Value.ToString();
                location = Text.Length;
            }
            else
            if (int.TryParse(newText, out int result)) {
                if (result <= Min) Value = Min;
                else if (result >= Max) Value = Max;
                else Value = result;
                Text = Value.ToString();
                location = Text.Length;
            }
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
            if (Value < Min) Value = Min;
            if (Value > Max) Value = Max;
            Text = Value.ToString();

            Bounds.Left = _origCenterX - Text.Length * 4;
            Bounds.Right = _origCenterX + Text.Length * 4;
        }
    }
}
