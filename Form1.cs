using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

namespace SandPile {
    public partial class Form1 : Form {
        private Stopwatch stopWatch = new Stopwatch();

        public Form1() {
            InitializeComponent();
            processorsToolLabel.Text = Environment.ProcessorCount.ToString();
            visualStylesComboBox.Items.Clear();
            visualStylesComboBox.Items.AddRange(Enum.GetNames(typeof(VisualStyle)));
            visualStylesComboBox.SelectedIndex = 2;

            this.taskTimer.Interval = 200;
            create();
        }

        private void create() {
            int width = Int32.Parse(widthTextBox.Text);
            int height = Int32.Parse(heightTextBox.Text);
            sandPileControl.create(width, height, checkBox1.Checked, checkBoxEnergyAware.Checked);
        }

        private void updateStatistics(Statistics stats) {
            _0TextCount.Text = stats.Counts[0].ToString();
            _1TextCount.Text = stats.Counts[1].ToString();
            _2TextCount.Text = stats.Counts[2].ToString();
            _3TextCount.Text = stats.Counts[3].ToString();
            _4TextCount.Text = stats.Counts[4].ToString();

            _1NTextCount.Text = stats.NCounts[1].ToString();
            _2NTextCount.Text = stats.NCounts[2].ToString();
            _3NTextCount.Text = stats.NCounts[3].ToString();
            _4NTextCount.Text = stats.NCounts[4].ToString();
        }

        private void OnCreate(object sender, EventArgs e) {
            create();
            sandPileControl.Invalidate();
        }

        private void OnFillClicked(object sender, EventArgs e) {
            if (!fillBackgroundWorker.IsBusy) {
                setButtonsEnabled(false);
                statusToolLabel.Text = "0%";
                matrixesProcessedStatusLabel.Text = "0";
                stopWatch.Restart();
                fillBackgroundWorker.RunWorkerAsync();
            }
        }

        private void panel4_Paint(object sender, PaintEventArgs e) {

        }

        private void OnDoWork(object sender, DoWorkEventArgs e) {
            int times = Int32.Parse(timesTextBox.Text);
            int procCount = Environment.ProcessorCount;
            SandPileMatrix[] matrixes = new SandPileMatrix[procCount];
            Statistics[] stats = new Statistics[procCount];
            
            for (int i = 0; i < procCount; ++i) {
                matrixes[i] = new SandPileMatrix();
                matrixes[i].create(sandPileControl.SandPileMatrix.Width, sandPileControl.SandPileMatrix.Height, sandPileControl.isEnergyAware);
                stats[i] = new Statistics();
            }

            int count = 0;
            int rangeSize = times / procCount;
            int additional = times - rangeSize * procCount;

            Parallel.For(0, procCount, i => {
                int start = rangeSize * i;
                int end = start + rangeSize;
                if (i == procCount - 1) {
                    end += additional;
                }
                //System.Diagnostics.Debug.WriteLine(start + " " + end);
                for (int j = start; j < end; ++j) {
                    matrixes[i].fill();
                    stats[i].add(matrixes[i].getStatistics());
                    Interlocked.Increment(ref count);
                    float percent = 100.0f * count / times;
                    fillBackgroundWorker.ReportProgress((int)percent, count);
                }
                    
            });

            Statistics finalStats = new Statistics();

            for (int i = 0; i < procCount; ++i) {
                finalStats.add(stats[i]);
            }

            finalStats.divide(count);
            e.Result = finalStats;
        }

        private void OnProgressChanged(object sender, ProgressChangedEventArgs e) {
            statusToolLabel.Text = e.ProgressPercentage + "%";
            matrixesProcessedStatusLabel.Text = e.UserState.ToString();
        }

        private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            updateStatistics((Statistics)e.Result);
            sandPileControl.SandPileMatrix.clear();
            sandPileControl.fill();
            sandPileControl.Invalidate();
            setButtonsEnabled(true);
            stopWatch.Stop();
            statusToolLabel.Text = stopWatch.ElapsedMilliseconds + " ms";
        }

        private void setButtonsEnabled(bool enabled) {
            createButton.Enabled = enabled;
            fillButton.Enabled = enabled;
            gossipFillButton.Enabled = enabled;
            gossipAnimateButton.Enabled = enabled;
        }

        private void OnBroadcastTimerTick(object sender, EventArgs e) {
            if (sandPileControl.isBroadcastFinished()) {
                SandPileMatrix matrix = sandPileControl.SandPileMatrix;
                double ratio = (double) matrix.BroadcastStepsCount / (matrix.Width);
                broadcastRatioTextView.Text = ratio.ToString();
                broadcastTimer.Stop();
                setButtonsEnabled(true);
                return;
            }
            sandPileControl.doBroadcastStep();
            sandPileControl.Invalidate();
        }

        private void OnBroadcastDoWork(object sender, DoWorkEventArgs e) {
            int times = Int32.Parse(broadcastTimesTextBox.Text);
            int procCount = Environment.ProcessorCount;
            SandPileMatrix[] matrixes = new SandPileMatrix[procCount];
            int[] steps = new int[procCount];

            for (int i = 0; i < procCount; ++i) {
                matrixes[i] = new SandPileMatrix();
                matrixes[i].create(sandPileControl.SandPileMatrix.Width, sandPileControl.SandPileMatrix.Height, checkBoxEnergyAware.Checked);
                steps[i] = 0;
            }

            int count = 0;
            int rangeSize = times / procCount;
            int additional = times - rangeSize * procCount;

            Parallel.For(0, procCount, i => {
                int start = rangeSize * i;
                int end = start + rangeSize;
                if (i == procCount - 1) {
                    end += additional;
                }
                //System.Diagnostics.Debug.WriteLine(start + " " + end);
                for (int j = start; j < end; ++j) {
                    matrixes[i].prepareBroadcast();
                    while (!matrixes[i].isBroadcastFinished()) {
                        matrixes[i].doBroadcastStep();
                    }
                    steps[i] += matrixes[i].BroadcastStepsCount;
                    Interlocked.Increment(ref count);
                    float percent = 100.0f * count / times;
                    broadcastBackgroundWorker.ReportProgress((int)percent, count);
                }

            });

            int finalCount = 0;
            float ratio;

            for (int i = 0; i < procCount; ++i) {
                finalCount += steps[i];
            }

            ratio = (float) finalCount / (count * sandPileControl.SandPileMatrix.Width);
            e.Result = ratio;
        }

        private void OnBroadcastProgressChanged(object sender, ProgressChangedEventArgs e) {
            statusToolLabel.Text = e.ProgressPercentage + "%";
            matrixesProcessedStatusLabel.Text = e.UserState.ToString();
        }

        private void OnBroadcastRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            broadcastRatioTextView.Text = e.Result.ToString();
            sandPileControl.Invalidate();
            setButtonsEnabled(true);
            stopWatch.Stop();
            statusToolLabel.Text = stopWatch.ElapsedMilliseconds + " ms";
        }

        private void OnBroadcastFillClicked(object sender, EventArgs e) {
            if (!broadcastBackgroundWorker.IsBusy) {
                setButtonsEnabled(false);
                statusToolLabel.Text = "0%";
                matrixesProcessedStatusLabel.Text = "0";
                stopWatch.Restart();
                broadcastBackgroundWorker.RunWorkerAsync();
            }
        }

        private void OnBroadcastAnimateButtonClicked(object sender, EventArgs e) {
            setButtonsEnabled(false);
            sandPileControl.prepareBroadcast();
            sandPileControl.Invalidate();
            broadcastTimer.Start();
        }

        private void OnVisualStyleSelectionChanged(object sender, EventArgs e) {
            SettingsManager.Instance.VisualStyle = (VisualStyle)visualStylesComboBox.SelectedIndex;
            sandPileControl.Invalidate();
        }

        private void OnTaskTimerTick(object sender, EventArgs e) {
            Console.WriteLine("on timertick " + DateTime.Now.ToString("HH:mm:ss tt"));
            if (this.checkBoxEnergyAware.Checked) {
                updateNodesStatus();
            }

            sandPileControl.SandPileMatrix.doTasksStep();
            sandPileControl.Invalidate();
        }

        private void OnTaskStopClick(object sender, EventArgs e) {
            if (taskTimer.Enabled) {
                setButtonsEnabled(true);
                taskTimer.Stop();
            }
        }

        private void OnTaskStartClick(object sender, EventArgs e) {
            if (!taskTimer.Enabled) {
                setButtonsEnabled(false);
                sandPileControl.SandPileMatrix.prepareTasks();
                taskTimer.Start();
            }
        }

        private void OnSandPileMouseDown(object sender, MouseEventArgs e)
        {
            if (taskTimer.Enabled)
            {
                int x = e.X - AutoScrollPosition.X;
                int y = e.Y - AutoScrollPosition.Y;
                int i = y / (sandPileControl.NodeSize + sandPileControl.NodeSpacing);
                int j = x / (sandPileControl.NodeSize + sandPileControl.NodeSpacing);
                SandPileMatrix matrix = sandPileControl.SandPileMatrix;

                if (i < 0 || j < 0 || i >= matrix.Height || j >= matrix.Width)
                {
                    return;
                }

                if (e.Button == System.Windows.Forms.MouseButtons.Left)
                {
                    int time = Int32.Parse(taskTimeTextBox.Text);
                    int count = Int32.Parse(taskCountTextBox.Text);

                    for (int k = 0; k < count; ++k)
                    {
                        if (matrix.Nodes[i][j].isEnabled) {
                            matrix.Nodes[i][j].isBusy = false;
                            matrix.Nodes[i][j].addTask(time);
                        }
                    }

                    if (checkBoxEnergyAware.Checked) {
                        updateNodesStatus();
                    }

                    sandPileControl.Invalidate();
                }
            }
        }

        private void updateNodesStatus()
        {
            SandPileMatrix sandPileMatrix = sandPileControl.SandPileMatrix;
            int activeTaskCount = sandPileMatrix.getActiveTaskCount();
            int neededNodeCount = (int)Math.Ceiling((double)activeTaskCount / (SandPileNode.TasksCount - SandPileMatrix.SN)); 

            SandPileNode[][] nodes = sandPileMatrix.Nodes;
            for (int i = 0; i < nodes.Length; ++i)
            {
                for (int j = 0; j < nodes[i].Length; ++j)
                {
                    bool isNodeBusy = true;
                    for (int taskIdx = SandPileMatrix.SN; taskIdx < nodes[i][j].Tasks.Length; taskIdx++)
                    {
                        if (nodes[i][j].Tasks[taskIdx] > 0)
                        {
                            isNodeBusy = false;
                            break;
                        }
                    }
                    nodes[i][j].isBusy = isNodeBusy;

                    if (i * nodes.Length + j < neededNodeCount) {
                        nodes[i][j].isBusy = false;
                    }
                }
            }

            /*
            for (int i = 0; i < nodes.Length; ++i)
            {
                for (int j = 0; j < nodes[i].Length; ++j)
                {
                    if (!nodes[i][j].isBusy) {
                        nodes[i][j].setBoost();
                    }               
                }
            }
             * */
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
                sandPileControl.setCheckedStatus(checkBox1.Checked);
        }

        private void checkBoxEnergyAware_CheckStateChanged(object sender, EventArgs e)
        {
            sandPileControl.isEnergyAware = checkBoxEnergyAware.Checked;
        }

        
    }
}