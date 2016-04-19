using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandPile {
    public class SandPileNode :IComparable {
        public static readonly int TasksCount = 7;
        private int mCount;
        private bool mHasInfo;
        private bool mIsEnabled;
        private int[] tasks = new int[TasksCount];
        private static Random randomGen = new Random();
        private int boost = 0;

        public SandPileNode() {
            isEnabled = true;
            isBusy = false;
        }

        public void clear() {
            mCount = 0;
            mHasInfo = false;
        }

        public void addTask(int time) {
            int startIdx = 0;
            if (!isBusy)
            {
                startIdx = TasksCount - 1;
            }
            else {
                startIdx = TasksCount - 4;
            }
            for (int i = startIdx; i >= 0; i--)
            {
                if (tasks[i] == 0)
                {
                    tasks[i] = time;
                    break;
                }
            }
        }

        public int popTask() {
            for (int i = 0; i < 4; ++i) {
                int time = tasks[i];
                if (time > 0) {
                    tasks[i] = 0;
                    return time;
                }
            }
            return 0;
        }

        public void decreaseTasks() {
            for (int i = TasksCount - 3; i < TasksCount; ++i) {
                if (tasks[i] > 0) {
                    tasks[i]--;
                }
            }
        }

        public bool HasFreeTask {
            get { return tasks[0] == 0; }
        }

        public int Count {
            get { return mCount; }
            set { mCount = value; }
        }

        public bool HasInfo {
            get { return mHasInfo; }
            set { mHasInfo = value; }
        }

        public int[] Tasks {
            get { return tasks; }
        }

        public bool isEnabled {
            get { return mIsEnabled; }
            set { mIsEnabled = value;}
        }

        public bool isBusy
        {
            get;
            set;
        }

        public int getBoost()
        {
            //if (this.boost == 0) {
                if (randomGen.NextDouble() < 0.9)
                {
                    this.boost = 3;
                }
                else {
                    this.boost = randomGen.Next(1, 5);
                }
            //}
           
            return this.boost;
        }

        public override string ToString()
        {
            return "boost = " + this.boost ;
        }

        public int CompareTo(object obj) {
            if (obj == null) return 1;

            SandPileNode sandPileNode = obj as SandPileNode;
            if (sandPileNode != null)
            {
                return this.boost.CompareTo(sandPileNode.boost);
            }
            else {
                throw new ArgumentException("Object is not an SandPileNode");
            }
        }

        internal void setBoost(int boost)
        {
            this.boost = boost;
        }
    }
}
