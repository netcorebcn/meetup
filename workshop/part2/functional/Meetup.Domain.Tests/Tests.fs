module Meetup.Domain.Tests

open System
open Xunit
open Meetup.Domain

[<Fact>]
let ``Given a meetup title when create then created`` () =
    Guid.NewGuid() 
    |> MeetupId.create 
    |> Result.bind (
        fun id -> 
        "title"
        |> MeetupTitle.create 
        |> Result.map (fun title -> (id,title)))
    |> Result.map (fun (id,title) -> Meetup.create id title)  
    |> function
        | Ok meetup -> 
            Assert.True("title" = MeetupTitle.value meetup.Title)
            Assert.True(MeetupState.Created = meetup.State) 
        | Error _ -> Assert.True(true)

[<Fact>]
let ``Given a meetup title when create then created with computation expression`` () =
        result {
            let! meetupId = Guid.NewGuid() |> MeetupId.create 
            let! meetupTitle = "title" |> MeetupTitle.create 
            return Meetup.create meetupId meetupTitle
        } 
        |> function
        | Ok meetup -> 
            Assert.True("title" = MeetupTitle.value meetup.Title)
            Assert.True(MeetupState.Created = meetup.State) 
        | Error _ -> Assert.True(true)
