using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wecomp.Gvis
{
    public class Layout
    {
        /// <summary>
        /// creates a bmp from the nodes and edges of a SMALL graph.
        /// </summary>
        public static Bitmap ToBmp<T>(IList<T> rgt, Func<T, IEnumerable<T>> dgentNeighbour, Func<T, string> dglabel = null)
        {
            var d = new Laydg();
            var mp = new Dictionary<T, Layn>();
            foreach (var t in rgt)
            {
                var node = new SpotLayn(Color.Aqua, dglabel == null ? t.ToString() : dglabel(t));
                d.AddNode(node);
                mp[t] = node;
            }


            foreach (var t in rgt)
                foreach (var tNeighbour in dgentNeighbour(t))
                    mp[t].AddChild(mp[tNeighbour]);

            d.Arrange();

            var bmp = new Bitmap(1024, 1024);
            using (Graphics g = Graphics.FromImage(bmp))
                d.Draw(g, new Rectangle(0, 0, 1024, 1024));

            return bmp;
        }
    }
}
