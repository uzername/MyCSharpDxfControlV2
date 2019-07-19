//#define OLD_TRANSFORM

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsDXF {

    /// <summary>
    /// A crude way how to perform transformations.
    /// important thing is how to rotate point: https://stackoverflow.com/q/2259476/
    /// </summary>
    public static class MathHelperForTransformations {
        public static double ConvertDegreesToRadians(double degrees)
        {
            double radians = (Math.PI / 180) * degrees;
            return (radians);
        }
        public static double ConvertRadiansToDegrees(double radians)
        {
            return radians * (180.0 / Math.PI);
        }
        // https://www.geeksforgeeks.org/find-mirror-image-point-2-d-plane/
        public static Tuple<double, double> mirrorImage(double a, double b, double c, double x1, double y1)
        {
            double temp = -2 * (a * x1 + b * y1 + c) / (a * a + b * b);
            double x = temp * a + x1;
            double y = temp * b + y1;
            return new Tuple<double, double>(x, y);
        }
        public static Tuple<double, double> mirrorImage(double verticalMirrorX, double x1, double y1) {
            double deltaMirror = Math.Abs(verticalMirrorX-x1);
            double x = x1; double y = y1;
            if (verticalMirrorX > x1) {
                x = x1 + 2 * deltaMirror;
            } else {
                x = x1 - 2 * deltaMirror;
            }
            return new Tuple<double, double>(x, y);
        }

        // https://en.wikipedia.org/wiki/Transformation_matrix#Rotation
        public static Tuple<double, double> rotateImage(double x1, double y1, double THETArad)
        {
            return new Tuple<double, double>(x1 * Math.Cos(THETArad) + y1 * Math.Sin(THETArad), -x1 * Math.Sin(THETArad) + y1 * Math.Cos(THETArad));
        }
        /// <summary>
        /// rotate point px, py around point cx, cy
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="angleRAD">Angle, measured clockwise</param>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <returns></returns>
        public static Tuple<double, double> rotateImage(double cx, double cy, double angleRAD, double px, double py) {
        return new Tuple<double, double>(Math.Cos(angleRAD) * (px - cx) - Math.Sin(angleRAD) * (py - cy) + cx,
                  Math.Sin(angleRAD) * (px - cx) + Math.Cos(angleRAD) * (py - cy) + cy);
        }
        /// <summary>
        /// rotate point px, py around point cx, cy : by 90 degrees clockwise
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <returns></returns>
        public static Tuple<double, double> rotateImage(double cx, double cy, double px, double py)  {
            /*
                return new Tuple<double, double>(0 * (px - cx) - 1 * (py - cy) + cx,
                          1 * (px - cx) + 0 * (py - cy) + cy);
                          */
            double s = 1;
            double c = 0;
            // translate point back to origin:
            double px1 = px- cx;
            double py1 = py- cy;
            // rotate point
            double xnew = px1 * c + py1 * s;
            double ynew = -px1 * s + py1 * c;

            // translate point back:
            px1 = xnew + cx;
            py1 = ynew + cy;
            return new Tuple<double, double>(px1, py1);
        }

        /// <summary>
        /// rotate point px, py around point cx, cy : by -90 degrees clockwise
        /// </summary>
        /// <param name="cx"></param>
        /// <param name="cy"></param>
        /// <param name="px"></param>
        /// <param name="py"></param>
        /// <returns></returns>
        public static Tuple<double, double> rotateImage2(double cx, double cy, double px, double py)
        {
            /*
                return new Tuple<double, double>(0 * (px - cx) - 1 * (py - cy) + cx,
                          1 * (px - cx) + 0 * (py - cy) + cy);
                          */
            double s = 1;
            double c = 0;
            // translate point back to origin:
            double px1 = px - cx;
            double py1 = py - cy;
            // rotate point
            double xnew = px1 * c - py1 * s;
            double ynew = px1 * s + py1 * c;

            // translate point back:
            px1 =  cx + xnew;
            py1 =  cy + ynew;
            return new Tuple<double, double>(px1, py1);
        }
        /// <summary>
        /// multiply two 2d matrices 
        /// <seealso cref="http://dev.bratched.fr/en/fun-with-matrix-multiplication-and-unsafe-code/"/> ;;
        /// <seealso cref="https://stackoverflow.com/questions/6311309/how-can-i-multiply-two-matrices-in-c"/> 
        /// </summary>
        /// <returns></returns>
        public static double[,] CrudeMultiplication2(double[,] m1, double[,] m2, int m1_rows, int m1_cols, int m2_rows, int m2_cols)
        {
            if (m1_cols != m2_rows) {
                return null;
            }
            int resultMatrixHeight = m1_rows; int resultMatrixWidth = m2_cols;
            double[,] resultMatrix = new double[resultMatrixHeight, resultMatrixWidth];

            for (int i=0; i<resultMatrixHeight; i++)  {
                for (int j = 0; j < resultMatrixWidth; j++)  {
                    resultMatrix[i, j] = 0;
                    for (int k = 0; k<m1_cols; k++)  {
                        resultMatrix[i, j] += m1[i, k] * m2[k, j];
                    }
                }
            }

            return resultMatrix;
        }

        public static double[,] getRotationMatrixAroundPoint(double cx, double cy, double angleRAD)
        {
            double[,] translationMtr1 = new double[,] { { 1.0, 0.0, cx }, { 0.0, 1.0, cy }, { 0.0, 0.0, 1.0 } };
            double[,] rotationMtr = new double[,] { { Math.Cos(angleRAD), -Math.Sin(angleRAD), 0.0 }, { Math.Sin(angleRAD), Math.Cos(angleRAD), 0.0 }, { 0.0, 0.0, 1.0 } };
            double[,] translationMtr2 = new double[,] { { 1.0, 0.0, -cx }, { 0.0, 1.0, -cy }, { 0.0, 0.0, 1.0 } };
            return CrudeMultiplication2(CrudeMultiplication2(translationMtr1, rotationMtr, 3, 3, 3, 3), translationMtr2, 3, 3, 3, 3);
        }

        /// <summary>
        /// rotate point px, py around point cx, cy : by angleRad . Returns new point coordinatez
        /// https://math.stackexchange.com/questions/2093314/rotation-matrix-of-rotation-around-a-point-other-than-the-origin
        /// </summary>
        public static Tuple<double, double> rotateImageUsingMatrix(double cx, double cy, double angleRAD, double px, double py)
        {
            double[,] coordinateMtrInitial = new double[,] { { px }, { py }, { 1.0 } };
            double[,] coordinateMtr = CrudeMultiplication2(getRotationMatrixAroundPoint(cx,cy,angleRAD), coordinateMtrInitial, 3, 3, 3, 1);
            return new Tuple<double, double>(coordinateMtr[0,0], coordinateMtr[1,0]);

        }
        /// <summary>
        /// apply transformation encoded in in_TransfrmMtr to the point declared by px and py. in_TransfrmMtr should have 3 rows and 3 cols. It is double 2d array. Returns new point coordinatez
        /// </summary>
        public static Tuple<double,double> rotateImageUsingPrecalculatedTransformationMatrix(double[,] in_TransfrmMtr, double px, double py)  {
            double[,] coordinateMtrInitial = new double[,] { { px }, { py }, { 1.0 } };
            double[,] coordinateMtr = CrudeMultiplication2(in_TransfrmMtr, coordinateMtrInitial, 3, 3, 3, 1);
            return new Tuple<double, double>(coordinateMtr[0, 0], coordinateMtr[1, 0]);
        }
    }


    public struct MyDxfBoundingBox {
        public double XLowerLeft;
        public double YLowerLeft;
        public double XUpperRight;
        public double YUpperRight;
    }
    public abstract class DXFdrawingEntry {
        public abstract MyDxfBoundingBox GetBoundingBox() ;
    }
    public class MyDxfArc : DXFdrawingEntry {
          public override MyDxfBoundingBox GetBoundingBox() {
            MyDxfBoundingBox valueToReturn = new MyDxfBoundingBox();
            List<int> obtainedQuarters = getAngleQuarters();
            double x1 = XCenter + Radius * Math.Cos(StartAngleRad);
            double y1 = YCenter + Radius * Math.Sin(StartAngleRad);
            double x2 = XCenter + Radius * Math.Cos(EndAngleRad);
            double y2 = YCenter + Radius * Math.Sin(EndAngleRad);
            if (obtainedQuarters[0]==obtainedQuarters[1]) {
                valueToReturn.XLowerLeft = (x1 < x2) ? x1 : x2;
                valueToReturn.XUpperRight = (x1 > x2) ? x1 : x2;
                valueToReturn.YLowerLeft = (y1 < y2) ? y1 : y2;
                valueToReturn.YUpperRight = (y2 < y1) ? y1 : y2;
            } else {
            if ((obtainedQuarters[0]==1)&&(obtainedQuarters[1]==2)) {
                    valueToReturn.XLowerLeft = x2;
                    valueToReturn.XUpperRight = x1;
                    valueToReturn.YLowerLeft = (y1 < y2) ? y1 : y2;
                    valueToReturn.YUpperRight = YCenter + Radius;
                } else {
                    if ((obtainedQuarters[0] == 1) && (obtainedQuarters[1] == 3)) {
                        valueToReturn.XLowerLeft = XCenter - Radius;
                        valueToReturn.XUpperRight = x1;
                        valueToReturn.YLowerLeft = y2;
                        valueToReturn.YUpperRight = YCenter + Radius;
                    } else {
                       if ((obtainedQuarters[0]==2)&&(obtainedQuarters[1]==3)) {
                            valueToReturn.XLowerLeft = XCenter - Radius;
                            valueToReturn.XUpperRight = x1<x2?x2:x1;
                            valueToReturn.YLowerLeft = y2;
                            valueToReturn.YUpperRight = y1;
                        } else {
                            if ((obtainedQuarters[0] == 3) && (obtainedQuarters[1] == 4)) {
                                valueToReturn.XLowerLeft = x2;
                                valueToReturn.XUpperRight = x1;
                                valueToReturn.YLowerLeft = YCenter + Radius;
                                valueToReturn.YUpperRight = y1 < y2 ? y2 : y1;
                            }else {
                                if ((obtainedQuarters[0] == 2) && (obtainedQuarters[1] == 4)) {
                                    valueToReturn.XLowerLeft = XCenter-Radius;
                                    valueToReturn.XUpperRight = x2;
                                    valueToReturn.YLowerLeft = YCenter - Radius;
                                    valueToReturn.YUpperRight = y1;
                                } else {
                                    if ((obtainedQuarters[0]==1)&&(obtainedQuarters[1]==4)) {
                                        valueToReturn.XLowerLeft = XCenter - Radius;
                                        valueToReturn.XUpperRight = x1 < x2 ? x2 : x1;
                                        valueToReturn.YLowerLeft = YCenter - Radius;
                                        valueToReturn.YUpperRight = YCenter + Radius;
                                    } else {
                                        ///////////////////////////////
                                        if ((obtainedQuarters[0] == 2) && (obtainedQuarters[1] == 1)) {
                                            valueToReturn.XLowerLeft = XCenter - Radius;
                                            valueToReturn.XUpperRight = XCenter + Radius;
                                            valueToReturn.YLowerLeft = YCenter - Radius;
                                            valueToReturn.YUpperRight = y1>y2?y1:y2;
                                        } else     {
                                            if ((obtainedQuarters[0] == 4) && (obtainedQuarters[1] == 1))   {
                                                valueToReturn.XLowerLeft = x1 < x2 ? x1 : x2;
                                                valueToReturn.XUpperRight = XCenter + Radius;
                                                valueToReturn.YLowerLeft = y1;
                                                valueToReturn.YUpperRight = y2;
                                            }
                                            else {
                                                if ((obtainedQuarters[0] == 3) && (obtainedQuarters[1] == 1))  {
                                                    valueToReturn.XLowerLeft = x1;
                                                    valueToReturn.XUpperRight = XCenter + Radius;
                                                    valueToReturn.YLowerLeft = YCenter - Radius;
                                                    valueToReturn.YUpperRight = y2;
                                                } else
                                                {
                                                    if ((obtainedQuarters[0] == 3) && (obtainedQuarters[1] == 2))
                                                    {
                                                        valueToReturn.XLowerLeft = x1<x2? x1:x2;
                                                        valueToReturn.XUpperRight = XCenter + Radius;
                                                        valueToReturn.YLowerLeft = y1;
                                                        valueToReturn.YUpperRight = YCenter+Radius;
                                                    } else
                                                    {
                                                        if ((obtainedQuarters[0] == 4) && (obtainedQuarters[1] == 3))   {
                                                            valueToReturn.XLowerLeft = XCenter - Radius;
                                                            valueToReturn.XUpperRight = XCenter + Radius;
                                                            valueToReturn.YLowerLeft = y1<y2?y1:y2;
                                                            valueToReturn.YUpperRight = YCenter + Radius;
                                                        } else
                                                        {
                                                            if ((obtainedQuarters[0] == 4) && (obtainedQuarters[1] == 2))
                                                            {
                                                                valueToReturn.XLowerLeft = x2;
                                                                valueToReturn.XUpperRight = XCenter + Radius;
                                                                valueToReturn.YLowerLeft = y1;
                                                                valueToReturn.YUpperRight = YCenter + Radius;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            return valueToReturn;
          }
        /// <summary>
        /// obtain quarters of start and end angles
        /// </summary>
        /// <returns>list with two values (1..4). One is for start angle, other is for stop angle</returns>
        private List<int> getAngleQuarters() {
            List<int> valueToReturn = new List<int>();
            valueToReturn.Add(0); valueToReturn.Add(0);
            if (((StartAngleRad >= 0) && (StartAngleRad <= Math.PI / 2.0)) || ((StartAngleRad <= -2.0 * Math.PI) && (StartAngleRad >= (-3.0 * Math.PI / 2.0)))) {
                valueToReturn[0] = 1;
            } else {
                if (((StartAngleRad > Math.PI / 2.0) && (StartAngleRad <= Math.PI)) || ((StartAngleRad >= -Math.PI) && (StartAngleRad <= (-3.0 * Math.PI / 2.0)))) {
                    valueToReturn[0] = 2;
                } else {
                    if (((StartAngleRad > Math.PI) && (StartAngleRad <= 3.0 * Math.PI / 2.0)) || ((StartAngleRad >= -Math.PI / 2.0) && (StartAngleRad <= -Math.PI))) {
                        valueToReturn[0] = 3;
                    } else {
                        if (((StartAngleRad > 3.0 * Math.PI / 2.0) && (StartAngleRad <= 2.0 * Math.PI)) || ((StartAngleRad >= -Math.PI / 2.0) && (StartAngleRad < 0.0))) {
                            valueToReturn[0] = 4;
                        }
                    }
                }
            }

            double EndAngleRadUsed = EndAngleRad;
            if (doneTransformationOfAngle)   {
                EndAngleRadUsed = EndAngleRad - 2.0 * Math.PI;
            }

            if (((EndAngleRadUsed >= 0) && (EndAngleRadUsed <= Math.PI / 2.0)) || ((EndAngleRadUsed <= -2.0 * Math.PI) && (EndAngleRadUsed >= (-3.0 * Math.PI / 2.0)))) {
                valueToReturn[1] = 1;
            } else {
                if (((EndAngleRadUsed > Math.PI / 2.0) && (EndAngleRadUsed <= Math.PI)) || ((EndAngleRadUsed >= -Math.PI) && (EndAngleRadUsed <= (-3.0 * Math.PI / 2.0)))) {
                    valueToReturn[1] = 2;
                } else {
                    if (((EndAngleRadUsed > Math.PI) && (EndAngleRadUsed <= 3.0 * Math.PI / 2.0)) || ((EndAngleRadUsed >= -Math.PI / 2.0) && (EndAngleRadUsed <= -Math.PI))) {
                        valueToReturn[1] = 3;
                    } else {
                        if (((EndAngleRadUsed > 3.0 * Math.PI / 2.0) && (EndAngleRadUsed <= 2.0 * Math.PI)) || ((EndAngleRadUsed >= -Math.PI / 2.0) && (EndAngleRadUsed < 0.0))) {
                            valueToReturn[1] = 4;
                        }
                    }
                }
            }

            return valueToReturn;
        }
        public MyDxfArc(double in_XCenter, double in_YCenter, double in_StartDegreeAngle, double in_EndDegreeAngle, double in_Radius) {
            XCenter = in_XCenter;
            YCenter = in_YCenter;
            StartAngleRad = MathHelperForTransformations.ConvertDegreesToRadians(in_StartDegreeAngle);
            StartAngleDegree = in_StartDegreeAngle;
            EndAngleDegree = in_EndDegreeAngle;
            EndAngleRad = MathHelperForTransformations.ConvertDegreesToRadians(in_EndDegreeAngle);
            doneTransformationOfAngle = false;
            if (EndAngleRad<StartAngleRad) {
                EndAngleRad += 2 * Math.PI;
                doneTransformationOfAngle = true;
            }
            Radius = in_Radius;
            //calculate parameters required for drawing. C# arc has a weird definition
            //https://docs.microsoft.com/en-us/dotnet/api/system.drawing.graphics.drawarc
            drawStartAngleDegreeCW = in_StartDegreeAngle;
            double drawEndAngleDegreeCW = in_EndDegreeAngle != 0 ? in_EndDegreeAngle : 359.9999999999999999;
            
            if (drawEndAngleDegreeCW < drawStartAngleDegreeCW) {
                /*
                    double tmpAngle = drawStartAngleDegreeCW;
                    drawStartAngleDegreeCW = drawEndAngleDegreeCW;
                    drawEndAngleDegreeCW = tmpAngle;
                 */
                drawEndAngleDegreeCW += 360.0;
            }
            
            drawSweepAngleDegree = drawEndAngleDegreeCW - drawStartAngleDegreeCW;

            setupDrawingParameters();
        }
        private void setupDrawingParameters()         {
            //From getBoundingBox we have lower left and upper right points of rectangle delimiting arc. BUT we need a rectangle limiting circle, for drawing
            //But we are interested in upper left point
            drawUpperLeftX = XCenter - Radius; drawUpperLeftY = YCenter - Radius;
            drawDimensionHorizontal = 2.0*Radius;
            drawDimensionVertical = 2.0*Radius;
        }

        public bool doneTransformationOfAngle;
        public double XCenter;
        public double YCenter;
        public double StartAngleRad;
        /// <summary>
        /// DO NOT THINK ABOUT MODIFYING THIS MANUALLY. it is set up only during constructor works and accessed during rotation
        /// </summary>
        public double StartAngleDegree;
        public double EndAngleRad;
        /// <summary>
        /// DO NOT THINK ABOUT MODIFYING THIS MANUALLY. it is set up only during constructor works and accessed during rotation
        /// </summary>
        public double EndAngleDegree;
        //drawing
        public double drawStartAngleDegreeCW;
        public double drawSweepAngleDegree;
        public double drawUpperLeftX;
        public double drawUpperLeftY;
        public double drawDimensionHorizontal;
        public double drawDimensionVertical;
        public double Radius;

    }
    public class MyDxfLine : DXFdrawingEntry
    {
        public override MyDxfBoundingBox GetBoundingBox()
        {
            MyDxfBoundingBox valueToReturn = new MyDxfBoundingBox();
            valueToReturn.XLowerLeft = XStart < XEnd ? XStart : XEnd;
            valueToReturn.YLowerLeft = YStart < YEnd ? YStart : YEnd;
            valueToReturn.XUpperRight = XStart < XEnd ? XEnd : XStart;
            valueToReturn.YUpperRight = YStart < YEnd ? YEnd: YStart ;
            return valueToReturn;
        }

            public MyDxfLine(double in_XStart, double in_YStart, double in_XEnd, double in_YEnd){
            XStart = in_XStart; YStart = in_YStart; XEnd = in_XEnd; YEnd = in_YEnd;
        }
        public double XStart; public double YStart;
        public double XEnd; public double YEnd;

    }
    public class completeDxfStruct {
        private List<DXFdrawingEntry> AllDXFdrawingEntries = new List<DXFdrawingEntry>();
        private MyDxfBoundingBox currentBoundingBox = new MyDxfBoundingBox();
        public void addDxfDrawingEntry(DXFdrawingEntry in_DxfEntry) {
            AllDXFdrawingEntries.Add(in_DxfEntry);
            MyDxfBoundingBox obtainedDxfBoundingBox = in_DxfEntry.GetBoundingBox();
            if (AllDXFdrawingEntries.Count > 1) {
                currentBoundingBox.XLowerLeft = currentBoundingBox.XLowerLeft < obtainedDxfBoundingBox.XLowerLeft ? currentBoundingBox.XLowerLeft : obtainedDxfBoundingBox.XLowerLeft;
                currentBoundingBox.YLowerLeft = currentBoundingBox.YLowerLeft < obtainedDxfBoundingBox.YLowerLeft ? currentBoundingBox.YLowerLeft : obtainedDxfBoundingBox.YLowerLeft;
                currentBoundingBox.XUpperRight = currentBoundingBox.XUpperRight > obtainedDxfBoundingBox.XUpperRight ? currentBoundingBox.XUpperRight : obtainedDxfBoundingBox.XUpperRight;
                currentBoundingBox.YUpperRight = currentBoundingBox.YUpperRight > obtainedDxfBoundingBox.YUpperRight ? currentBoundingBox.YUpperRight : obtainedDxfBoundingBox.YUpperRight;
            } else {
                currentBoundingBox = obtainedDxfBoundingBox;
            }
        }
        public void recalculateBoundingBoxFromScratch()
        {
            if ((AllDXFdrawingEntries == null) || (AllDXFdrawingEntries.Count <= 0))  {
                return;
            }
            MyDxfBoundingBox obtainedBoundingBox = AllDXFdrawingEntries[0].GetBoundingBox();
            currentBoundingBox.XLowerLeft = obtainedBoundingBox.XLowerLeft;
            currentBoundingBox.XUpperRight = obtainedBoundingBox.XUpperRight;
            currentBoundingBox.YLowerLeft = obtainedBoundingBox.YLowerLeft;
            currentBoundingBox.YUpperRight = obtainedBoundingBox.YUpperRight;
            for (int i=1; i<AllDXFdrawingEntries.Count; i++)  {
                obtainedBoundingBox = AllDXFdrawingEntries[i].GetBoundingBox();
                currentBoundingBox.XLowerLeft = currentBoundingBox.XLowerLeft < obtainedBoundingBox.XLowerLeft ? currentBoundingBox.XLowerLeft : obtainedBoundingBox.XLowerLeft;
                currentBoundingBox.YLowerLeft = currentBoundingBox.YLowerLeft < obtainedBoundingBox.YLowerLeft ? currentBoundingBox.YLowerLeft : obtainedBoundingBox.YLowerLeft;
                currentBoundingBox.XUpperRight = currentBoundingBox.XUpperRight > obtainedBoundingBox.XUpperRight ? currentBoundingBox.XUpperRight : obtainedBoundingBox.XUpperRight;
                currentBoundingBox.YUpperRight = currentBoundingBox.YUpperRight > obtainedBoundingBox.YUpperRight ? currentBoundingBox.YUpperRight : obtainedBoundingBox.YUpperRight;
            }

        }

        public MyDxfBoundingBox GetBoundingBox() {
            return currentBoundingBox;
        }

        private bool globalMirrorAdjustment = false;
        public void performMirroringOfDxfStruct() { 
        
        }
        /// <summary>
        /// Return item from internal List of structures
        /// </summary>
        /// <param name="i"></param>
        /// <returns>DXFdrawingEntry</returns>
        public DXFdrawingEntry getItemByIndex(int i)   {
            return AllDXFdrawingEntries[i];
        }
        /// <summary>
        /// get size of internal List of structures
        /// </summary>
        /// <returns></returns>
        public int getSize()  {
            return AllDXFdrawingEntries.Count;
        }

    /// <summary>
    /// rotate all entries in a structure around bounding box center point. Changes coordinates of them
    /// </summary>
    /// <param name="in_deg"></param>
    public void performRotationOfDxfStruct(double in_deg)  {
            //obtain center of rotation
            double horizontalMidPoint = (this.currentBoundingBox.XLowerLeft + this.currentBoundingBox.XUpperRight) / 2.0;
            double verticalMidPoint = (this.currentBoundingBox.YLowerLeft + this.currentBoundingBox.YUpperRight) / 2.0;
            //obtain rotation matrix
            double[,] currentRotationMatrix = MathHelperForTransformations.getRotationMatrixAroundPoint(horizontalMidPoint, verticalMidPoint, MathHelperForTransformations.ConvertDegreesToRadians(in_deg));
            //iterate over all entries in dxf structure altering them
            for (int iii = 0; iii < AllDXFdrawingEntries.Count; iii++) 
            {
                DXFdrawingEntry item = AllDXFdrawingEntries[iii];
                if (item is MyDxfLine)  {
                     Tuple<double,double> coord1 = MathHelperForTransformations.rotateImageUsingPrecalculatedTransformationMatrix(currentRotationMatrix, (item as MyDxfLine).XStart, (item as MyDxfLine).YStart);
                    (item as MyDxfLine).XStart = coord1.Item1;
                    (item as MyDxfLine).YStart = coord1.Item2;
                    Tuple<double, double> coord2 = MathHelperForTransformations.rotateImageUsingPrecalculatedTransformationMatrix(currentRotationMatrix, (item as MyDxfLine).XEnd, (item as MyDxfLine).YEnd);
                    (item as MyDxfLine).XEnd = coord2.Item1;
                    (item as MyDxfLine).YEnd = coord2.Item2;
                } else if (item is MyDxfArc) {
                    //center coordinates
                    Tuple<double, double> coordCenter = MathHelperForTransformations.rotateImageUsingPrecalculatedTransformationMatrix(currentRotationMatrix, (item as MyDxfArc).XCenter, (item as MyDxfArc).YCenter);
                    // Rotate start and stop angles. They are pointed by vectors, coming from Center and by magnitube of Radius
                    // https://www.youtube.com/watch?v=g9lgL6f3h90
                    /*
                    double startAngleCoordinateX = (item as MyDxfArc).XCenter + (item as MyDxfArc).Radius * Math.Cos((item as MyDxfArc).StartAngleRad);
                    double startAngleCoordinateY = (item as MyDxfArc).YCenter + (item as MyDxfArc).Radius * Math.Sin((item as MyDxfArc).StartAngleRad);
                    double endAngleCoordinateX = (item as MyDxfArc).XCenter + (item as MyDxfArc).Radius * Math.Cos((item as MyDxfArc).EndAngleRad);
                    double endAngleCoordinateY = (item as MyDxfArc).YCenter + (item as MyDxfArc).Radius * Math.Sin((item as MyDxfArc).EndAngleRad);
                    //rotate vector which corresponds to start angle
                    Tuple<double, double> startAnglePointNew = MathHelperForTransformations.rotateImageUsingPrecalculatedTransformationMatrix(currentRotationMatrix, (startAngleCoordinateX ), (startAngleCoordinateY ));
                    double theta1Start = Math.Asin(Math.Abs(startAnglePointNew.Item2 - coordCenter.Item2) / (item as MyDxfArc).Radius);
                    double startAngleDegree = MathHelperForTransformations.ConvertRadiansToDegrees(theta1Start);
                    //rotate vector which corresponds to end angle
                    Tuple<double, double> endAnglePointNew = MathHelperForTransformations.rotateImageUsingPrecalculatedTransformationMatrix(currentRotationMatrix, (endAngleCoordinateX ), (endAngleCoordinateY ));
                    double theta2End = Math.Asin(Math.Abs(endAnglePointNew.Item2 - coordCenter.Item2) / (item as MyDxfArc).Radius);
                    double endAngleDegree = MathHelperForTransformations.ConvertRadiansToDegrees(theta2End);
                    */
                    double startAngleDegree = (item as MyDxfArc).StartAngleDegree + in_deg;
                    double endAngleDegree = (item as MyDxfArc).EndAngleDegree + in_deg;
                    AllDXFdrawingEntries[iii] = new MyDxfArc(coordCenter.Item1, coordCenter.Item2, startAngleDegree, endAngleDegree, (item as MyDxfArc).Radius);
                }
            }
            //bounding box should be changed too. rotate ALL the coordinates of bounding rectangle and select the new appropriate values

            recalculateBoundingBoxFromScratch();
        }


    }
}