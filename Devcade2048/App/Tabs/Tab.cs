using Microsoft.Xna.Framework;

namespace Devcade2048.App.Tabs;

public interface ITab {
    public SelectedTab Id();
    public void Begin();
    public void End();
    public void Update(GameTime gameTime);
    public void Draw(GameTime gameTime);
}