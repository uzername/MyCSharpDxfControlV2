using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsDXF;

namespace WindowsFormsRotation
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            
            InitializeComponent();
            this.customNumericUpdown1.valueOfControlChangedEvt += CustomNumericUpdown1_valueOfControlChangedEvt;
        }

        private void CustomNumericUpdown1_valueOfControlChangedEvt(double in_value)
        {
            this.userControlForPaint1.realTimeRotation(-in_value);
        }

        private void Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {
            this.userControlForPaint1.realTimeRotation(15.0);

            
        }

        private void buttonRight_Click(object sender, EventArgs e)
        {
            this.userControlForPaint1.realTimeRotation(-15.0);
        }

        private void button1_Click(object sender, EventArgs e)  {
            // Displays an OpenFileDialog so the user can select a Cursor.  
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Dxf Files|*.dxf";
            openFileDialog1.Title = "Select Dxf File";
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // get here a path to dxf file
                string obtainedFileName = openFileDialog1.FileName;
                textBox1.Text = obtainedFileName;
                //unroll wrapper for reading
                DxfReadWrapper DxfReadWrapperInstance = new DxfReadWrapper();
                //prepare struct for obtaining data
                completeDxfStruct structToObtain = DxfReadWrapperInstance.processDxfFile(obtainedFileName);
                userControlForPaint1.structToRender = structToObtain;
                /*
                toolStripStatusLabel2.Text = myDxfControl1.horizontalDimensionDxf.ToString();
                toolStripStatusLabel4.Text = myDxfControl1.verticalDimensionDxf.ToString();
                toolStripStatusLabel6.Text = myDxfControl1.getCurrentTransformationAngle().ToString();
                */
                userControlForPaint1.recalculateScaleToFitAll();
                userControlForPaint1.Refresh();
            }
        }
    }
}
