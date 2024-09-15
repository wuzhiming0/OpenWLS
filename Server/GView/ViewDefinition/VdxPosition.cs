using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;

namespace OpenWLS.Server.GView.ViewDefinition
{
    public class VdxPosition
    {
        int trackID;
        public VdTrack Track  {    get; set;     } 
        public double Position  {    get; set;     }

        public int TrackID
        {
            get
            {
                if (Track != null)
                    return Track.ID;
                else
                    return trackID;
            }
            set
            {
                if (Track != null)
                    Track.ID = value;
                else
                    trackID = value;
            }
        }

        public void InitTrack(VdTracks ts)
        {
            if (trackID < ts.Count)
                Track = ts[trackID];
            else
                Track = null;
        }

        public double GetX()
        {
            if (Track != null)
                return Track.GetX(Position);
            return Double.NaN;
        }

    }
}
