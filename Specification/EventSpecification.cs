using System;
using System.Linq.Expressions;

namespace motoMeet
{
    public class EventSpecification : BaseSpecification<Event>
    {
        public EventSpecification() : base(e => true)
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

        public EventSpecification OrderByStartDateTime()
        {
            ApplyOrderBy(e => e.StartDateTime);
            return this;
        }

        public EventSpecification OrderByPopularity()
        {
            ApplyOrderByDescending(e => e.EventParticipants.Count);
            return this;
        }

        public EventSpecification IncludeEventStages()
        {
            AddInclude(e => e.EventStages);
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
            AddInclude(e => e.EventParticipants);
            return this;
        }
    }

    public class EventParticipantSpecification : BaseSpecification<EventParticipant>
    {
        public EventParticipantSpecification() : base(p => true)
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
            ApplyOrderBy(p => p.JoinedOn);
            return this;
        }

        public EventParticipantSpecification IncludePerson()
        {
            AddInclude(p => p.Person);
            return this;
        }
    }
}