using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PlaymoreClient.Gui
{
    interface PlaymoreController
    {
        bool logIn(string userName, string password);
        void changeRegion(LeagueRegion region);
        LeagueRegion getCurrentRegion();
        void stopServices();
        string getVersion();
    }
}
