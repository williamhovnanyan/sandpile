using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandPile {
    public class SandPileNode {
        public static readonly int TasksCount = 7;
        private int mCount;
        private bool mHasInfo;
        private bool mIsEnabled;
        private int[] tasks = new int[TasksCount];

        public SandPileNode() {
            isEnabled = true;
            isBusy = false;
        }

        public void clear() {
            mCount = 0;
            mHasInfo = false;
        }

        public void addTask(int time) {
            if (!isBusy)
            {
                for (int i = TasksCount - 1; i >= 0; i--)
                {
                    if (tasks[i] == 0)
                    {
                        tasks[i] = time;
                        break;
                    }
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
            for (int i = TasksCount - 4; i < TasksCount; ++i) {
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
    }
}
