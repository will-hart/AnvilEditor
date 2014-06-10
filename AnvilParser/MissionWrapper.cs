using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AnvilParser.Tokens;

namespace AnvilParser
{
    class MissionWrapper
    {

        private MissionBase mission = new MissionBase("mission");
        private MissionBase intro = new MissionBase("intro");
        private MissionBase outroLose = new MissionBase("outroLoose");
        private MissionBase outroWin = new MissionBase("outroWin");

        public MissionWrapper() {}
    }
}
