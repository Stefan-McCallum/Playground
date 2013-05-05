module ListUtils


(*
    Finds all items of list2 that are not in list1
*)

let except list1 list2 = 
    let listContainsElement item = List.exists (fun i -> i = item) list1
    List.filter(fun item -> not (listContainsElement item)) list2
