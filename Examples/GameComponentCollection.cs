using System.Collections.ObjectModel;

namespace Examples {

	public class GameComponentCollection : Collection<IGameComponent> {

		public void Draw() {
			foreach (var component in this.Items) {
				component.Draw();
			}
		}

		public void Update() {
			foreach (var component in this.Items) {
				component.Update();
			}
		}
	}


}
