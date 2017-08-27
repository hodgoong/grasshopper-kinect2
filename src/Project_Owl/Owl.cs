// Developed by Hojoong Chung(hodgoong@gmail.com) and Giulio Brugnaro(giuliobrugnaro@gmail.com)
// Last update: 15 SEP 2015

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Threading;

using Grasshopper.Kernel;
using Rhino.Geometry;
using Microsoft.Kinect;
using Grasshopper;

namespace Project_Owl
{
    public class Owl : GH_Component
    {
        private KinectSensor kinectSensor = null;
        private List<Point3d> pointCloud = null;
        private List<System.Drawing.Color> pointCloudColor = null;
        public int resolution = 1;
        public double depth = 8.00;
        public Point3d origin;


        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public Owl()
            : base("Owl_KinectV2", "Owl_KinectV2", "Kinect V2 Point Cloud Generator",
                "Owl", "PointCloud"){}

        public void CallExpireSolution()
        {
            this.ExpireSolution(true);
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Origin of Pointcloud", "O", "Origin of pointcoud", GH_ParamAccess.item);
            pManager.AddIntegerParameter("Resolution of Pointcloud", "R", "Resolution of pointcoud", GH_ParamAccess.item);
            pManager.AddNumberParameter("Depth of Pointcloud", "D", "Depth of pointcoud", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Pointcloud", "PC", "Pointcloud", GH_ParamAccess.list);
            pManager.AddColourParameter("Color of Pointcloud", "C", "Color of Pointcloud", GH_ParamAccess.list);
        }

        private void ScheduleDelegate(GH_Document doc)
        {
            ExpireSolution(false);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DA.GetData<Point3d>(0, ref origin);
            DA.GetData<int>(1, ref resolution);
            DA.GetData<double>(2, ref depth);
            int res;
            double dep;
            Point3d org = origin;

            if (resolution < 1)
            {
                res = 1;
            }
            else
            {
                res = resolution;
            }

            if (depth < 0)
            {
                dep = 8.00;
            }
            else
            {
                dep = depth;
            }
            
            if (this.kinectSensor == null)
            {
                KinectController.AddRef();
                this.kinectSensor = KinectController.sensor;
                KinectController.kinectGHC = this;
            }

            if (this.kinectSensor != null)
            {
                if (KinectController.cameraSpacePoints != null && KinectController.colorSpacePoints != null)
                {
                    pointCloud = new List<Point3d>();
                    pointCloudColor = new List<System.Drawing.Color>();

                    for (int i = 0; i < KinectController.cameraSpacePoints.Length; i += res)
                    {
                        CameraSpacePoint p = KinectController.cameraSpacePoints[i];
                        Point3d pt = new Point3d();

                        if (p.Z > dep)
                        {
                            continue;
                        }

                        else if(p.Z <= dep)
                        {
                            if (System.Single.IsNegativeInfinity(p.X) == true)
                            {
                                continue;
                            }

                            else if (System.Single.IsNegativeInfinity(p.X) == false)
                            {
                                ColorSpacePoint colPt = KinectController.colorSpacePoints[i];

                                int colorX = (int)Math.Floor(colPt.X+0.5);
                                int colorY = (int)Math.Floor(colPt.Y+0.5);


                                if ((colorX >= 0) && (colorX < KinectController.colorWidth) && (colorY >= 0) && (colorY < KinectController.colorHeight))
                                {
                                    int colorIndex = ((colorY * KinectController.colorWidth) + colorX) * KinectController.bytesPerPixel;
                                    Byte b = 0; Byte g = 0; Byte r = 0;

                                    b = KinectController.colorFrameData[colorIndex++];
                                    g = KinectController.colorFrameData[colorIndex++];
                                    r = KinectController.colorFrameData[colorIndex++];

                                    System.Drawing.Color color = System.Drawing.Color.FromArgb(r, g, b);

                                    pt.X = p.X * 1000 + org.X;
                                    pt.Y = p.Z * 1000 + org.Y;
                                    pt.Z = p.Y * 1000 + org.Z;

                                    pointCloud.Add(pt);
                                    pointCloudColor.Add(color);
                                }
                            }
                        }
                    }
                }
                    DA.SetDataList(0, pointCloud);
                    DA.SetDataList(1, pointCloudColor);
                    //kinectManager.RemoveRef();
            }
            base.OnPingDocument().ScheduleSolution(20, new GH_Document.GH_ScheduleDelegate(ScheduleDelegate));
        }


        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return Properties.Resources.icon;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("{5fdf336d-68ef-47bc-abf9-a83f2c48a3ab}"); }
        }
    }
}
