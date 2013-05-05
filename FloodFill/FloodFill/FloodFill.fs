module FloodFill

open FloodFillTypes

(* 
    Helper methods to move the position around
*)
                                          
let private moveRight position = 
    let (x,y) = position
    (x + 1, y)

let private moveLeft position = 
    let (x,y) = position
    (x - 1, y)

let private moveUp position = 
    let (x,y) = position
    (x, y + 1)

let moveDown position = 
    let (x,y) = position
    (x, y - 1)

(*
    Size helpers
*)

let private xSize board = Array2D.length1 board

let private ySize board = Array2D.length2 board

let private offBoard position board = 
    let (x,y) = position
    x < 0 || y < 0 || x >= (xSize board) || y >= (ySize board)

(*
    Alias to push elements onto a list
*)

let private markPosition position previousSpots = position::previousSpots

(*
    Determines if the position on the board equals the target
*)

let private positionOnTarget position board target = 
    if offBoard position board then 
        false
    else
        let (x, y) = position
        (Array2D.get board x y) = target

(*
    Alias to find if we already processed a position
*)

let private positionExists position list = 
    List.exists(fun pos -> pos = position) list

(* 
   Iterate over each element in a 2d array, passing the x and y
   coordinate and the board, to the supplied function
   which can return an item. The items are all cons together
   and the function returns a new list
*)

let private forEachElement (applier:(X -> Y -> Board<'a> -> 'b)) (twoDimArray:Board<'a>) =
    let mutable items = [] 
    for x in 0..(xSize twoDimArray)-1 do
        for y in 0..(ySize twoDimArray)-1 do            
            items <- (applier x y twoDimArray)::items
    items


(*
    Looks for a specified contigoius block
    and keeps track of processed positions using a 
    reference cell of a list of positions (supplied by the caller)
*)

let private elemAt board (x, y) = Array2D.get board x y

(*    
let displayPosition board position = 
    //System.Threading.Thread.Sleep(500)
    
    Console.Clear()    

    for x in 0..(xSize board)-1 do
        Console.WriteLine()

        for y in 0..(ySize board)-1 do                                
            let elem = ViewType (elemAt board (x, y))

            let (x1, y1) = position

            if (x,y) = position then
                Console.Write("x")
            else
                Console.Write elem
        
            Console.Write "    "
        
 *)  

let private findMassStartingAt (position:Position) (board:Board<'A>) (target:'A) (positionSeed:ProcessedPositions) : MassFinder = 
    let rec findMassStartingAt' position (currentMass:ContiguousPoints, processedList:ProcessedPositions) cont = 
            // if you move off the board return
        if offBoard position board then
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
            
            let found = positionOnTarget position board target   

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
    Finds all items of list2 that are not in list1
*)

let private except list1 list2 = 
    let listContainsElement item = List.exists (fun i -> i = item) list1
    List.filter(fun item -> not (listContainsElement item)) list2

(*
    Find first non processed position
*)

let private firstNonProcessedPosition processedList xCount yCount = 
    match processedList with
        | [] -> 
            Some((0, 0))
        | _ ->
            if List.length processedList = (xCount * yCount) then
                None 
            else

                // get an array representing (x, y) tuples of the entire board
                let totalPositions = [0..xCount] |> List.collect (fun x -> [0..yCount] |> List.map (fun y -> (x, y)))

                // set intersections from the total positions array and the entire board
                let intersections = Set.intersect (Set.ofList totalPositions) (Set.ofList processedList)
                                        |> List.ofSeq

                // exclude the intersections from the total list
                let excludes = except intersections totalPositions

                match excludes with 
                    | [] -> None
                    | _ -> Some(List.head excludes)

                        

(*
    Finds all contiguous blocks of the specified type
    and returns a list of lists (each list is the points for a specific
    block)
*)
    
let getContiguousBlocks board target = 
    
    let xCount = (xSize board) - 1
    let yCount = (ySize board) - 1

    let rec findBlocks' (blocks, processed:PositionList) = 
        
        let findMass x y board = findMassStartingAt (x, y) board target processed

        // find the first non processed block 
        // and try and find its contigoius area
        // if it isn't a valid area the block it returns will be
        // empty and we can exclude it
        match firstNonProcessedPosition processed xCount yCount with 
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

let floodFillArea (point:Position) (canvas:Board<'T>) =
    let (x, y) = point
    let itemAtPoint = Array2D.get canvas x y
    
    findMassStartingAt point canvas itemAtPoint [] |> fst


(* 
    Test functions to run it
*)
