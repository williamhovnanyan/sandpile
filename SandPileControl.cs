using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SandPile {
    public partial class SandPileControl : ScrollableControl {
        private SandPileMatrix mMatrix = new SandPileMatrix();
        private int mNodeSize;
        private int mNodeSpacing;
        private StringFormat mStringFormat = new StringFormat();
        private Font taskFont;
        private bool isDebugMode;
        private bool energyAware;

        public SandPileControl() {
            InitializeComponent();
            mStringFormat.LineAlignment = StringAlignment.Center;
            mStringFormat.Alignment = StringAlignment.Center;
            DoubleBuffered = true;
            ResizeRedraw = true;
            AutoScrollMinSize = new Size(0, 0);
            FontFamily fontFamily = new FontFamily("Arial");
            taskFont = new Font(
               fontFamily,
               7,
               FontStyle.Regular,
               GraphicsUnit.Pixel);
        }

        protected override void OnPaint(PaintEventArgs pe) {
            Graphics g = pe.Graphics;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bicubic;
            g.TranslateTransform(AutoScrollPosition.X, AutoScrollPosition.Y);
            SandPileNode[][] nodes = mMatrix.Nodes;
            VisualStyle visualStyle = SettingsManager.Instance.VisualStyle;
            Brush fillBrush;

            switch (visualStyle) {
                case VisualStyle.LargeCircles:
                    mNodeSize = 20;
                    mNodeSpacing = 10;
                    break;

                case VisualStyle.SmallRectangles:
                    mNodeSize = 4;
                    mNodeSpacing = 0;
                    break;

                case VisualStyle.LargeRectangles:
                    mNodeSize = SandPileNode.TasksCount * 9;
                    mNodeSpacing = 0;
                    break;
            }

            //Draw Nodes
            for (int i = 0; i < nodes.Length; ++i) {
                for (int j = 0; j < nodes[i].Length; ++j) {
                    int x = j * mNodeSize + j * mNodeSpacing;
                    int y = i * mNodeSize + i * mNodeSpacing;
                    if (x + AutoScrollPosition.X >= -mNodeSize && y + AutoScrollPosition.Y >= -mNodeSize
                        && x + AutoScrollPosition.X < Width && y + AutoScrollPosition.Y < Height) {
                        if (nodes[i][j].Count == SandPileMatrix.SN) {
                            fillBrush = Brushes.Black;
                        }
                        else {
                            fillBrush = Brushes.Blue;
                        }
                        if (nodes[i][j].HasInfo) {
                            switch (visualStyle) {
                                case VisualStyle.LargeCircles:
                                    g.FillEllipse(fillBrush, x, y, mNodeSize, mNodeSize);
                                    break;

                                case VisualStyle.SmallRectangles:
                                case VisualStyle.LargeRectangles:
                                    if(isDebugMode)
                                        g.FillRectangle(fillBrush, x, y + (SandPileMatrix.SN - nodes[i][j].Count) * 9, mNodeSize, nodes[i][j].Count * 9);
                                    break;
                            }
                        }

                        switch (visualStyle) {
                            case VisualStyle.LargeCircles:
                                g.DrawEllipse(Pens.Gray, x, y, mNodeSize, mNodeSize);
                                g.DrawString(nodes[i][j].Count.ToString(), DefaultFont, Brushes.Gray,
                                    x + mNodeSize / 2, y + mNodeSize / 2, mStringFormat);
                                break;

                            case VisualStyle.SmallRectangles:
                                break;

                            case VisualStyle.LargeRectangles:
                                //if (nodes[i][j].Count == SandPileMatrix.SN) {
                                if(nodes[i][j].isEnabled && isDebugMode) {
                                    g.FillRectangle(fillBrush, x, y + (SandPileMatrix.SN - nodes[i][j].Count) * 9, mNodeSize, nodes[i][j].Count * 9);
                                }
                                int[] tasks = nodes[i][j].Tasks;
                                bool isEnabled = nodes[i][j].isEnabled;
                                float h = mNodeSize / SandPileNode.TasksCount;
                                float w = mNodeSize / 2;
                                if (isEnabled)
                                {
                                    for (int k = 0; k < SandPileNode.TasksCount; ++k)
                                    {
                                        float kx = x + mNodeSize / 2 - w / 2;
                                        float ky = y + k * h;
                                        Brush taskBrush;

                                        if (tasks[k] == 0)
                                        {
                                            taskBrush = Brushes.White;
                                        }
                                        else
                                        {
                                            if (k <= SandPileNode.TasksCount - SandPileMatrix.SN)
                                            {
                                                taskBrush = Brushes.Red;
                                            }
                                            else
                                            {
                                                taskBrush = Brushes.Green;
                                            }
                                        }
                                        if (k >= SandPileMatrix.SN && isDebugMode) {
                                            g.DrawString(nodes[i][j].Tasks[k].ToString(), taskFont, Brushes.Black, new PointF(kx - w / 2 + w / 6, ky + 1));
                                        }
                                        g.DrawRectangle(Pens.DarkGray, kx, ky, w, h);
                                        g.FillRectangle(taskBrush, kx + 1, ky + 1, w - 2, h - 2);
                                    }
                                }
                                /*
                                float s = 0.85f * mNodeSize / SandPileNode.TasksCount;
                                for (int k = 0; k < SandPileNode.TasksCount; ++k) {
                                    if (tasks[k] > 0) {
                                        g.DrawString(tasks[k].ToString(), taskFont, Brushes.Gray,
                                        x + k * s, y + k * s);
                                    }
                                }
                                */
                                break;
                        }
                    }
                }
            }

            //Draw Grid
            if (!DesignMode && 
                (visualStyle == VisualStyle.SmallRectangles || visualStyle == VisualStyle.LargeRectangles)) {
                for (int i = 0; i <= nodes.Length; ++i) {
                    int y = i * mNodeSize;
                    g.DrawLine(Pens.Gray, 0, y, mMatrix.Width * mNodeSize, y);
                }

                for (int j = 0; j <= nodes[0].Length; ++j) {
                    int x = j * mNodeSize;
                    g.DrawLine(Pens.Gray, x, 0, x, mMatrix.Height * mNodeSize);
                }
            }
        }

        public void create(int width, int height, bool isDebugMode, bool isEnergyAware) {
            mMatrix.create(width, height);
            this.isDebugMode = isDebugMode;
            this.isEnergyAware = isEnergyAware;
            AutoScrollMinSize = new Size(width * mNodeSize + (width - 1) * mNodeSpacing,
                height * mNodeSize + (height - 1) * mNodeSpacing);
        }

        public void fill() {
            mMatrix.fill();
        }

        public void prepareBroadcast() {
            mMatrix.prepareBroadcast();
        }

        public void doBroadcastStep() {
            mMatrix.doBroadcastStep();
        }

        public bool isBroadcastFinished() {
            return mMatrix.isBroadcastFinished();
        }

        public Statistics getStatistics() {
            return mMatrix.getStatistics();
        }

        private void OnScroll(object sender, ScrollEventArgs e) {
            Invalidate();
        }

        public SandPileMatrix SandPileMatrix {
            get { return mMatrix; }
        }

        public int NodeSize {
            get { return mNodeSize; }
        }

        public int NodeSpacing {
            get { return mNodeSpacing; }
        }

        private void OnMouseDown(object sender, MouseEventArgs e) {
            
            int x = e.X - AutoScrollPosition.X;
            int y = e.Y - AutoScrollPosition.Y;
            int i = y / (mNodeSize + mNodeSpacing);
            int j = x / (mNodeSize + mNodeSpacing);

            if (i < 0 || j < 0 || i >= mMatrix.Height || j >= mMatrix.Width) {
                return;
            }

            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                mMatrix.Nodes[i][j].isEnabled = !mMatrix.Nodes[i][j].isEnabled;   
                Invalidate();
                //MessageBox.Show(i.ToString() + " " + j.ToString());
            }
        }

        internal void setCheckedStatus(bool p)
        {
            this.isDebugMode = p;
        }

        public bool isEnergyAware { get; set; }
    }
}
