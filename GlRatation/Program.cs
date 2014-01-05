using System;
using System.Collections.Generic;
using System.Text;

namespace GlRatation {

	class Program {

		static void Main(string[] args) {
			using (MainWindow win = new MainWindow()) {
				win.Run(30);
			}
		}
	}
}
