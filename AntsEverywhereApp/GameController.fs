﻿//
// This is Richard Minerich's F# Ant Colony Silverlight Ediiton
// Visit my Blog at http://RichardMinerich.com
// This code is free to be used for anything you like as long as I am properly acknowledged.
//
// The basic Silverlight used here is based on Phillip Trelford's Missile Command Example
// http://www.trelford.com/blog/post/MissileCommand.aspx
//

namespace AntsEverywhereApp

open System
open System.Collections.Generic
open System.Windows
open System.Windows.Controls
open System.Windows.Data
open System.Windows.Input
open System.Windows.Media
open System.Windows.Shapes
open System.Windows.Threading

open AntsEverywhereLib.Types

type GameController () as this =
    inherit UserControl ()

    //
    // Phillip Trelford's great pattern for IDisposables
    //

    let mutable disposables = []
    let remember (disposable : IDisposable) = disposables <- disposable :: disposables
    let dispose (d:IDisposable) = d.Dispose()
    let forget () = disposables |> List.iter dispose; disposables <- []

    let outerLayout = new StackPanel()
    let layout = Grid()
    do this.Content <- layout
    
    let defaultAI = AI.TestAntBehavior() :> AntsEverywhereLib.Types.IAntBehavior

    let aiSelectionControl = new AISelectionControl(defaultAI)
    do aiSelectionControl |> remember

    let simulationControl = new SimulationControl()
    do simulationControl |> remember

    let switchControls newControl =
        layout.Children.Clear()
        layout.Children.SafeAdd newControl

    let startWithAI (blackAI : IAntBehavior) (redAI : IAntBehavior) maxCycles: unit =
        try 
            switchControls (simulationControl)
            simulationControl.StartSimulation blackAI redAI maxCycles
            
        with e -> layout.Children.SafeAdd( new TextBox(Text = sprintf "%O" e) )
   
    do aiSelectionControl.LoadedAIEvent.Subscribe (fun (redAI, blackAI, timed) -> startWithAI redAI blackAI timed) |> ignore
       simulationControl.LoadAIEvent.Subscribe (fun _ -> switchControls aiSelectionControl) |> ignore

    do startWithAI defaultAI defaultAI maxWorldCycles

    member x.SetInitParams args = 
        aiSelectionControl.addItems args

    interface System.IDisposable with
        member this.Dispose() = forget()