using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MS.Katusha.Domain
{
    public static class KatushaDbSqlStatements
    {
        public const string DeleteUserSql = @"DECLARE @guid varchar(200)
DECLARE @profileId bigint
DECLARE @userId bigint

--User Guid to select
SET @guid = '{0}'

select @userId = Id from Users(nolock) where Guid = @guid
select @profileId = Id from Profiles(nolock) where UserId = @userId
select @profileId, @userId

delete from Conversations where ToId = @profileId
delete from Conversations where FromId = @profileId
delete from CountriesToVisits where ProfileId = @profileId
delete from LanguagesSpokens where ProfileId = @profileId
delete from SearchingFors where ProfileId = @profileId
delete from States where ProfileId = @profileId
delete from Visits where ProfileId = @profileId
delete from Visits where VisitorProfileId = @profileId

delete pb from PhotoBackups as pb JOIN Photos as p ON pb.Guid = p.Guid where p.ProfileId = @profileId
delete from Photos where ProfileId = @profileId

delete from Profiles where UserId = @userId
delete from Users  where Id = @userId";

    }
}
