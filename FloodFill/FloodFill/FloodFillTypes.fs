module FloodFillTypes


type Board<'T> = 'T[,]

type X = int

type Y = int

type Position = X * Y

type PositionList = Position list 

type ProcessedPositions = PositionList

type ContiguousPoints = PositionList

type MassFinder = ContiguousPoints * ProcessedPositions

type Earth = 
    | Land
    | Water

type IBoard<'T> = 
    abstract board : Board<'T>    
    abstract xSize : int
    abstract ySize : int
    abstract allPositions : PositionList