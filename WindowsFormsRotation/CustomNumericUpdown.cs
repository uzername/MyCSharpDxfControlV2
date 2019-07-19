using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsRotation
{
    public delegate void valueOfControlChanged(double in_value);
    public partial class CustomNumericUpdown : UserControl
    {
        // our control should cycle over the minimal and maximal values
        public UInt16 incrementDecrementCustom;
        private String prevText;
        
        public double numericValueStep = 1.0;
        public double numericValueMinimal = 0.0;
        public double numericValueMaximal = 359.0;
        private double numericValue ;
        private bool preventFireOfTextChangedEvent = false;
        /// <summary>
        /// event which happens when some value changes. receives a delta 
        /// </summary>
        public event valueOfControlChanged valueOfControlChangedEvt;
        private void setNumericValue(double in_numericValue)    {
            numericValue = in_numericValue;
            prevText = String.Format("{0}",numericValue);
            textBoxNumber.Text = prevText;
        }
        public CustomNumericUpdown()
        {
            InitializeComponent();
            preventFireOfTextChangedEvent = true;
            numericValue = numericValueMinimal;
            prevText = String.Format("{0}", numericValue);
            this.textBoxNumber.Text = prevText;
            preventFireOfTextChangedEvent = false;
        }

        private void buttonDecrease_Click(object sender, EventArgs e)
        {
            if (numericValue <= numericValueMinimal)  {
                numericValue = numericValueMaximal;
            }  else  {
                numericValue -= numericValueStep;
            }
            prevText = String.Format("{0}", numericValue);
            this.textBoxNumber.Text = prevText;
            valueOfControlChangedEvt?.Invoke(-numericValueStep);
        }

        private void buttonIncrease_Click(object sender, EventArgs e)
        {
            if (numericValue >= numericValueMaximal)    {
                numericValue = numericValueMinimal;
            }   else   {
                numericValue += numericValueStep;
            }
            prevText = String.Format("{0}", numericValue);
            this.textBoxNumber.Text = prevText;
            valueOfControlChangedEvt?.Invoke(numericValueStep);
        }

        private void textBoxNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            
        }

        private void textBoxNumber_KeyUp(object sender, KeyEventArgs e)
        {
            
        }

        private void textBoxNumber_TextChanged(object sender, EventArgs e)
        {
            if (preventFireOfTextChangedEvent) return;
            double v = 0;
            if (Double.TryParse(textBoxNumber.Text,out v) == false )  {
                // wrong input. do not change value
                int caretP= this.textBoxNumber.SelectionStart;
                preventFireOfTextChangedEvent = true;
                this.textBoxNumber.Text = prevText;
                preventFireOfTextChangedEvent = false;
                if (caretP>=1)
                this.textBoxNumber.SelectionStart = caretP-1;
            } else {
                if (v >= numericValueMaximal) { v = v % numericValueMaximal; }
                if (v<numericValueMinimal) { throw new NotImplementedException(); }                
                prevText = this.textBoxNumber.Text;                
                double deltaValue = v - numericValue;
                valueOfControlChangedEvt?.Invoke(deltaValue);
                numericValue = v;
            }

        }
    }
}
