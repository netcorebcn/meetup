using System;
using System.Data;
using System.Linq;
using Dapper;
using Meetup.Relational.Models;

namespace Meetup.Relational.Services
{
    public class MeetupGroupService
    {
        IDbConnection _connection;
        public MeetupGroupService(IDbConnection connection)
        {
            _connection = connection;
        }

        public MeetupGroup GetMeetup(Guid id) =>
             _connection
                .Query<MeetupGroup>("select * from MeetupGroup where id = @id", new { id = id })
                .First();

    }
}
