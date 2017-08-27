using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace Project_Owl
{
    public class Owl_Info : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "KinectV23";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("1e9a0843-894b-49cc-b5e9-bf17f4cd30a2");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "";
            }
        }
    }
}
