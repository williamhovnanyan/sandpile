using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandPile {
    public class SandPileMatrix {
        public static readonly int SN = 4;
        private Random mRnd = new Random();
        private SandPileNode[][] mNodes = new SandPileNode[0][];
        private List<int> mIndexes = new List<int>();
        private int mBroadcastStepsCount;
        private int mBroadcastHoldersCount;
        private bool isEnergyAware;

        public SandPileMatrix() {

        }

        public void create(int width, int height, bool isEnergyAware) {
            this.isEnergyAware = isEnergyAware;
            mNodes = new SandPileNode[height][];
            for (int i = 0; i < mNodes.Length; ++i) {
                mNodes[i] = new SandPileNode[width];
                for (int j = 0; j < mNodes[i].Length; ++j) {
                    mNodes[i][j] = new SandPileNode();
                }
            }

            mIndexes = new List<int>(Width * Height);
            for (int i = 0; i < Width * Height; ++i) {
                mIndexes.Add(i);
            }
            mBroadcastStepsCount = 0;
            mBroadcastHoldersCount = 0;
        }

        public void clear() {
            for (int i = 0; i < mNodes.Length; ++i) {
                for (int j = 0; j < mNodes[i].Length; ++j) {
                    mNodes[i][j].clear();
                }
            }
            mBroadcastStepsCount = 0;
            mBroadcastHoldersCount = 0;
        }

        public SandPileNode getLeft(int i, int j) {
            SandPileNode returnNode = null;
            if (j == 0) {
                returnNode = mNodes[i][mNodes[i].Length - 1];//mNodes[i][j + 1];//
            } else {
                returnNode = mNodes[i][j - 1];
            }

//            if (!returnNode.isEnabled) {
//                returnNode = null;
//            }
            return returnNode;
        }

        public SandPileNode getRight(int i, int j) {
            SandPileNode returnNode = null;
            if (j == mNodes[i].Length - 1) {
                returnNode = mNodes[i][0];//mNodes[i][j - 1];//
            } else {
                returnNode = mNodes[i][j + 1];
            }
//            if (!returnNode.isEnabled) {
//                returnNode = null;
//            }
            return returnNode;
        }

        public SandPileNode getUp(int i, int j) {
            SandPileNode returnNode = null;
            if (i == 0) {
                returnNode = mNodes[mNodes.Length - 1][j];//mNodes[i + 1][j];
            } else {
                returnNode = mNodes[i - 1][j]; 
            }
//            if (!returnNode.isEnabled) {
//                returnNode = null;
//            }
            return returnNode;
        }

        public SandPileNode getDown(int i, int j) {
            SandPileNode returnNode = null;
            if (i == mNodes.Length - 1) {
                returnNode = mNodes[0][j];//mNodes[i - 1][j];//
            } else {
                returnNode = mNodes[i + 1][j];
            }

//            if (!returnNode.isEnabled) {
//                returnNode = null;
//            }
            return returnNode;
        }

        public IEnumerable<SandPileNode> getNeighbours(int i, int j) {
            SandPileNode node = getLeft(i, j);
            if (node != null) {
                yield return node;
            }

            node = getRight(i, j);
            if (node != null) {
                yield return node;
            }

            if (Height > 1) {
                node = getUp(i, j);
                if (node != null) {
                    yield return node;
                }

                node = getDown(i, j);
                if (node != null) {
                    yield return node;
                }
            }
        }

        public Tuple<int, int> getLeftTuple(int i, int j) {
            SandPileNode neighbour = getLeft(i, j);
           
            if (neighbour != null) {
                if (j == 0) {
                    return Tuple.Create(i, mNodes[i].Length - 1);
                }
                return Tuple.Create(i, j - 1);
            }
            return null;
        }

        public Tuple<int, int> getRightTuple(int i, int j) {
            SandPileNode neighbour = getRight(i, j);

            if (neighbour != null) {
                if (j == mNodes[i].Length - 1) {
                    return Tuple.Create(i, 0);    
                }
                return Tuple.Create(i, j + 1);
            }
            return null;
        }

        public Tuple<int, int> getUpTuple(int i, int j) {
            SandPileNode neighbour = getUp(i, j);

            if (neighbour != null) {
                if (i == 0) {
                    return Tuple.Create(mNodes.Length - 1, j);
                }

                return Tuple.Create(i - 1, j);
            }
            return null;
        }

        public Tuple<int, int> getDownTuple(int i, int j) {
            SandPileNode neighbour = getDown(i, j);

            if (neighbour != null) {
                if (i == mNodes.Length - 1) {
                    return Tuple.Create(0, j);
                }
                return Tuple.Create(i + 1, j);
            }
            return null;
        }

        public IEnumerable<Tuple<int, int>> getNeighboursTuples(int i, int j) {
            Tuple<int, int> neighbourTuple = getLeftTuple(i, j);

            if (neighbourTuple != null) {
                yield return getLeftTuple(i, j);
            }

            neighbourTuple = getRightTuple(i, j);
            if (neighbourTuple != null)
            {
                yield return getRightTuple(i, j);
            }

            neighbourTuple = getUpTuple(i, j);
            if (neighbourTuple != null) {
                yield return getUpTuple(i, j);
            }

            neighbourTuple = getDownTuple(i, j);
            if (neighbourTuple != null) {
                yield return getDownTuple(i, j);
            }
        }

        public int getNeighboursSum(int i, int j) {
            int sum = 0;
            foreach(SandPileNode node in getNeighbours(i, j)) {
                sum += node.Count;
            }
            return sum;
        }

        public int getSNCount(int i, int j) {
            int sum = 0;
            foreach (SandPileNode node in getNeighbours(i, j)) {
                if (node.Count == SN) {
                    ++sum;
                }
            }
            return sum;
        }

        public void shuffle(List<int> list) {
            int n = list.Count;
            while (n > 1) {
                n--;
                int k = mRnd.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public void shuffleNodes() {
            int n = Width * Height;
            while (n > 1) {
                n--;
                int k = mRnd.Next(n + 1);
                int i1 = mIndexes[n] / Width;
                int j1 = mIndexes[n] % Width;
                int i2 = mIndexes[k] / Width;
                int j2 = mIndexes[k] % Width;
                SandPileNode value = mNodes[i2][j2];
                mNodes[i2][j2] = mNodes[i1][j1];
                mNodes[i1][j1] = value;
            }
        }

        public SandPileNode getFirstFullNode() {
            for (int i = 0; i < mNodes.Length; ++i) {
                for (int j = 0; j < mNodes[i].Length; ++j) {
                    if (mNodes[i][j].Count == SN && mNodes[i][j].isEnabled) {
                        return mNodes[i][j];
                    }
                }
            }
            return null;
        }

        public void prepareBroadcast() {
            clear();
            shuffle(mIndexes);
            int nodeIndex = mIndexes[0];
            int i = mIndexes[nodeIndex] / Width;
            int j = mIndexes[nodeIndex] % Width;
            SandPileNode centerNode = mNodes[i][j];
            centerNode.HasInfo = true;
            mBroadcastHoldersCount = 1;
        }

        public void doBroadcastStep() {
            bool changed = false;
            for (int index = 0; index < mIndexes.Count; ++index) {
                int nodeIndex = mIndexes[index];
                int i = mIndexes[nodeIndex] / Width;
                int j = mIndexes[nodeIndex] % Width;
                SandPileNode centerNode = mNodes[i][j];

                if (centerNode.HasInfo) {
                    foreach (SandPileNode node in getNeighbours(i, j)) {
                        if (!node.HasInfo) {
                            mBroadcastHoldersCount++;
                            node.HasInfo = true;
                            changed = true;
                        }
                    }
                }
            }
            if (changed) {
                ++mBroadcastStepsCount;
            }
        }

        /*
        public void prepareBroadcast() {
            clear();
            fill();
            getFirstFullNode().HasInfo = true;
            mBroadcastHoldersCount = 1;
        }

        public void doBroadcastStep() {
            for (int i = 0; i < mNodes.Length; ++i) {
                for (int j = 0; j < mNodes[i].Length; ++j) {
                    SandPileNode currentNode = mNodes[i][j];
                    if (currentNode.Count == SN) {
                        currentNode.Count = 0;
                        foreach (SandPileNode node in getNeighbours(i, j)) {
                            node.Count++;
                            if (currentNode.HasInfo) {
                                if (!node.HasInfo) {
                                    node.HasInfo = true;
                                    ++mBroadcastHoldersCount;
                                }
                            }
                        }
                    }
                }
            }
            ++mBroadcastStepsCount;
        }
        */

        public void prepareTasks() {
            clear();
            fill();
        }


        private static Random randomGen = new Random(DateTime.Now.Millisecond);
        public void doTasksStep() {
            for (int i = 0; i < mNodes.Length; ++i) {
                for (int j = 0; j < mNodes[i].Length; ++j) {
                    SandPileNode currentNode = mNodes[i][j];
                    //Console.WriteLine("node = " + currentNode + ", state = " + currentNode.isBusy);
                    //if (currentNode.Count == SN && currentNode.isEnabled) {
                    if (currentNode.Count == SN) {
                        currentNode.Count = currentNode.Count - SN;// getNumOfNeighbours(i, j);
                        //Console.WriteLine("[" + i + ", " + j + "] = " + getNumOfNeighbours(i, j));
                        IEnumerable<SandPileNode> neighbours = getNeighbours(i, j);
                        //IEnumerable<Tuple<int, int>> neighboursIndices = getNeighboursTuples(i, j);
                        int direction = 0;
                        if (isBorderTuple(Tuple.Create<int, int>(i, j))) {
                            direction = getRightDirection(Tuple.Create<int, int>(i, j));
                        }

                        if (direction == 0)
                        {
                            int counter = 0;
                            double nextRandomDouble = randomGen.NextDouble();
                            neighbours = neighbours.OrderBy(el =>
                            {
                                counter++;
                                if (counter > 4)
                                {
                                    counter = 1;
                                }
                                int returnValue = 1;
                                switch (counter)
                                {
                                    case 1: returnValue = (nextRandomDouble >= 0.4 && nextRandomDouble < 0.9) ? 1000 : 1;
                                        break;
                                    case 2: returnValue = (nextRandomDouble > 0.9) ? 1000 : 1;
                                        break;
                                    case 3: returnValue = (nextRandomDouble < 0.4) ? 1000 : 1;
                                        break;
                                    case 4: returnValue = (nextRandomDouble > 0.9) ? 1000 : 1;
                                        break;
                                    default:
                                        throw new ArgumentException("Error");
                                }

                                return -returnValue;
                            });

                        }
                        else {
                            int counter = 0;
                            neighbours = neighbours.OrderBy(el => {
                                counter++;
                                if (counter > 4)
                                {
                                    counter = 1;
                                }
                                int returnValue = 1;
                                if (counter == direction)
                                {
                                    returnValue = 1000;
                                }
                                return -returnValue;
                            });
                        }
                        //neighbours = neighbours.OrderBy(el => el.getBoost());
                        //Console.WriteLine("ordered list is " + neighbours.Max().ToString());
                        foreach (SandPileNode node in neighbours) {
                            //if (!node.isBusy)
                            {
                                node.Count++;
                                if (node.HasFreeTask && node.isEnabled)
                                {
                                    node.addTask(currentNode.popTask());
                                }
                                //Array.Sort(node.Tasks);
                            }
                        }

                    }

                    if (!currentNode.isBusy)
                    {
                        Array.Sort(currentNode.Tasks);
                        //PartialSort(currentNode.Tasks, SN, SandPileNode.TasksCount);
                    }
                }
            }

            for (int i = 0; i < mNodes.Length; ++i) {
                for (int j = 0; j < mNodes[i].Length; ++j) {
                    SandPileNode currentNode = mNodes[i][j];
                    if (!currentNode.isBusy)
                    {
                        currentNode.decreaseTasks();
                        Array.Sort(currentNode.Tasks);
                    }
                }
            }
        }

        private int getRightDirection(Tuple<int, int> tuple)
        {
            if (tuple.Item1 == 0 && tuple.Item2 != 0 && tuple.Item2 != mNodes.Length) {
                return randomGen.Next() % 3 == 0 ? 4 : randomGen.Next() % 2 == 0 ? 1 : 2;
            }
            else if (tuple.Item1 == 0) {
                return 4;
            }
            
            if (tuple.Item1 == mNodes.Length && tuple.Item2 != 0 && tuple.Item2 != mNodes.Length) {
                return randomGen.Next() % 3 == 0 ? 3 : randomGen.Next() % 2 == 0 ? 1 : 2;
            } else if(tuple.Item1 == mNodes.Length) {
                return 3;
            }

            if (tuple.Item2 == 0 && tuple.Item1 != 0 && tuple.Item1 != mNodes.Length) {
                return randomGen.Next() % 3 == 0 ? 2 : randomGen.Next() % 2 == 0 ? 3 : 4;
            }
            else if (tuple.Item2 == 0) {
                return 2;
            }

            if (tuple.Item2 == mNodes.Length && tuple.Item1 != 0 && tuple.Item1 != mNodes.Length)
            {
                return randomGen.Next() % 3 == 0 ? 1 : randomGen.Next() % 2 == 0 ? 3 : 4;
            }
            else if (tuple.Item2 == mNodes.Length) {
                return 1;
            }
            return 0;
        }

        private bool isBorderTuple(Tuple<int, int> tuple)
        {
            return tuple.Item1 == 0 || tuple.Item1 == mNodes.Length
                || tuple.Item2 == 0 || tuple.Item2 == mNodes.Length;
        } 
        
        public int getActiveTaskCount() {
            int count = 0;
            for (int i = 0; i < mNodes.Length; ++i)
            {
                for (int j = 0; j < mNodes[i].Length; ++j)
                {
                    if (mNodes[i][j].isEnabled) {
                        for (int taskIdx = 0; taskIdx < SandPileMatrix.SN; taskIdx++)
                        {
                            if(mNodes[i][j].Tasks[taskIdx] != 0)
                                count += 1;
                        }
                    }
                }
            }
            return count;
        }

        private int getNumOfNeighbours(int i, int j)
        {
            IEnumerator<SandPileNode> ienum = getNeighbours(i, j).GetEnumerator();

            int count = 0;
            while (ienum.MoveNext())
            {
                SandPileNode node = ienum.Current;
                if (node.isEnabled)
                {
                    count++;
                }
            }
            return count;
            
            
        }

        internal bool isBroadcastFinished() {
            return mBroadcastHoldersCount == Width * Height;
        }

        public void fill() {
            shuffle(mIndexes);
            clear();
            for (int k = 0; k < mIndexes.Count; ++k) {
                int i = mIndexes[k] / Width;
                int j = mIndexes[k] % Width;
                mNodes[i][j].Count = 0;
                foreach (SandPileNode node in getNeighbours(i, j)) {
                    node.Count++;
                }
            }
        }

        public Statistics getStatistics() {
            Statistics stats = new Statistics();
            int[] counts = new int[5];
            int[] nCounts = new int[5];
            float nodesCount = Width * Height;

            for (int i = 0; i < mNodes.Length; ++i) {
                for (int j = 0; j < mNodes[i].Length; ++j) {
                    counts[mNodes[i][j].Count]++;
                    if (mNodes[i][j].Count == 0) {
                        int count = getSNCount(i, j);
                        if (count > 0) {
                            nCounts[count]++;
                        }
                    }
                }
            }

            for (int i = 0; i < counts.Length; ++i) {
                stats.Counts[i] = counts[i] / nodesCount;
                stats.NCounts[i] = nCounts[i] / nodesCount;
            }

            return stats;
        }

        public int Width {
            get { return mNodes[0].Length; }
        }

        public int Height {
            get { return mNodes.Length; }
        }

        public SandPileNode[][] Nodes {
            get { return mNodes; }
        }

        public int BroadcastStepsCount {
            get { return mBroadcastStepsCount; }
        }
    }
}
