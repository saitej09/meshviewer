using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MeshGeometry;

namespace SU2_Viewer
{
    /// <summary>
    /// The main GUI for the application
    /// </summary>
    public partial class Form1 : Form
    {
        private SU2Mesh currentMesh;
        private bool mDisplayListCreated = true;
        private int mDisplayListID;

        /// <summary>
        /// Form Initialising Method
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            glControl.Paint += glControl_Paint;
        }

        /// <summary>
        /// Exit Button OnCLick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Open Button OnClick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        /// <summary>
        /// Actions to be taken after open button is pressed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            currentMesh = new SU2Mesh(openFileDialog1.FileName, SU2Mesh.FILETYPE.SU2);
            mDisplayData.XMax = currentMesh.Bound1.X;
            mDisplayData.XMin = currentMesh.Bound0.X;
            mDisplayData.YMax = currentMesh.Bound1.Y;
            mDisplayData.YMin = currentMesh.Bound0.Y;
            mDisplayData.ZMax = currentMesh.Bound1.Z;
            mDisplayData.ZMin = currentMesh.Bound0.Z;
            mDisplayData.ScaleX = 0.5;
            mDisplayData.ScaleY = 0.5;
            mDisplayData.ScaleZ = 0.5;
            mDisplayData.RotateX = 45;
            mDisplayData.RotateY = 45;
            mDisplayData.RotateZ = 0;
            mDisplayData.TranslateX = 0;
            mDisplayData.TranslateY = 0;
            mDisplayData.TranslateZ = 0;
            mDisplayListCreated = false;
            glControl.Invalidate();
        }

        /// <summary>
        /// Close Button OnClick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentMesh = null;
            mDisplayListCreated = false;
            glControl.Invalidate();
        }

        /// <summary>
        /// Reload button OnClick event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentMesh = new SU2Mesh(openFileDialog1.FileName, SU2Mesh.FILETYPE.SU2);
            mDisplayData.XMax = currentMesh.Bound1.X;
            mDisplayData.XMin = currentMesh.Bound0.X;
            mDisplayData.YMax = currentMesh.Bound1.Y;
            mDisplayData.YMin = currentMesh.Bound0.Y;
            mDisplayData.ZMax = currentMesh.Bound1.Z;
            mDisplayData.ZMin = currentMesh.Bound0.Z;
            mDisplayData.ScaleX = 0.5;
            mDisplayData.ScaleY = 0.5;
            mDisplayData.ScaleZ = 0.5;
            mDisplayData.RotateX = 45;
            mDisplayData.RotateY = 45;
            mDisplayData.RotateZ = 0;
            mDisplayData.TranslateX = 0;
            mDisplayData.TranslateY = 0;
            mDisplayData.TranslateZ = 0;
            mDisplayListCreated = false;
            glControl.Invalidate();
        }
    }
}