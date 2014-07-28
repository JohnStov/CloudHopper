module CloudHopper

open Microsoft.Xna.Framework
open Microsoft.Xna.Framework.Graphics
open Microsoft.Xna.Framework.Content
open Microsoft.Xna.Framework.Input
open System
open System.Windows.Input

type Actor =
    {
        Position : Vector2;
        Size : Vector2;
        Texture : Texture2D;
        Velocity : Vector2;
        ThrustAngle : int;
    }

    member this.Bounds 
        with get () = Rectangle ((int this.Position.X), (int this.Position.Y),(int this.Size.X), (int this.Size.Y))

let CreateActor (content:ContentManager) (textureName, position) = 
    let tex = content.Load<Texture2D> textureName
    let size = new Vector2 ((float32 tex.Width), (float32 tex.Height))
    { Position = position; Size = size; Texture = tex; Velocity = new Vector2(0.f, 0.f); ThrustAngle = 45 }

let DrawActor (sb:SpriteBatch) actor =
    sb.Draw (actor.Texture, actor.Position, Color.White)

let ForceInBounds (bounds : Rectangle) (a : Actor) =
    let result = 
        if bounds.Bottom < a.Bounds.Bottom 
        then 
            {a with Position = new Vector2(a.Position.X, float32 (bounds.Bottom - a.Bounds.Height)); Velocity = new Vector2(a.Velocity.X, 0.0f)}
        elif bounds.Top > a.Bounds.Top 
        then 
            {a with Position = new Vector2(a.Position.X, float32 bounds.Top); Velocity = new Vector2(a.Velocity.X, 0.0f)}
        else a

    if bounds.Left > result.Bounds.Left 
    then 
        {result with Position = new Vector2(float32 bounds.Left, result.Position.Y); Velocity = new Vector2(0.0f, a.Velocity.Y)}
    else if bounds.Right < result.Bounds.Right 
    then 
        {result with Position = new Vector2(float32 (bounds.Right - a.Bounds.Width), result.Position.Y); Velocity = new Vector2(0.0f, a.Velocity.Y)}
    else result

let MoveActor (bounds : Rectangle) (a : Actor) =
    ForceInBounds bounds { a with Position = a.Position + a.Velocity }

let ApplyGravity (a : Actor) =
    let g = 0.05
    let downward = Math.Min(double a.Velocity.Y + g, 5.0)
    { a with Velocity = new Vector2 (a.Velocity.X, float32 downward) }

let ApplyThrust (ks: KeyboardState) (a : Actor) =
    let thrust = if ks.IsKeyDown(Keys.Space) 
                 then 
                    let angle = (float (a.ThrustAngle) / 360.0) * (2.0 * Math.PI)
                    new Vector2(float32 (Math.Sin angle), float32 (Math.Cos angle) * -1.f) * 0.1f
                 else
                    new Vector2(0.f, 0.f)

    { a with Velocity = a.Velocity + thrust }

let rec SetThrustAngle(ks: Keys []) (a : Actor) =
    let CheckForArrow (a1 : Actor) (k : Keys) =
        if k = Keys.Left
        then 
            { a1 with ThrustAngle = Math.Min (a1.ThrustAngle + 5, 90) }
        elif k = Keys.Right
        then 
            { a1 with ThrustAngle = Math.Max (a1.ThrustAngle - 5, -90) }
        else a1

    Array.fold CheckForArrow a ks

let UpdateActor bounds actor =
    actor 
        |> SetThrustAngle (Keyboard.GetState().GetPressedKeys())
        |> ApplyThrust (Keyboard.GetState())
        |> ApplyGravity 
        |> MoveActor bounds

type CloudHopperGame () as g =
    inherit Game()

    let graphics = new GraphicsDeviceManager(g)
    let mutable spriteBatch = Unchecked.defaultof<SpriteBatch>
    let mutable actors = []
    let actorData = [("player.png", Vector2(10.f,28.f))]

    override g.LoadContent () =
        spriteBatch <- new SpriteBatch(g.GraphicsDevice)
        actors <- actorData |> List.map (CreateActor g.Content)

    override g.Update _ =
        actors <- actors 
            |> List.map (UpdateActor g.GraphicsDevice.Viewport.Bounds)

    override g.Draw _ =
        g.GraphicsDevice.Clear Color.CornflowerBlue
        spriteBatch.Begin ()
        ignore (actors |> List.map (DrawActor spriteBatch))
        spriteBatch.End ()
