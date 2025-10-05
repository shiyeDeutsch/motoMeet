using System;
using System.Linq.Expressions;
using System.Linq;

namespace motoMeet
{
    public class EventSpecification : Specification<Event>
    {
        public EventSpecification() : base()
        {
        }

        public EventSpecification ByIsPublic(bool isPublic)
        {
            AddCriteria(e => e.IsPublic == isPublic);
            return this;
        }

        public EventSpecification ByCreator(int creatorId)
        {
            AddCriteria(e => e.CreatorId == creatorId);
            return this;
        }

        public EventSpecification ByStartDateFrom(DateTime fromDate)
        {
            AddCriteria(e => e.StartDateTime >= fromDate);
            return this;
        }

        public EventSpecification ByStartDateTo(DateTime toDate)
        {
            AddCriteria(e => e.StartDateTime <= toDate);
            return this;
        }

        public EventSpecification ByNotCancelled()
        {
            AddCriteria(e => e.IsCancelled == false);
            return this;
        }

        public EventSpecification ByEventType(int eventTypeId)
        {
            AddCriteria(e => e.EventActivities.Any(a => a.ActivityTypeId == eventTypeId));
            return this;
        }

        // No-op to keep compatibility with callers; ordering not implemented in Repository
        public EventSpecification OrderByStartDateTime()
        {
            return this;
        }

        // No-op to keep compatibility with callers; ordering not implemented in Repository
        public EventSpecification OrderByPopularity()
        {
            return this;
        }

        public EventSpecification IncludeEventStages()
        {
            AddInclude(e => e.Stages);
            return this;
        }

        public EventSpecification IncludeRequiredItems()
        {
            AddInclude(e => e.RequiredItems);
            return this;
        }

        public EventSpecification IncludeEventActivities()
        {
            AddInclude(e => e.EventActivities);
            return this;
        }

        public EventSpecification IncludeEventParticipants()
        {
            AddInclude(e => e.Participants);
            return this;
        }
    }

    public class EventParticipantSpecification : Specification<EventParticipant>
    {
        public EventParticipantSpecification() : base()
        {
        }

        public EventParticipantSpecification ByEventId(int eventId)
        {
            AddCriteria(p => p.EventId == eventId);
            return this;
        }

        public EventParticipantSpecification ByPersonId(int personId)
        {
            AddCriteria(p => p.PersonId == personId);
            return this;
        }

        public EventParticipantSpecification ByApprovalStatus(bool isApproved)
        {
            AddCriteria(p => p.IsApproved == isApproved);
            return this;
        }

        public EventParticipantSpecification OrderByJoinDate()
        {
            return this;
        }

        public EventParticipantSpecification IncludePerson()
        {
            AddInclude(p => p.Person);
            return this;
        }
    }
}