module FloodFill

open FloodFillTypes
open Positions

(* 
   Iterate over each element in a 2d array, passing the x and y
   coordinate and the board, to the supplied function
   which can return an item. The items are all cons together
   and the function returns a new list
*)

let private forEachElement (applier:(X -> Y -> IBoard<'a> -> 'b)) (twoDimArray:IBoard<'a>) =
    let mutable items = [] 
    for x in 0..(twoDimArray.xSize) do
        for y in 0..(twoDimArray.ySize) do            
            items <- (applier x y twoDimArray)::items
    items

(*
    Looks for a specified contigoius block
    and keeps track of processed positions using a 
    reference cell of a list of positions (supplied by the caller)
*)


let private findMassStartingAt (position:Position) (canvas:IBoard<'A>) (target:'A) (positionSeed:ProcessedPositions) : MassFinder = 
    let rec findMassStartingAt' position (currentMass:ContiguousPoints, processedList:ProcessedPositions) cont = 
            // if you move off the board return
        if offBoard position canvas then
            cont (currentMass, processedList)

        // if you already processed this position then don't do anything
        else if positionExists position processedList then
            cont (currentMass, processedList)
        else  
            
            // branch out left, up, right, and down and see what you can find
            let up = moveUp position
            let down = moveDown position
            let left = moveLeft position
            let right = moveRight position
            
            let found = positionOnTarget position canvas target   

            let updatedProcess = position::processedList

            match found with 
                | true ->                    
                           let massState = (position::currentMass, updatedProcess)

                           findMassStartingAt' up  massState (fun foundMassUp -> 
                           findMassStartingAt' down foundMassUp (fun foundMassDown ->
                           findMassStartingAt' left foundMassDown (fun foundMassLeft ->
                           findMassStartingAt' right foundMassLeft cont))) 

                | false -> 
                    // if you didn't find anything return the masses that you 
                    // found prevoiusly
                    cont((currentMass, updatedProcess))

    findMassStartingAt' position ([], positionSeed) id

(*
    Find first non processed position
*)

let private firstNonProcessedPosition processedList (canvas:IBoard<'T>) = 
    match processedList with
        | [] -> 
            Some((0, 0))
        | _ ->
            if List.length processedList = (canvas.xSize * canvas.ySize) then
                None 
            else
                let rec nonProcessed' (x, y) cont = 
                    if offBoard (x,y) canvas then 
                        cont(None)
                    else
                        if not (List.exists(fun pos -> pos = (x,y)) processedList) then
                            cont (Some((x,y)))
                        else
                            nonProcessed' (x + 1, y) (fun found1 -> 
                                if Option.isSome found1 then
                                    found1
                                else
                                    nonProcessed' (x, y + 1) (fun found2 ->
                                        cont(found2)))

                let minX = List.minBy(fst) processedList
                let minY = List.minBy(snd) processedList

                nonProcessed' (fst minX, snd minY) id                     
                                            

(*
    Finds all contiguous blocks of the specified type
    and returns a list of lists (each list is the points for a specific
    block)
*)
    
let getContiguousBlocks (board:IBoard<'T>) target = 
        
    let rec findBlocks' (blocks, processed:PositionList) = 
        
        let findMass x y board = findMassStartingAt (x, y) board target processed

        // find the first non processed block 
        // and try and find its contigoius area
        // if it isn't a valid area the block it returns will be
        // empty and we can exclude it
        match firstNonProcessedPosition processed board with 
            | None -> blocks
            | Some (x, y) -> 
                let (block, processed) = findMass x y board


                findBlocks' ((match block with 
                                | [] -> blocks
                                | _ -> block::blocks), processed)
        
    findBlocks' ([],[])

(*
    Returns a list of points representing a contigious block 
    of the type that the point was at. 
*)

let floodFillArea (point:Position) (canvas:IBoard<'T>) =
    let (x, y) = point
    let itemAtPoint = Array2D.get canvas.board x y
    
    findMassStartingAt point canvas itemAtPoint [] |> fst


(* 
    Test functions to run it
*)
