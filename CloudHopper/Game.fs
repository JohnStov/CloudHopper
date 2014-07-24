module CloudHopper

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics

type CloudHopperGame () as g =
    inherit Game()

    let graphics = new GraphicsDeviceManager(g)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>

    override g.LoadContent () =
        spriteBatch <- new SpriteBatch(g.GraphicsDevice)

    override g.Draw gametime =
        g.GraphicsDevice.Clear Color.CornflowerBlue
        base.Draw gametime