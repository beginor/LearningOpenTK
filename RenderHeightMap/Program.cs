using System;

namespace RenderHeightMap {

    internal class Program {
    
        [STAThread]
        private static void Main(string[] args) {
            using (var window = new MainWindow()) {
                window.Run();
            }
        }
    }
}
