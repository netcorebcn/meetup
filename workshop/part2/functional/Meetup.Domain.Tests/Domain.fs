namespace Meetup.Domain

open System

type MeetupId = MeetupId of Guid
type NumberOfSeats = private NumberOfSeats of int
type MeetupTitle= private MeetupTitle of string
type Address = Address of string
type DateTimeRange = DateTimeRange of DateTime * DateTime

type CreateData = {Id:MeetupId;Title:MeetupTitle}
type PublishData = {NumberOfSeats:NumberOfSeats;Time:DateTimeRange;Location:Address}
type MeetupData = {CreateData:CreateData;PublishData:PublishData}

type Meetup =
    | Created of CreateData
    | Published of MeetupData
    | Canceled of MeetupData
    | Closed of MeetupData

module MeetupTitle =
    let create = function
        |title when String.IsNullOrEmpty(title) -> Error "NumberOfSeats can not be negative"
        |title -> title |> MeetupTitle |> Ok        

    let value (MeetupTitle title) = title     

module MeetupId=
    let create = function
        | id when id=System.Guid.Empty -> Error "Empty meetupId"
        | id -> id |> MeetupId |> Ok
        

module Meetup =
    let publishMeetup publishData meetup =
        match meetup with
        | Created created-> {CreateData = created;PublishData = publishData} |> Published |> Ok
        | Published _ -> Error "Already published"
        | Canceled _ -> Error "Meetup is canceled"
        | Closed _ -> Error "Meetup is closed"

module NumberOfSeats =
    let create = function
        | seats when seats < 1 -> Error "NumberOfSeats can not be negative"
        | seats when seats > 1000 -> Error "NumberOfSeats can not be more than 1000"
        | seats -> seats |> NumberOfSeats |> Ok
    
    let value (NumberOfSeats seats) = seats