using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SandPile {
    public class SettingsManager {
        private static readonly SettingsManager instance = new SettingsManager();

        private VisualStyle currentVisualStyle = VisualStyle.LargeCircles;

        private SettingsManager() {

        }

        public static SettingsManager Instance {
            get { return instance; }
        }

        public VisualStyle VisualStyle {
            get { return currentVisualStyle; }
            set { currentVisualStyle = value; }
        }
    }
}
