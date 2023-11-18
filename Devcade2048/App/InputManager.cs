using Devcade;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using System;

namespace Devcade2048.App;

public static class InputManager {
    public static bool AnyButtonPressed() {
        return Keyboard.GetState().IsKeyDown(Keys.Space)
            || Input.GetButton(1, Input.ArcadeButtons.A1)
            || Input.GetButton(1, Input.ArcadeButtons.A2)
            || Input.GetButton(1, Input.ArcadeButtons.A3)
            || Input.GetButton(1, Input.ArcadeButtons.A4)
            || Input.GetButton(1, Input.ArcadeButtons.B1)
            || Input.GetButton(1, Input.ArcadeButtons.B2)
            || Input.GetButton(1, Input.ArcadeButtons.B3)
            || Input.GetButton(1, Input.ArcadeButtons.B4)
            || Input.GetButton(2, Input.ArcadeButtons.A1)
            || Input.GetButton(2, Input.ArcadeButtons.A2)
            || Input.GetButton(2, Input.ArcadeButtons.A3)
            || Input.GetButton(2, Input.ArcadeButtons.A4)
            || Input.GetButton(2, Input.ArcadeButtons.B1)
            || Input.GetButton(2, Input.ArcadeButtons.B2)
            || Input.GetButton(2, Input.ArcadeButtons.B3)
            || Input.GetButton(2, Input.ArcadeButtons.B4);
    }

    public static Manager.Direction GetStickDirection() {
        Vector2 direction = Vector2.Zero;
#if DEBUG
        if (Keyboard.GetState().IsKeyDown(Keys.W)) return Manager.Direction.Up;
        if (Keyboard.GetState().IsKeyDown(Keys.S)) return Manager.Direction.Down;
        if (Keyboard.GetState().IsKeyDown(Keys.D)) return Manager.Direction.Right;
        if (Keyboard.GetState().IsKeyDown(Keys.A)) return Manager.Direction.Left;
#else
        direction = Input.GetStick(1);
        if (direction.Y >  0.9) return Manager.Direction.Up;
        if (direction.Y < -0.9) return Manager.Direction.Down;
        if (direction.X < -0.9) return Manager.Direction.Left;
        if (direction.X >  0.9) return Manager.Direction.Right;
        direction = Input.GetStick(2);
        if (direction.Y >  0.9) return Manager.Direction.Up;
        if (direction.Y < -0.9) return Manager.Direction.Down;
        if (direction.X < -0.9) return Manager.Direction.Left;
        if (direction.X >  0.9) return Manager.Direction.Right;
#endif
        return Manager.Direction.None;
    }

    public static bool isButtonPressed(Button button) {
        switch (button) {
            case Button.None: return !AnyButtonPressed();
            case Button.Red: 
                return  
                    // Start; Restart
                    Keyboard.GetState().IsKeyDown(Keys.R)
                 || Input.GetButton(1, Input.ArcadeButtons.A1)
                 || Input.GetButton(2, Input.ArcadeButtons.A1)
                ;
            case Button.Blue: 
                return  
                    // Exit.
                    Keyboard.GetState().IsKeyDown(Keys.B)
                 || Input.GetButton(1, Input.ArcadeButtons.A2)
                 || Input.GetButton(2, Input.ArcadeButtons.A2)
                ;
            case Button.Green: 
                return  
                    Keyboard.GetState().IsKeyDown(Keys.G)
                 || Input.GetButton(1, Input.ArcadeButtons.A3)
                 || Input.GetButton(2, Input.ArcadeButtons.A3)
                ;
            case Button.White: 
                return  
                    Keyboard.GetState().IsKeyDown(Keys.C)
                 || Input.GetButton(1, Input.ArcadeButtons.A4)
                 || Input.GetButton(2, Input.ArcadeButtons.A4)
                ;
            case Button.Purple: 
                return  
                    Keyboard.GetState().IsKeyDown(Keys.P)
                 || Input.GetButton(1, Input.ArcadeButtons.B1)
                 || Input.GetButton(1, Input.ArcadeButtons.B2)
                 || Input.GetButton(1, Input.ArcadeButtons.B3)
                 || Input.GetButton(1, Input.ArcadeButtons.B4)
                ;
            case Button.Yellow: 
                return
                    Keyboard.GetState().IsKeyDown(Keys.Y)
                 || Input.GetButton(2, Input.ArcadeButtons.B1)
                 || Input.GetButton(2, Input.ArcadeButtons.B2)
                 || Input.GetButton(2, Input.ArcadeButtons.B3)
                 || Input.GetButton(2, Input.ArcadeButtons.B4)
                ;
        }
        throw new Exception("Invalid button type: " + button);
    }
}