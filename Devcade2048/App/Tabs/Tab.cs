using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.Tabs;

public interface ITab {
    public void Begin();
    public void End();
    public void Update(GameTime gameTime);
    public void Draw(GameTime gameTime);
    public SelectedTab NextTab();
}