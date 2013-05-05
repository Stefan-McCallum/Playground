// Learn more about F# at http://fsharp.net

open System
open FloodFillTypes

let rand = new Random()

let board = { new IBoard<Earth> with
                member x.board = Array2D.init 10 10 (fun x y ->
                                     match rand.Next(0, 1) with 
                                        | 1 -> Land
                                        | _ -> Water)                        
                                  (*array2D  [[Land;  Land;  Land;  Land;];
                                              [Water; Land;  Land;  Water;];
                                              [Land;  Water; Water; Water;];
                                              [Water; Land;  Land;  Water;]]*)

                member x.xSize = (Array2D.length1 x.board) - 1

                member x.ySize = (Array2D.length2 x.board) - 1 
                
                member x.allPositions = // get an array representing (x, y) tuples of the entire board
                    [0..x.xSize]
                         |> List.collect (fun row -> [0..x.ySize] |> List.map (fun col -> (row, col)))
}

let masses = FloodFill.getContiguousBlocks board Water

let largestList = List.maxBy(List.length) masses

System.Console.WriteLine("Largest mass is " + (List.length largestList).ToString());

Console.ReadKey()
let p = 0;