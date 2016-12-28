using System.Drawing;
using System.Windows.Forms;
using Cmn.Util;

namespace Ch24.Util
{
    public static partial class U24
    {
        public static void Tsto(this Bitmap bmp)
        {
            var form = new Form();
            var imgbox =
                new Cyotek.Windows.Forms.ImageBox
                {
                    Image = bmp,
                    Dock = DockStyle.Fill,
                };

            var b = new Button();
            form.Controls.Add(imgbox);
            form.Controls.Add(b);

            form.CancelButton = b;
            form.TopLevel = true;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ClientSize = new Size(
                U.Min(Screen.PrimaryScreen.WorkingArea.Width, bmp.Width + 20),
                U.Min(Screen.PrimaryScreen.WorkingArea.Height, bmp.Height + 20));
            form.Shown += (sender, args) => form.Activate();
            form.ShowDialog(null);
        }
    }
}
