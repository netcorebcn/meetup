module Tests

open System
open Xunit

type MeetupId = MeetupId of Guid
type MeetupTitle= MeetupTitle of string
type NumberOfSeats = NumberOfSeats of int
type Address = Address of string
type DateTimeRange = DateTimeRange of DateTime * DateTime
type MeetupState =
    | Created
    | Published
    | Canceled
    | Closed

type Meetup = {
    Id: MeetupId 
    Title: MeetupTitle
    NumberOfSeats: NumberOfSeats option
    Time: DateTimeRange option
    Location: Address option
    State: MeetupState }

let create meetupId title =
    {
        Id = MeetupId meetupId
        Title = MeetupTitle title
        Location = None
        NumberOfSeats = None
        Time = None
        State = Created
    }

let publish location numberOfSeats startTime endTime meetup =
    {
        meetup with
            Location = Some (Address location)
            NumberOfSeats = Some (NumberOfSeats numberOfSeats)
            Time = Some (DateTimeRange (startTime,endTime))
            State = Published 
    }

[<Fact>]
let ``Given a meetup title when create then created`` () =
    let id = Guid.NewGuid()
    let meetup = create id "event sourcing" 
    Assert.Equal(Created, meetup.State)

[<Fact>]
let ``Given created meetup when publish then published`` () =
    let meetup = 
        "event sourcing"
        |> create (Guid.NewGuid())
        |> publish "SanFrancisco, MountainView" 15 DateTime.UtcNow (DateTime.UtcNow.AddHours(2.0))

    Assert.Equal(Published, meetup.State)
