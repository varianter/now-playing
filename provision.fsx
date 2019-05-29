open System.IO
// Dependencies and FAKE-specific stuff
// ==============================================
#r "paket:
nuget Fake.Core.Target
nuget Fake.DotNet.Cli
nuget Fake.IO.FileSystem
nuget Fake.IO.Zip //"
// Use this for IDE support. Not required by FAKE 5.
#load ".fake/provision.fsx/intellisense.fsx"
open Fake.Core
open Fake.Core.TargetOperators
open Fake.IO.Globbing.Operators
// ==============================================

let workingDir = __SOURCE_DIRECTORY__

let azPath =
    Process.tryFindFileOnPath "az"
    |> function
        | Some az when File.Exists az -> az
        | _ -> failwith "Couldn't find az command on PATH" 

let runAzureCli cmd =
    let result =
        Process.execWithResult(fun psi ->
            { psi with
                FileName = azPath
                WorkingDirectory = workingDir 
                Arguments = cmd })
            (System.TimeSpan.FromMinutes 15.)
    result.Results |> Seq.iter (fun cm -> printfn "%O: %s" cm.Timestamp cm.Message)
    if not result.OK then failwith (sprintf "az command failed: '%s'." cmd )
    result.Messages |> Seq.last

let tryFindArgument (argument : string) (targetArguments : TargetParameter) =
    targetArguments.Context.Arguments
    |> Seq.map (fun arg -> arg.Split('=') |> fun split -> split.[0], split.[1])
    |> Map.ofSeq<string,string>
    |> Map.tryFind argument

Target.create "EnsureResourceGroupExists" <| fun ctx ->
    let appName = tryFindArgument "appName" ctx |> Option.get 
    sprintf "group create --location westeurope --name %s --query \"properties.provisioningState\""
        appName
    |> runAzureCli
    |> function
        | "\"Succeeded\"" -> ()
        | output -> failwith (sprintf "Could not provision Resource Group. Output: %s" output)

Target.create "RunGroupDeployment" <| fun ctx ->
    let appName = tryFindArgument "appName" ctx |> Option.get
    let spotifyApiClientId = tryFindArgument "spotifyApiClientId" ctx |> Option.get
    let spotifyApiClientSecret = tryFindArgument "spotifyApiClientSecret" ctx |> Option.get
    let parameterSet = 
        tryFindArgument "parameterSet" ctx 
        |> Option.get
        |> sprintf "infrastructure/parameters/%s.parameters.json" 
    let templateFile = "infrastructure/azuredeploy.json"
    sprintf "group deployment create -g %s --template-file %s --parameters %s --parameters appName=%s spotifyApiClientId=%s spotifyApiClientSecret=%s --query \"properties.provisioningState\""
        appName
        templateFile
        parameterSet
        appName
        spotifyApiClientId
        spotifyApiClientSecret
    |> runAzureCli
    |> function
        | "\"Succeeded\"" -> ()
        | _ -> failwith (sprintf "Could not provision resource group for app '%s' with parameterSet '%s'" appName parameterSet)

Target.create "DeployFunctionApp" <| fun ctx ->
    let appName = tryFindArgument "appName" ctx |> Option.get
    let zipFile = 
        tryFindArgument "zip" ctx
        |> function
            | Some zip -> zip
            | None -> 
                !! "./*.zip"
                ++ "./artifacts/*.zip"
                |> Seq.tryHead
                |> function
                    | Some zip -> zip
                    | None -> failwith "Couldn't find any zipfile for function app deployment. Try providing it with zip=pathtozip."

    sprintf "functionapp deployment source config-zip -g %s -n %s --src %s --query \"status\""
        appName
        appName
        zipFile
    |> runAzureCli
    |> function
        | "4" -> () // 4 = Success, 3 = Failed
        | output -> failwith (sprintf "Could not deploy zipfile '%s' to function app '%s'. Output: %s" zipFile appName output)

"EnsureResourceGroupExists"
    ==> "RunGroupDeployment"
    ==> "DeployFunctionApp"

Target.runOrDefaultWithArguments "DeployFunctionApp"