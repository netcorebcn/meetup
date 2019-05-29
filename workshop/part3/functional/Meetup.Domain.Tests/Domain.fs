namespace Meetup.Domain
open System

type MeetupId = MeetupId of Guid 
type MemberId = MemberId of Guid 
type Title = Title of string 
type Seats = private Seats of int 
type Address = Address of string 
type DateTimeRange = { Start: DateTime; End: DateTime }

type MeetupState = 
    | Created
    | Published
    | Canceled 
    | Closed

type RSVPInfo = {
    MeetupId: MeetupId;
    MemberId: MemberId;
    RSVPAt: DateTime
}

type RSVP = 
    | Sent of RSVPInfo 
    | Accepted of RSVPInfo 
    | Rejected of RSVPInfo 


type Command<'data> = {
    Data: 'data;
    OcurredOn: DateTime
}

type UnvalidatedMeetup = { 
    Id: Guid; 
    Title:string; 
    NumberOfSeats:int; 
    Location: Address; 
    Time: DateTimeRange 
    }
type UnvalidatedRsvp = { Id: Guid; MemberId: Guid; OcurredOn: DateTime }

type CreateMeetup = Command<UnvalidatedMeetup>
type PublishMeetup= Command<Guid>

type MeetupCommand = 
    | Create of CreateMeetup
    | Publish of PublishMeetup
    // | AcceptRsvp of AcceptRsvp 
    // | RejectRsvp of RejectRsvp
    // | Close of CloseMeetup 
    // | Cancel of CancelMeetup 


// type Create = CreateMeetup -> MeetupCreated
// type Publish = PublishMeetup -> MeetupPublished

module Seats =
    let create = 
        function
        | seats when seats > 1 && seats < 1000  -> Seats seats |> Ok
        | _ -> Error "seats must be from 1 to 1000"

    let value (Seats seats) = seats
    