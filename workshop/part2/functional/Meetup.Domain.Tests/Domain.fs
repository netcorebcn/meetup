namespace Meetup.Domain

open System

type MeetupId = MeetupId of Guid
type NumberOfSeats = private NumberOfSeats of int
type MeetupTitle= private MeetupTitle of string
type Address = private Address of string
type DateTimeRange = private DateTimeRange of DateTime * DateTime
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

// let failOnError aResult =
//     match aResult with
//     | Ok success -> success 
//     | Error error -> failwithf "%A" error

module MeetupTitle =
    let create =
        let validate title=
            if String.IsNullOrEmpty(title) then
                Error "NumberOfSeats can not be negative"
            else
                Ok (MeetupTitle title)

        // validate >> failOnError 
        validate

    let value (MeetupTitle title) = title 

    let UseMeetupId (title:MeetupTitle):Result<String,String> =
        title.ToString() + " append" |>  Ok

    // first example
    let temp = "test" |> create |> Result.bind UseMeetupId
    
        // pipe a two-track value into a switch function 
    let (>>=) x f = 
        Result.bind f x    

    // With operator
    let useId = "test" |> create >>= UseMeetupId

    

module MeetupId=
    let create id=
        if id= System.Guid.Empty  then
            Error "Empty meetupId"
        else
            Ok (MeetupId id)
 
        // id |> validate |> failOnError
        

module Meetup=
    let create meetupId title =
         {
            Id = meetupId
            Title = title 
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


module NumberOfSeats =
    let create seats=
        if seats < 1 then
            Error "NumberOfSeats can not be negative"
        else if seats > 1000 then
            Error "NumberOfSeats can not be more than 1000"
        else
            Ok (NumberOfSeats seats)
    
    let value (NumberOfSeats seats) = seats 