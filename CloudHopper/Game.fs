module CloudHopper

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content

type Actor =
    {
        Position : Vector2;
        Size : Vector2;
        Texture : Texture2D;
    }

    member this.Bounds 
        with get () = Rectangle ((int this.Position.X), (int this.Position.Y),(int this.Size.X), (int this.Size.Y))

let CreateActor (content:ContentManager) (textureName, position) = 
    let tex = content.Load<Texture2D> textureName
    let size = new Vector2 ((float32 tex.Width), (float32 tex.Height))
    { Position = position; Size = size; Texture = tex }

let DrawActor (sb:SpriteBatch) actor =
    sb.Draw (actor.Texture, actor.Position, Color.White)

let MoveActor (bounds : Rectangle) x y (a : Actor) =
    if bounds.Contains a.Bounds then
        { Position = new Vector2 ((float32 a.Position.X + x), (float32 a.Position.Y + y)); Size = a.Size; Texture = a.Texture }
    else
        a

type CloudHopperGame () as g =
    inherit Game()

    let graphics = new GraphicsDeviceManager(g)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable actors = []
    let actorData = [("player.png", Vector2(10.f,28.f))]

    override g.LoadContent () =
        spriteBatch <- new SpriteBatch(g.GraphicsDevice)
        actors <- actorData |> List.map (CreateActor g.Content)

    override g.Update gametime =
        let viewport = g.GraphicsDevice.Viewport.Bounds
        actors <- actors |> List.map (MoveActor viewport 1.f 0.f)

    override g.Draw gametime =
        g.GraphicsDevice.Clear Color.CornflowerBlue
        spriteBatch.Begin ()
        ignore (actors |> List.map (DrawActor spriteBatch))
        spriteBatch.End ()
        base.Draw gametime