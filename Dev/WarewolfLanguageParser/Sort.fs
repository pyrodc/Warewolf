﻿module Sort

open DataASTMutable
open LanguageAST
//open LanguageEval
open Microsoft.FSharp.Text.Lexing
open DataASTMutable
open WarewolfParserInterop
open CommonFunctions
open WarewolfDataEvaluationCommon



let rec sortRecst (recset:WarewolfRecordset) (colName:string) (desc:bool)=
    let data = recset.Data.[colName]
    let positions = [0..recset.Data.[PositionColumn].Count-1]
    let interpolated = Seq.map2 (fun a b ->  a,  b) data positions
    let sorted = if not desc then Seq.sortBy (fun x ->  (fst x)  ) interpolated else Seq.sortBy (fun x ->  (fst x)   ) interpolated |> List.ofSeq |>List.rev |> Seq.ofList
    let indexes = Seq.map snd sorted  |> Array.ofSeq
    let data = Map.map (fun a b -> ApplyIndexes b indexes a) recset.Data
    {recset with Data = data; Optimisations =Ordinal ; LastIndex = positions.Length  }

and ApplyIndexes (data:WarewolfParserInterop.WarewolfAtomList<WarewolfAtom>) (indexes : int[]) (name:string) =
    let newdata = new WarewolfParserInterop.WarewolfAtomList<WarewolfAtom>(WarewolfAtom.Nothing)
    if name = PositionColumn then
        for x in [1..data.Count] do
            newdata.AddSomething ( Int x)
        newdata
    else        
        for x in indexes do
            newdata.AddSomething data.[x]
        newdata

and sortRecset (exp:string) (desc:bool) (update:int)  (env:WarewolfEnvironment) =
    let left = parseLanguageExpression exp update
    match left with 
                |   RecordSetExpression b ->  let recsetopt = env.RecordSets.TryFind b.Name
                                              match recsetopt with
                                              | None -> failwith "record set does not exist"  
                                              | Some a -> let sorted =  sortRecst a b.Column desc
                                                          let recset = Map.remove b.Name env.RecordSets  
                                                          let recsets =   Map.add  b.Name sorted recset        
                                                          {env with RecordSets = recsets }
                |_-> failwith "Only recordsets that contain recordset columns can be sorted"