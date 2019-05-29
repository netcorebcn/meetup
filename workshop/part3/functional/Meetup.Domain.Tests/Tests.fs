module Tests

open System
open Xunit
open Meetup.Domain


[<Fact>]
let ``Given something When command Then event`` () =

    let title = Title "EventSourcing with Marten"
    let seats = Seats.create 30 |> function
        | Ok a -> Seats.value a
        | Error b -> 0

    Assert.Equal(30,seats)


    // let meetup = { 
    //     Id = MeetupId (Guid.NewGuid()); 
    //     Title = title; 
    //     Seats = Some (Seats.create 30);
    //     Location = Some (Address "California, Cupertino");
    //     Time = None;
    //     RSVPs = None;
    // }

    // printfn "%A" meetup
    // Assert.Equal(title , meetup.Title)
