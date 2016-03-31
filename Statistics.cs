using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandPile {
    public class Statistics {
        public static readonly int Length = 5;
        private readonly double[] counts = new double[Length];
        private readonly double[] nCounts = new double[Length];

        public Statistics() {

        }

        public void add(Statistics s) {
            for (int i = 0; i < Length; ++i) {
                counts[i] += s.counts[i];
                nCounts[i] += s.nCounts[i];
            }
        }

        public void divide(double value) {
            for (int i = 0; i < Length; ++i) {
                counts[i] /= value;
                nCounts[i] /= value;
            }
        }

        public double[] Counts {
            get { return counts; }
        }

        public double[] NCounts {
            get { return nCounts; }
        }
    }
}
