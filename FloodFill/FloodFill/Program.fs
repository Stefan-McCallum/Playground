// Learn more about F# at http://fsharp.net

open System
open FloodFillTypes

let ViewType e = 
    match e with
        | Land -> "L"
        | Water -> "W"

let board = array2D [[Land;  Land;  Land;  Land;];
                     [Water; Land;  Land;  Water;];
                     [Land;  Water; Water; Water;];
                     [Water; Land;  Land;  Water;]]

let boardInt = array2D [[0;  0;  0;  1;];
                        [1; 0;  0;  1;];
                        [0;  1; 1; 1;];
                        [1; 0;  0;  1;]]


let masses = FloodFill.getContiguousBlocks board Land

let largestList = List.maxBy(List.length) masses

System.Console.WriteLine("Largest mass is " + (List.length largestList).ToString());


let p = 0;