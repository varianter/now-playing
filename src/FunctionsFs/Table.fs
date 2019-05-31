module Table
open Microsoft.WindowsAzure.Storage.Table

let createIfNotExist (table: CloudTable) =
    table.CreateIfNotExistsAsync()
    |> Async.AwaitTask
    |> Async.Ignore

let execute operation (table: CloudTable) =
    operation
    |> table.ExecuteAsync
    |> Async.AwaitTask

let executeIgnore operation table =
    execute operation table
    |> Async.Ignore

let insert entity table =
    executeIgnore (TableOperation.Insert entity) table

let delete entity table =
    executeIgnore (TableOperation.Delete entity) table

let retrieve partitionKey rowKey table =
    execute (TableOperation.Retrieve(partitionKey, rowKey)) table