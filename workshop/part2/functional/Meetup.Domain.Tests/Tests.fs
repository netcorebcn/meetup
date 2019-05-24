module Meetup.Domain.Tests

open System
open Xunit
open Meetup.Domain
open System
open Meetup.Domain
open System.Text.RegularExpressions

[<Fact>]
let ``Given a created meetup when try to publish should change to publish meetup`` () =
    let createData = result{
        let! id = Guid.NewGuid() |> MeetupId.create
        let! title = "title" |> MeetupTitle.create
        return {Id = id;Title = title}
    }

    let publishData = result{
        let! seats = 100 |> NumberOfSeats.create
        let time = (DateTime.Now,DateTime.Now.AddDays(1.0)) |> DateTimeRange
        let address = "home" |> Address
        return {NumberOfSeats = seats;Location = address;Time=time}
    }
   

    let result = createData |> Result.map Meetup.Created |> Result.bind2 Meetup.publishMeetup publishData

    match result with
    |Ok meetup -> function 
                  |Meetup.Published p -> Assert.True(MeetupTitle.value(p.CreateData.Title) = "title") 
                  |_ -> "Incorrect state" |> failwith
    |Error e -> e |> failwith           

[<Theory>]
[<InlineData(0)>]
[<InlineData(-1)>]
[<InlineData(1001)>]
[<InlineData(10000)>]
let ``Seats less that 1 or over 1000 should return error`` (x) =
    x |> NumberOfSeats.create |> Result.isError |> Assert.True
      
[<Theory>]
[<InlineData(1)>]
[<InlineData(1000)>]
[<InlineData(2)>]
[<InlineData(100)>]
let ``Seats between 1 and 1000 should return Ok result`` (x) =
    x |> NumberOfSeats.create |> Result.isOk |> Assert.True
    
    