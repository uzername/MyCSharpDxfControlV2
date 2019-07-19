using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IxMilia.Dxf;
using IxMilia.Dxf.Entities;
using IxMilia.Dxf.Blocks;

namespace WindowsFormsDXF
{
    public class DxfReadWrapper
    {
        public completeDxfStruct processDxfFile(string in_obtainedFileName)
        {
            completeDxfStruct valueToReturn = new completeDxfStruct();
            DxfFile dxfFile;
            using (System.IO.FileStream fs = new System.IO.FileStream(in_obtainedFileName, System.IO.FileMode.Open))
            {
                dxfFile = DxfFile.Load(fs);
                IList<DxfBlock> allBlocks = dxfFile.Blocks;
                IList<DxfEntity> usedEntities;
                if ((allBlocks.Count == 0) || (allBlocks[0].Name.ToUpper().Contains("MODEL") == false))
                {
                    usedEntities = dxfFile.Entities;
                }
                else
                {
                    usedEntities = allBlocks[0].Entities;
                }

                foreach (DxfEntity entity in dxfFile.Entities)
                {
                    switch (entity.EntityType)
                    {
                        case DxfEntityType.Line:
                            {
                                DxfLine line = (DxfLine)entity;
                                MyDxfLine TransfLine = new MyDxfLine(line.P1.X, line.P1.Y, line.P2.X, line.P2.Y);
                                valueToReturn.addDxfDrawingEntry(TransfLine);
                                break;
                            }
                        case DxfEntityType.Arc:
                            {
                                DxfArc arc = (DxfArc)entity;
                                MyDxfArc TransfArc = new MyDxfArc(arc.Center.X, arc.Center.Y, arc.StartAngle, arc.EndAngle, arc.Radius);
                                valueToReturn.addDxfDrawingEntry(TransfArc);
                                break;
                            }
                        case DxfEntityType.LwPolyline:{ //polyline. It has vertices. 
                                DxfLwPolyline polylineS = entity as DxfLwPolyline;
                                int totalnumberOfVertices = polylineS.Vertices.Count;
                                for (int i=0; i<totalnumberOfVertices-1; i++)
                                { //iterate through vertices, taking them by 2. A figure is between these two
                                    DxfLwPolylineVertex point1 = polylineS.Vertices[i];
                                    DxfLwPolylineVertex point2 = polylineS.Vertices[i+1];
                                    if (point1.Bulge == 0) {
                                        MyDxfLine TransfLine = new MyDxfLine(point1.X, point1.Y, point2.X, point2.Y);
                                        valueToReturn.addDxfDrawingEntry(TransfLine);
                                    } else { //it is arc
                                        // The bulge is the tangent of one fourth the included angle for an arc segment, 
                                        // made negative if the arc goes clockwise from the start point to the endpoint. 
                                        // A bulge of 0 indicates a straight segment, and a bulge of 1 is a semicircle
                                        double angleOfArc = System.Math.Atan(point1.Bulge) * 180.0 / Math.PI * 4;
                                        double startAngle = Math.Atan(point1.Y / point1.X) * 180.0 / Math.PI;
                                        //double endAngle = 

                                    }
                                }

                                break;
                            }
                        case DxfEntityType.Polyline:  {
                                //https://github.com/IxMilia/Dxf/issues/90
                                DxfPolyline polyline = entity as DxfPolyline;
                                
                                break;
                            }
                    }
                }
            }
            return valueToReturn;
        }

    }
}
