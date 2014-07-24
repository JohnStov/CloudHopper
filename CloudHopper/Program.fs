open CloudHopper

[<EntryPoint>]
let main argv = 
    let game = new CloudHopper.CloudHopperGame()
    game.Run()
    0