module Helpers
open System.Threading.Tasks;

type Channel() =
    member this.WriteAsync(message : string) = Task.CompletedTask