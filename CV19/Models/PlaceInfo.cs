using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace CV19.Models
{
    internal class PlaceInfo
    {
        public string Name { get; set; }

        public Point Location { get; set; }

        public IEnumerable<ConfirmedCount> Counts { get; set; }
    }
}
