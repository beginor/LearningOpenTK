using System;
using System.Collections.Generic;
using System.Text;

namespace RenderHeightMap {

	class Program {

		[STAThread]
		static void Main(string[] args) {
			using (MainWindow window = new MainWindow()) {
				window.Run();
			}
		}
	}
}
