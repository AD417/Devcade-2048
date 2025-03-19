using Devcade2048.App.Render;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Devcade2048.App.State;

public abstract class BaseState {
    
    protected static SpriteBatch Pen {get; private set; }
    protected static Manager Game {get; private set; }
    public static readonly BaseState InitialState = new FadeInState();
    public static readonly Color Background = new Color(251, 194, 27);

    public static void SetRendering(SpriteBatch pen, Manager game) {
        Pen = pen;
        Game = game;
    }

    public BaseState() {
    }

    public virtual void Tick(GameTime gt) {
        // Nothing by default
    }

    public virtual float PercentComplete() {
        return 0.0F;
    }

    public virtual bool IsComplete() {
        return false;
    }

    public virtual BaseState NextState() {
        return this;
    }


    public virtual bool IsAcceptingInput() {
        return false;
    }

    public virtual BaseState ProcessInput() {
        return this;
    }

    public virtual bool IsUpdatingGrid() {
        return false;
    }

    public virtual bool IsRenderingTiles() {
        return true;
    }

    public virtual bool HasGameReset() {
        return false;
    }

    public virtual bool HasTabSwitched() {
        return false;
    }

    public virtual bool IsGameContinuing() {
        return false;
    }




    public virtual void Draw() {
        DrawTitle();
    }

    private static void DrawTitle() {
        DrawAsset(Asset.Title, new Vector2(60, 0), Color.White);
    }

    protected static void DrawAsset(Texture2D image, Vector2 pos) {
        // Color does not create compile-time constants.
        DrawAsset(image, pos, Color.White);
    }

    protected static void DrawAsset(Texture2D image, Rectangle pos) {
        // Color does not create compile-time constants.
        DrawAsset(image, pos, Color.White);
    }

    protected static void DrawAsset(SpriteFont font, string data, Vector2 pos) {
        DrawAsset(font, data, pos, Color.White);
    }

    protected static void DrawAsset(Texture2D image, Vector2 pos, Color color) {
        #region 
#if DEBUG
        Pen.Draw(image, pos, color);
#else
        Vector2 dim = new(
            image.Width,
            image.Height
        );
        Rectangle devcadePos = new(
            (int) (pos.X * 18 / 7),
            (int) (pos.Y * 18 / 7),
            (int) (dim.X * 18 / 7),
            (int) (dim.Y * 18 / 7)
        );
        pen.Draw(image, devcadePos, color);
#endif
        #endregion
    }

    protected static void DrawAsset(Texture2D image, Rectangle pos, Color color) {
        #region 
#if DEBUG
        Pen.Draw(image, pos, color);
#else
        Rectangle devcadePos = new(
            pos.X * 18 / 7,
            pos.Y * 18 / 7,
            pos.Width * 18 / 7,
            pos.Height * 18 / 7
        );
        pen.Draw(image, devcadePos, color);
#endif
        #endregion
    }

    // XXX: Copy located At TextBox.DrawString
    protected static void DrawAsset(SpriteFont font, string data, Vector2 pos, Color color) {
        #region
#if DEBUG
        Pen.DrawString(font, data, pos, color);
#else
        Vector2 actualPos = new(
            (int) (pos.X * 18.0f / 7.0f),
            (int) (pos.Y * 18.0f / 7.0f)
        );
        pen.DrawString(
            spriteFont: font, 
            text: data, 
            position: actualPos, 
            color: color, 
            rotation: 0.0f, 
            origin: new(), 
            scale: 18.0f / 7.0f, 
            effects: SpriteEffects.None, 
            layerDepth: 0.0f
        );
#endif
        #endregion
    }
}