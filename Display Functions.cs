using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MeshGeometry;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace SU2_Viewer
{
    public partial class Form1 : Form
    {
        /// <summary>
        /// Display Data for current GUI
        /// </summary>
        private DisplayData mDisplayData = new DisplayData();

        /// <summary>
        /// Large dimension for setting display limit
        /// </summary>
        private static float BOX_CONSTANT = 1e8f;

        /// <summary>
        /// OpenGL Paint Method
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl_Paint(object sender, PaintEventArgs e)
        {
            // if (currentMesh != null)

            GLInit();
            if (!mDisplayListCreated)
            {
                mDisplayListID = GL.GenLists(1);
                GL.NewList(mDisplayListID, ListMode.Compile);
                mDisplayListCreated = true;
                if (currentMesh != null)
                {
                    GL.Color3(mDisplayData.materialColor);
                    currentMesh.Draw();
                    GL.Color3(Color.Black);
                    //GL.LineWidth(5);
                    //for (int i = 0; i < currentMesh.elements.Count; i++)
                    //{
                    //    List<Edge> edges = currentMesh.elements[i].GetEdges();

                    //    for (int j = 0; j < edges.Count; j++)
                    //        edges[j].Draw();
                    //}
                }
                GL.EndList();
            }
            GL.CallList(mDisplayListID);
            glControl.SwapBuffers();
        }

        /// <summary>
        /// Graphics Initialising Method
        /// </summary>
        private void GLInit()
        {
            double mx = (mDisplayData.XMin + mDisplayData.XMax) * 0.5;
            double my = (mDisplayData.YMin + mDisplayData.YMax) * 0.5;
            double mz = (mDisplayData.ZMin + mDisplayData.ZMax) * 0.5;
            double lx = mDisplayData.XMax - mDisplayData.XMin;
            double ly = mDisplayData.YMax - mDisplayData.YMin;
            double lz = mDisplayData.ZMax - mDisplayData.ZMin;
            GL.ClearColor(Color.LightSkyBlue);
            GL.Viewport(0, 0, glControl.Width, glControl.Height);

            //GL.Viewport(0, 0, this.Width, this.Height);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Disable(EnableCap.Lighting);
            GL.Disable(EnableCap.ClipPlane0);
            GL.Begin(BeginMode.Quads);
            double z = 0.99;
            GL.Color3(mDisplayData.bottomColor);
            GL.Vertex3(-1.0, -1.0, z);
            GL.Vertex3(1.0, -1.0, z);

            GL.Color3(mDisplayData.topColor);
            GL.Vertex3(1.0, 1.0, z);
            GL.Vertex3(-1.0, 1.0, z);
            GL.End();

            GL.MatrixMode(MatrixMode.Projection);
            GL.PopMatrix();

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            float[] light_position0 = { BOX_CONSTANT, BOX_CONSTANT, -BOX_CONSTANT };
            float[] light_position1 = { -BOX_CONSTANT, BOX_CONSTANT, BOX_CONSTANT };
            float[] light_position2 = { -BOX_CONSTANT, -BOX_CONSTANT, -BOX_CONSTANT };

            float[] light_ambient = { 0.2f, 0.2f, 0.2f, 1.0f };
            float[] light_diffuse = { 0.0f, 0.0f, 0.0f, 1.0f };
            float[] light_diffuse1 = { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] light_diffuse2 = { 0.8f, 0.8f, 0.8f, 1.0f };
            float[] light_specular = { 0.4f, 0.4f, 0.4f, 1.0f };

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.Light0);
            GL.Enable(EnableCap.Light1);
            GL.Enable(EnableCap.Light2);
            if (!mDisplayData.transparent)
                GL.Enable(EnableCap.DepthTest);
            else
                GL.Disable(EnableCap.DepthTest);
            GL.Light(LightName.Light0, LightParameter.Position, light_position0);
            GL.Light(LightName.Light0, LightParameter.Diffuse, light_diffuse);
            GL.Light(LightName.Light1, LightParameter.Position, light_position1);
            GL.Light(LightName.Light1, LightParameter.Diffuse, light_diffuse1);
            GL.Light(LightName.Light2, LightParameter.Position, light_position2);
            GL.Light(LightName.Light2, LightParameter.Diffuse, light_diffuse2);
            GL.Enable(EnableCap.ColorMaterial);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            double min = Math.Min(mx - lx, my - ly);
            double max = Math.Max(mx + lx, my + ly);
            GL.Ortho(min, max,
                min / mDisplayData.aspectRatio, max / mDisplayData.aspectRatio, Math.Min(Math.Min(mx - 5.1 * lx, my - 5.1 * ly), mz - 5.1 * lz),
                Math.Max(Math.Max(mx + 5.1 * lx, my + 5.1 * ly), mz + 5.1 * lz));
            GL.Translate(mDisplayData.TranslateX, mDisplayData.TranslateY, 0);
            GL.Scale(mDisplayData.ScaleX, mDisplayData.ScaleY, mDisplayData.ScaleZ);
            drawAxes();
            GL.Rotate(mDisplayData.RotateX, 1, 0, 0);
            GL.Rotate(mDisplayData.RotateY, 0, 1, 0);
            GL.Rotate(mDisplayData.RotateZ, 0, 0, 1);
            GL.Enable(EnableCap.Normalize);
            GL.ClearColor(Color.Black);
            GL.ShadeModel(ShadingModel.Smooth);
        }

        /// <summary>
        /// Method to draw Axes in bottom left corner
        /// </summary>
        private void drawAxes()
        {
            double length = 0.05;
            GL.PushMatrix();
            GL.LoadIdentity();
            GL.Translate(-0.85, -0.75, -0.9);
            float size = 1.7f;
            DrawSphere(OpenTK.Vector3.Zero, 0.006f * size, 16);

            GL.Rotate(mDisplayData.RotateX, 1, 0, 0);
            GL.Rotate(mDisplayData.RotateY, 0, 1, 0);
            GL.Rotate(mDisplayData.RotateZ, 0, 0, 1);
            GL.Color3(Color.Blue);
            Arrow(0, 0, 0, 0, 0, 1.7 * length, 0.0039 * size);
            GL.Color3(Color.Red);
            Arrow(0, 0, 0, 1.7 * length, 0, 0, 0.0035 * size);
            GL.Color3(Color.Green);
            Arrow(0, 0, 0, 0, 2.5 * length, 0, 0.003 * size);

            GL.PopMatrix();
        }

        /// <summary>
        /// Method to draw a sphere
        /// </summary>
        /// <param name="Center"> Center co-ordinates of sphere</param>
        /// <param name="Radius">Radius of sphere</param>
        /// <param name="Precision">Accuracy of sphere display</param>
        private static void DrawSphere(OpenTK.Vector3 Center, float Radius, int Precision)
        {
            double ai, aj, aj1;
            double HalfPI = Math.PI * 0.5;

            if (Precision % 2 > 0) Precision = Precision + 1;

            if (Radius < 0.0f)
                Radius = -Radius;

            if (Radius == 0) Radius = 1.0f;

            if (Precision < 8) Precision = 8;

            double OneThroughPrecision = 1.0 / Convert.ToDouble(Precision);
            double TwoPIThroughPrecision = 2.0 * Math.PI * OneThroughPrecision;

            double theta1, theta2, theta3;
            OpenTK.Vector3 Normal;
            OpenTK.Vector3 Position;

            for (int j = 0; j <= Precision / 2 - 1; j++)
            {
                aj = Convert.ToDouble(j);
                aj1 = Convert.ToDouble(j + 1);
                theta1 = (aj * TwoPIThroughPrecision) - HalfPI;
                theta2 = (aj1 * TwoPIThroughPrecision) - HalfPI;
                GL.Begin(BeginMode.TriangleStrip);

                for (int i = 0; i <= Precision; i++)
                {
                    ai = Convert.ToDouble(i);
                    theta3 = ai * TwoPIThroughPrecision;
                    Normal.X = Convert.ToSingle(Math.Cos(theta2) * Math.Cos(theta3));
                    Normal.Y = Convert.ToSingle(Math.Sin(theta2));
                    Normal.Z = Convert.ToSingle(Math.Cos(theta2) * Math.Sin(theta3));
                    Position.X = Center.X + Radius * Normal.X;
                    Position.Y = Center.Y + Radius * Normal.Y;
                    Position.Z = Center.Z + Radius * Normal.Z;

                    GL.Normal3(Normal);
                    GL.TexCoord2(ai * OneThroughPrecision, 2.0 * aj1 * OneThroughPrecision);
                    GL.Vertex3(Position);

                    Normal.X = Convert.ToSingle(Math.Cos(theta1) * Math.Cos(theta3));
                    Normal.Y = Convert.ToSingle(Math.Sin(theta1));
                    Normal.Z = Convert.ToSingle(Math.Cos(theta1) * Math.Sin(theta3));
                    Position.X = Center.X + Radius * Normal.X;
                    Position.Y = Center.Y + Radius * Normal.Y;
                    Position.Z = Center.Z + Radius * Normal.Z;

                    GL.Normal3(Normal);
                    GL.TexCoord2(ai * OneThroughPrecision, 2.0 * aj * OneThroughPrecision);
                    GL.Vertex3(Position);
                }
                GL.End();
            }
        }

        /// <summary>
        /// Method to draw Arrows for axes
        /// </summary>
        /// <param name="x1">Arrow cylinder base X</param>
        /// <param name="y1">Arrow cylinder base Y</param>
        /// <param name="z1">Arrow cylinder base Z</param>
        /// <param name="x2">Arrow Cone base X</param>
        /// <param name="y2">Arrow Cone base Y</param>
        /// <param name="z2">Arrow Cone base Z</param>
        /// <param name="D">Arrow cylinder diameter</param>
        private static void Arrow(double x1, double y1, double z1, double x2, double y2, double z2, double D)
        {
            double x = x2 - x1;
            double y = y2 - y1;
            double z = z2 - z1;
            double L = Math.Sqrt(x * x + y * y + z * z);
            double RADPERDEG = 0.0174533;
#pragma warning disable 0618
            IntPtr quadObj = OpenTK.Graphics.Glu.NewQuadric();

            GL.PushMatrix();

            GL.Translate(x1, y1, z1);

            if ((x != 0) || (y != 0))
            {
                GL.Rotate(Math.Atan2(y, x) / RADPERDEG, 0, 0, 1);
                GL.Rotate(Math.Atan2(Math.Sqrt(x * x + y * y), z) / RADPERDEG, 0, 1, 0);
            }
            else if (z < 0)
            {
                GL.Rotate(180, 1, 0, 0);
            }

            GL.Translate(0, 0, L - 4 * D);

            quadObj = OpenTK.Graphics.Glu.NewQuadric();
            OpenTK.Graphics.Glu.QuadricDrawStyle(quadObj, OpenTK.Graphics.QuadricDrawStyle.Fill);
            OpenTK.Graphics.Glu.QuadricNormal(quadObj, OpenTK.Graphics.QuadricNormal.Smooth);
            OpenTK.Graphics.Glu.Cylinder(quadObj, 2 * D, 0.0, 4 * D, 32, 1);
            OpenTK.Graphics.Glu.DeleteQuadric(quadObj);

            quadObj = OpenTK.Graphics.Glu.NewQuadric();
            OpenTK.Graphics.Glu.QuadricDrawStyle(quadObj, OpenTK.Graphics.QuadricDrawStyle.Fill);
            OpenTK.Graphics.Glu.QuadricNormal(quadObj, OpenTK.Graphics.QuadricNormal.Smooth);
            OpenTK.Graphics.Glu.Disk(quadObj, 0.0, 2 * D, 32, 1);
            OpenTK.Graphics.Glu.DeleteQuadric(quadObj);

            GL.Translate(0, 0, -L + 4 * D);

            quadObj = OpenTK.Graphics.Glu.NewQuadric();
            OpenTK.Graphics.Glu.QuadricDrawStyle(quadObj, OpenTK.Graphics.QuadricDrawStyle.Fill);
            OpenTK.Graphics.Glu.QuadricNormal(quadObj, OpenTK.Graphics.QuadricNormal.Smooth);
            OpenTK.Graphics.Glu.Cylinder(quadObj, D, D, L - 4 * D, 32, 1);
            OpenTK.Graphics.Glu.DeleteQuadric(quadObj);

            quadObj = OpenTK.Graphics.Glu.NewQuadric();
            OpenTK.Graphics.Glu.QuadricDrawStyle(quadObj, OpenTK.Graphics.QuadricDrawStyle.Fill);
            OpenTK.Graphics.Glu.QuadricNormal(quadObj, OpenTK.Graphics.QuadricNormal.Smooth);
            OpenTK.Graphics.Glu.Disk(quadObj, 0.0, D, 32, 1);
            OpenTK.Graphics.Glu.DeleteQuadric(quadObj);

            GL.PopMatrix();
        }

        private void glControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!((Math.Abs(e.X - mouseX) > 0) || ((Math.Abs(e.Y - mouseY) > 0))))
            {
                mouseX = e.X;
                mouseY = e.Y;
                return;
            }

            if (mDisplayData != null)
                if (mousedown)
                {
                    if (e.Button == MouseButtons.Right)
                    {
                        if (e.Y > mouseY)
                        {
                            mDisplayData.ScaleX *= 1.01;
                            mDisplayData.ScaleY *= 1.01;
                            mDisplayData.ScaleZ *= 1.01;
                            this.Cursor = Cursors.PanSouth;
                        }
                        else if (e.Y < mouseY)
                        {
                            mDisplayData.ScaleX /= 1.01;
                            mDisplayData.ScaleY /= 1.01;
                            mDisplayData.ScaleZ /= 1.01;
                            this.Cursor = Cursors.PanNorth;
                        }
                    }
                    else if (e.Button == MouseButtons.Left)
                    {
                        calcMovement(e.X, e.Y, mouseX, mouseY);
                        if (e.Y > mouseY)
                            mDisplayData.RotateX += 2 * Math.Atan2(Math.Abs(e.Y - mouseY), Math.Abs(e.X - mouseX));
                        else
                            mDisplayData.RotateX -= 2 * Math.Atan2(Math.Abs(e.Y - mouseY), Math.Abs(e.X - mouseX));
                        if (e.X > mouseX)
                            mDisplayData.RotateY += 2 * Math.Atan2(Math.Abs(e.X - mouseX), Math.Abs(e.Y - mouseY));
                        else
                            mDisplayData.RotateY -= 2 * Math.Atan2(Math.Abs(e.X - mouseX), Math.Abs(e.Y - mouseY));
                    }
                    else if (e.Button == MouseButtons.Middle)
                    {
                        this.Cursor = Cursors.SizeAll;
                        double size = Math.Min(Math.Min((mDisplayData.XMax - mDisplayData.XMin), (mDisplayData.YMax - mDisplayData.YMin)), (mDisplayData.ZMax - mDisplayData.ZMin));
                        double mouseScaleX = (e.X - mouseX) / (double)glControl.Width;
                        double mouseScaleY = (e.Y - mouseY) / (double)glControl.Height;
                        OpenTK.Matrix4 modelViewMatrix, projectionMatrix;
                        GL.GetFloat(GetPName.ModelviewMatrix, out modelViewMatrix);
                        GL.GetFloat(GetPName.ProjectionMatrix, out projectionMatrix);
                        OpenTK.Vector3 ray_wor1, ray_wor2;

                        ray_wor1 = new OpenTK.Vector3(UnProject(ref projectionMatrix,
                            modelViewMatrix, new System.Drawing.Size(glControl.Width, glControl.Height),
                            new OpenTK.Vector2(-e.X + mouseX, -e.Y + mouseY), 0.5f));
                        ray_wor2 = new OpenTK.Vector3(UnProject(ref projectionMatrix,
                    modelViewMatrix, new System.Drawing.Size(glControl.Width, glControl.Height),
                    new OpenTK.Vector2(mouseX, mouseY), 0.5f));
                        size = (ray_wor1 - ray_wor2).Length;
                        mDisplayData.TranslateX += mouseScaleX * size * mDisplayData.ScaleX;
                        mDisplayData.TranslateY -= mouseScaleY * size * mDisplayData.ScaleY;
                    }

                    glControl.Invalidate();
                }
                else
                {
                    this.Cursor = Cursors.Arrow;
                }

            mouseX = e.X;
            mouseY = e.Y;
        }

        /// <summary>
        /// Method to project a point from screen co-ordinates to world co-ordinates
        /// </summary>
        /// <param name="projection">Projection Matrix</param>
        /// <param name="view">View Matrix</param>
        /// <param name="viewport">Viewport Dimensions</param>
        /// <param name="mouse">Mouse screen co-ordinates</param>
        /// <param name="zLocation">Z-location of projection(-1,1)</param>
        /// <returns></returns>
        public static OpenTK.Vector4 UnProject(ref OpenTK.Matrix4 projection, OpenTK.Matrix4 view, Size viewport, OpenTK.Vector2 mouse, float zLocation)
        {
            OpenTK.Vector4 vec;
            OpenTK.Matrix4 viewInv;
            OpenTK.Matrix4 projInv;
            vec.X = 2.0f * mouse.X / (float)viewport.Width - 1;
            vec.Y = -(2.0f * mouse.Y / (float)viewport.Height - 1);
            vec.Z = zLocation;
            vec.W = 1.0f;

            OpenTK.Matrix4.Invert(ref view, out viewInv);
            OpenTK.Matrix4.Invert(ref projection, out projInv);

            OpenTK.Vector4.Transform(ref vec, ref projInv, out vec);
            OpenTK.Vector4.Transform(ref vec, ref viewInv, out vec);

            if (vec.W > float.Epsilon || vec.W < float.Epsilon)
            {
                vec.X /= vec.W;
                vec.Y /= vec.W;
                vec.Z /= vec.W;
            }

            return vec;
        }

        /// <summary>
        /// MouseDown event for GLCONTROL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl_MouseDown(object sender, MouseEventArgs e)
        {
            mousedown = true;
            mouseX = e.X;
            mouseY = e.Y;
        }

        /// <summary>
        /// MouseUp event for GLCONTROL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void glControl_MouseUp(object sender, MouseEventArgs e)
        {
            mousedown = false;
        }

        /// <summary>
        /// Mouse X position on MouseDown
        /// </summary>
        private int mouseX;

        /// <summary>
        /// Mouse Y position on MouseDown
        /// </summary>
        private int mouseY;

        /// <summary>
        /// Checks whether mouse is down
        /// </summary>
        private bool mousedown;

        #region Rotate

        private Quaternion _rotation = Quaternion.Identity;
        private double _trackballSize = 0.8;

        private double getXnormalized(double x, double width)
        {
            return ((2.0d * x) / width) - 1.0d;
        }

        private double getYnormalized(double y, double height)
        {
            return 1.0d - ((2.0d * y) / height);
        }

        private bool calcMovement(double x1, double y1, double x2, double y2)
        {
            double px0 = getXnormalized(x1, glControl.Width);
            double py0 = getYnormalized(y1, glControl.Height);
            Vector3 axis;
            double angle;
            double px1 = getXnormalized(x2, glControl.Width);
            double py1 = getYnormalized(y2, glControl.Height);
            trackball(out axis, out angle, px1, py1, px0, py0);
            Quaternion new_rotate = new Quaternion(axis, (float)(180.0 * angle / Math.PI));
            _rotation = _rotation * new_rotate;
            return true;
        }

        private void trackball(out Quaternion q, double p1x, double p1y, double p2x, double p2y)
        {
            if (p1x == p2x && p1y == p2y)
            {
                //Zero rotation
                q = Quaternion.Identity;
                return;
            }

            //First, figure out z-coordinates for projection of P1 and P2 to
            //deformed sphere
            Vector3 p1 = new Vector3((float)p1x, (float)p1y, (float)tb_project_to_sphere(_trackballSize, p1x, p1y));
            Vector3 p2 = new Vector3((float)p2x, (float)p2y, (float)tb_project_to_sphere(_trackballSize, p2x, p2y));

            //Axis of rotation
            //Now, we want the cross product of P1 and P2
            Vector3 a = Vector3.Cross(p2, p1);
            a.Normalize();
            // Figure out how much to rotate around that axis.
            double t = (p2 - p1).Length / (2.0f * _trackballSize);

            //Avoid problems with out-of-control values...
            if (t > 1.0) t = 1.0;
            if (t < -1.0) t = -1.0;
            //how much to rotate about axis
            double phi = 2.0 * Math.Asin(t);

            q = new Quaternion(a, (float)(180.0 * phi / Math.PI));
        }

        private void trackball(out Vector3 axis, out double angle, double p1x, double p1y, double p2x, double p2y)
        {
            //Matrix4 rotation_matrix = Matrix4.CreateFromQuaternion(_rotation);
            //Vector3 v = new Vector3(0.0f, 1.0f, 0.0f);
            //Vector3 uv = rotation_matrix * new Vector3(0.0f, 1.0f, 0.0f);
            //Vector3 sv = rotation_matrix * new Vector3(1.0f, 0.0f, 0.0f);
            //Vector3 lv = rotation_matrix * new Vector3(0.0f, 0.0f, -1.0f);

            //Vector3 p1 = sv * p1x + uv * p1y - lv * tb_project_to_sphere(_trackballSize, p1x, p1y);
            //Vector3 p2 = sv * p2x + uv * p2y - lv * tb_project_to_sphere(_trackballSize, p2x, p2y);

            //First, figure out z-coordinates for projection of P1 and P2 to
            //deformed sphere
            Vector3 p1 = new Vector3((float)p1x, (float)p1y, (float)tb_project_to_sphere(_trackballSize, p1x, p1y));
            Vector3 p2 = new Vector3((float)p2x, (float)p2y, (float)tb_project_to_sphere(_trackballSize, p2x, p2y));

            //Axis of rotation
            //Now, we want the cross product of P1 and P2
            axis = Vector3.Cross(p2, p1);
            axis.Normalize();
            // Figure out how much to rotate around that axis.
            double t = (p2 - p1).Length / (2.0f * _trackballSize);

            //Avoid problems with out-of-control values...
            if (t > 1.0) t = 1.0;
            if (t < -1.0) t = -1.0;
            //how much to rotate about axis
            angle = 2.0 * Math.Asin(t);
        }

        private double tb_project_to_sphere(double r, double x, double y)
        {
            double z = 0;
            double z2 = 1 - x * x - y * y;
            double d = Math.Sqrt(x * x + y * y);
            if (d < r * 0.70710678118654752440d)
            {
                //Inside sphere
                z = Math.Sqrt(r * r - d * d);
            }
            else
            {
                //On hyperbola
                double t = r / 1.41421356237309504880d;
                z = t * t / d;
            }
            return z;
        }

        #endregion Rotate
    }

    /// <summary>
    /// Class that contains all data relevant to displaying in OpenGL
    /// </summary>
    public class DisplayData
    {
        public bool sketchMode = false;
        public double aspectRatio = 830.0 / 465.0, extrudeDepth = 0;
        public int colorZones = 14, colorScheme = 0;
        public double RotateX, RotateY, RotateZ;
        public double ScaleX = 0.4, ScaleY = 0.4, ScaleZ = 0.4;
        public int selected, undercutDirection = 0;
        public Color selectionColor = Color.Blue;
        public Color topColor = Color.DarkGray, bottomColor = Color.Black, materialColor = Color.GhostWhite;
        public double TranslateX = 0, TranslateY = 0, TranslateZ = 0;
        public bool wireframe = false, solid = true, showEdges = false, transparent = false, millimeter = false, ShowBoundingBox = false;
        public double XMin, YMin, ZMin, XMax, YMax, ZMax, remeshCutOff;
        public float maximumThickness;

        public enum sketch { Pencil, Polygon, Circle };

        public sketch SketchType;
    }
}