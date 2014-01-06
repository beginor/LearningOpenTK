using System.Windows.Forms;

namespace TerrainGL {

    internal class Program {
    
        private static void Main(string[] args) {
            Application.SetCompatibleTextRenderingDefault(false);
            Application.EnableVisualStyles();

            var mc = new MyGLControl();
            mc.Dock = DockStyle.Fill;
            mc.Margin = new Padding(8);

            var form = new Form {
                                    Width = 800,
                                    Height = 600
                                };
            form.Controls.Add(mc);

            Application.Run(form);
        }
    }
}
