using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsDXF;

namespace WindowsFormsRotation
{
    public partial class UserControlForPaint : UserControl
    {
        private Pen solidBlack = new Pen(Color.Black);
        // canvas dimensions required to fit all the geometrical figures
        private double requiredCanvasHeight;
        private double requiredCanvasWidth;
        public completeDxfStruct structToRender = new completeDxfStruct();

        internal void realTimeRotation(double v)
        {
            this.structToRender.performRotationOfDxfStruct(v);
            this.recalculateRequiredDrawingWidth();
            this.Refresh();
        }

        public double internalScaleFactor = 1.0;
        public double internalScaleFactorDelta = 0.1;
        private bool isCtrlButtonHeld = false;
        // not sure about these numbers. Looks prettier with them 
        private double prescaleKoefficient = 1.10; // we don't want the figure to keep the whole space. The higher is the value the bigger are margins
        
        /// <summary>
        /// we calculate here the dimensions which should be set to contain all the geometric entities in drawing panel.
        /// By the way, drawing panel can be larger than control, then the scrollbars appear
        /// </summary>
        private void recalculateRequiredDrawingWidth() {
            double xll = structToRender.GetBoundingBox().XLowerLeft;
            double yll = structToRender.GetBoundingBox().YLowerLeft;
            double xur = structToRender.GetBoundingBox().XUpperRight;
            double yur = structToRender.GetBoundingBox().YUpperRight;
            double domainHeight = prescaleKoefficient * Math.Abs(yur - yll);
            double domainWidth = prescaleKoefficient * Math.Abs(xll - xur);
            
            requiredCanvasHeight = internalScaleFactor * domainHeight;
            requiredCanvasWidth = internalScaleFactor * domainWidth;
            
        }
        /// <summary>
        /// A FitAll command - calculates the scale required to fit all geometry inside the control, without scrollbars
        /// Also recalculates the required bounding box
        /// </summary>
        public void recalculateScaleToFitAll()
        {
            double xll = structToRender.GetBoundingBox().XLowerLeft;
            double yll = structToRender.GetBoundingBox().YLowerLeft;
            double xur = structToRender.GetBoundingBox().XUpperRight;
            double yur = structToRender.GetBoundingBox().YUpperRight;
            
            double domainHeight = prescaleKoefficient * Math.Abs(yur - yll) ;
            double domainWidth = prescaleKoefficient * Math.Abs(xll - xur) ;
            double wscale = this.Width / domainWidth;
            double hscale = this.Height / domainHeight;
            internalScaleFactor = hscale < wscale ? hscale : wscale;
            requiredCanvasHeight = internalScaleFactor * domainHeight;
            requiredCanvasWidth = internalScaleFactor * domainWidth;

        }
        public UserControlForPaint()
        {
            InitializeComponent();
            
            //structToRender.addDxfDrawingEntry(new MyDxfLine(20, 10, 30, 100));
            //structToRender.addDxfDrawingEntry(new MyDxfLine(20, 110, 30, 10));
            //structToRender.addDxfDrawingEntry(new MyDxfArc(50.0, 50.0, 0.0, 45.0, 10.0));
            
            recalculateRequiredDrawingWidth();
            
            this.realPaintingPanel.MouseWheel += RealPaintingPanel_MouseWheel;
            this.KeyDown += MainControl_KeyDown;
            this.KeyUp += MainControl_KeyUp;
            
        }


        private void MainControl_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.ControlKey) && (isCtrlButtonHeld))
            {
                isCtrlButtonHeld = false;
            }
        }

        private void MainControl_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Add) && (e.Control)) {
                performRealTimeImageScaling(1.0);
                return;
            }
            if ((e.KeyCode == Keys.Subtract) && (e.Control))
            {
                performRealTimeImageScaling(-1.0);
                return;
            }
            if (e.KeyCode == Keys.ControlKey) { isCtrlButtonHeld = true; }
            
        }
        private void performRealTimeImageScaling(double direction)
        {
            internalScaleFactor += direction * internalScaleFactorDelta;
            recalculateRequiredDrawingWidth();
            UserControlForPaint_Resize(null, null);
            this.Refresh();
        }
        private void RealPaintingPanel_MouseWheel(object sender, MouseEventArgs e)
        {
            if (isCtrlButtonHeld )  {
                double direction = e.Delta < 0 ? -1.0 : 1.0;
                performRealTimeImageScaling(direction);
            }
        }

        private void RealPaintingPanel_Paint(object sender, PaintEventArgs e)
        {
            //https://stackoverflow.com/questions/1485745/flip-coordinates-when-drawing-to-control
            // Call the OnPaint method of the base class.  
            base.OnPaint(e);
            // Begin graphics container
            System.Drawing.Drawing2D.GraphicsContainer containerState = e.Graphics.BeginContainer();
            double offset2X = 0.0; double offset2Y = 0.0;
            // Flip the Y-Axis
            e.Graphics.ScaleTransform(1.0F, -1.0F);
            e.Graphics.TranslateTransform(0.0F, -(float)Height + 1.0f);

            e.Graphics.Clear(Color.White);
            //let's have all entries shown on field, no matter how far or close are they located
            double deltaDrawingOffsetX = (0 - structToRender.GetBoundingBox().XLowerLeft)*internalScaleFactor;
            double deltaDrawingOffsetY = (0 - structToRender.GetBoundingBox().YLowerLeft)*internalScaleFactor;
            // do not cling pixel-to-pixel to lower bottom
            double deltaTinyOffsetW = Math.Abs(1 - prescaleKoefficient) / 2.0 * this.Width;
            double deltaTinyOffsetH = Math.Abs(1 - prescaleKoefficient) / 2.0 * this.Height;
            //render all
            using (System.Drawing.Pen myPen = new System.Drawing.Pen(Color.Black))
            {
                int totalNumber = structToRender.getSize();
                for (int i = 0; i < totalNumber; i++)
                {
                    DXFdrawingEntry tmpEntryFromList = structToRender.getItemByIndex(i);
                    if (tmpEntryFromList is MyDxfLine)
                    {
                        e.Graphics.DrawLine(myPen, 
                            (float)((tmpEntryFromList as MyDxfLine).XStart * internalScaleFactor + deltaDrawingOffsetX + deltaTinyOffsetW),
                            (float)((tmpEntryFromList as MyDxfLine).YStart * internalScaleFactor + deltaDrawingOffsetY + deltaTinyOffsetH),
                            (float)((tmpEntryFromList as MyDxfLine).XEnd * internalScaleFactor + deltaDrawingOffsetX + deltaTinyOffsetW), 
                            (float)((tmpEntryFromList as MyDxfLine).YEnd * internalScaleFactor + deltaDrawingOffsetY + deltaTinyOffsetH));
                    }
                    else if (tmpEntryFromList is MyDxfArc)
                    {
                        //width and height (3 and 4 parametrs) should be scaled and not translated
                        e.Graphics.DrawArc(myPen,
                            (float)((tmpEntryFromList as MyDxfArc).drawUpperLeftX * internalScaleFactor + deltaDrawingOffsetX + deltaTinyOffsetW),
                            (float)((tmpEntryFromList as MyDxfArc).drawUpperLeftY * internalScaleFactor + deltaDrawingOffsetY + deltaTinyOffsetH),
                            (float)((tmpEntryFromList as MyDxfArc).drawDimensionHorizontal * internalScaleFactor ),
                            (float)((tmpEntryFromList as MyDxfArc).drawDimensionVertical * internalScaleFactor ),
                            (float)((tmpEntryFromList as MyDxfArc).drawStartAngleDegreeCW),
                            (float)((tmpEntryFromList as MyDxfArc).drawSweepAngleDegree));
                    }

                }
            }
            e.Graphics.EndContainer(containerState);
        }

        private void UserControlForPaint_Load(object sender, EventArgs e)
        {
            //called after constructor works
            realPaintingPanel.Width = this.Width-realPaintingPanel.Left;
            realPaintingPanel.Height = this.Height-realPaintingPanel.Top;
            recalculateScaleToFitAll();
        }

        private void UserControlForPaint_Resize(object sender, EventArgs e)
        {
            // when control is resized and it becomes less than square required to draw geometric figures,
            // then keep size of drawing panel equal to size of bounding box
            this.HScroll = false;
            this.VScroll = false; //stop scrollbars from flickering
            if (this.Width>requiredCanvasWidth)  {
                realPaintingPanel.Width = this.Width- realPaintingPanel.Left;
            } else {
                realPaintingPanel.Width = (int)requiredCanvasWidth;
            }

            if (this.Height > requiredCanvasHeight)  {
                realPaintingPanel.Height = this.Height- realPaintingPanel.Top;
            }
            else
            {
                realPaintingPanel.Height = (int)requiredCanvasHeight;
            }

            this.HScroll = true;
            this.VScroll = true;
        }

        private void UserControlForPaint_Click(object sender, EventArgs e)  {
            // triggered almost never
        }

        private void realPaintingPanel_Click(object sender, EventArgs e)    {
            // triggered often
            this.Focus();
        }
    }
}
