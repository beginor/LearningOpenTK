using System.Collections.ObjectModel;

namespace Examples {

    public class GameComponentCollection : Collection<IGameComponent> {
    
        public void Draw() {
            foreach (IGameComponent component in Items) {
                component.Draw();
            }
        }

        public void Update() {
            foreach (IGameComponent component in Items) {
                component.Update();
            }
        }
    }
}
