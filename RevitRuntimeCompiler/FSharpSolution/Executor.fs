namespace FSharpSolution
open Autodesk.Revit.DB;
open Autodesk.Revit.UI;
open Helpers;

module Exec = 
    
    let execFromApp (app :UIApplication) (channel:Channel) = async { 
        ignore()    
    }

    let execFromDoc (doc: Document) (channel:Channel) = async{
        ignore()
    }


















[<AbstractClass;Sealed>]
type Executor() = 
    static member Execute(trace) = trace ||> Exec.execFromApp |> Async.StartAsTask

    static member Execute(trace) = trace ||> Exec.execFromDoc |> Async.StartAsTask
