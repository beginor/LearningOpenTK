namespace GlRatation {

    internal class Program {
    
        private static void Main(string[] args) {
            using (var win = new MainWindow()) {
                win.Run(30);
            }
        }
    }
}
