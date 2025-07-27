# MotoMeet Data Classes Documentation

This document provides a comprehensive overview of all data classes in the MotoMeet project, organized by their purpose and location.

## Table of Contents
- [Entity Framework Classes](#entity-framework-classes)
- [Data Transfer Objects (DTOs)](#data-transfer-objects-dtos)
- [Request Models](#request-models)
- [Other Data Models](#other-data-models)
- [Enums](#enums)

---

## Entity Framework Classes

### Core Entities

#### `EntityBase` - `Entities/EntityBase.cs`
**Interface:** `IEntity`
- `Id` (int) - Primary key

#### `Person` - `Entities/User.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `Username` (string?)
- `FirstName` (string?)
- `LastName` (string?)
- `PhoneNumber` (string?)
- `AddedOn` (DateTime?)
- `EditOn` (DateTime?)
- `ProfilePictureUrl` (string?)
- `Bio` (string?)
- `Address` (string?)
- `Email` (string?)
- `PasswordHash` (string?)
- `VerificationToken` (string?)
- `VerificationTokenExpiration` (DateTime?)
- `IsVerified` (bool?)
- `CountryId` (int)
- `TotalDistance` (double)

**Navigation Properties:**
- `UserRoutes` (ICollection<UserRoute>)
- `Followers` (ICollection<PersonFollow>)
- `Following` (ICollection<PersonFollow>)
- `GroupMemberships` (ICollection<GroupMember>)
- `CreatedRoutes` (ICollection<Route>)
- `CreatedEvents` (ICollection<Event>)
- `NotificationsReceived` (ICollection<Notification>)
- `Reactions` (ICollection<Reaction>)
- `Favorites` (ICollection<Favorite>)
- `PointsOfInterest` (ICollection<PointOfInterest>)

### Route-Related Entities

#### `Route` - `Entities/Route.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `PersonId` (int) - Foreign key
- `Name` (string)
- `Description` (string?)
- `StartDate` (DateTime?)
- `EndDate` (DateTime?)
- `StartPoint` (Point) - NetTopologySuite geometry
- `EndPoint` (Point) - NetTopologySuite geometry
- `RouteTypeId` (int?)
- `DifficultyLevelId` (int?)
- `Length` (double)
- `Duration` (TimeSpan)
- `ElevationGain` (double)
- `Rating` (double)
- `isLoop` (bool)
- `Country` (string?)
- `Region` (string?)

**Navigation Properties:**
- `Person` (Person)
- `RouteType` (RouteType)
- `DifficultyLevel` (DifficultyLevel)
- `RoutePoints` (ICollection<RoutePoint>)
- `Reviews` (ICollection<Review>)
- `Tags` (ICollection<Tag>)
- `UserRoutes` (ICollection<UserRoute>)
- `PointsOfInterest` (ICollection<PointOfInterest>)

#### `UserRoute` - `Entities/Route.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `PersonId` (int) - Foreign key
- `RouteId` (int) - Foreign key
- `DifficultyLevelId` (int?)
- `RouteTypeId` (int?)
- `DateTraveled` (DateTime)
- `Duration` (TimeSpan?)
- `Distance` (double?)
- `ElevationGain` (double?)
- `EventStageParticipantId` (int?)

**Navigation Properties:**
- `Person` (Person)
- `Route` (Route)
- `DifficultyLevel` (DifficultyLevel)
- `RouteType` (RouteType)
- `EventStageParticipant` (EventStageParticipant)
- `UserRoutePoints` (ICollection<UserRoutePoint>)

#### `RoutePoint` - `Entities/Route.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `RouteId` (int) - Foreign key
- `SequenceNumber` (int)
- `Point` (Point) - NetTopologySuite geometry

**Navigation Properties:**
- `Route` (Route)

#### `UserRoutePoint` - `Entities/Route.cs`
- `Id` (int) - Primary key
- `UserRouteId` (int) - Foreign key
- `SequenceNumber` (int)
- `Point` (Point) - NetTopologySuite geometry

**Navigation Properties:**
- `UserRoute` (UserRoute)

#### `PointOfInterest` - `Entities/Route.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `RouteId` (int) - Foreign key
- `PersonId` (int?)
- `Location` (Point) - NetTopologySuite geometry
- `ImageUrl` (string)
- `Name` (string)
- `Description` (string)
- `WaypointType` (WaypointType) - Enum

**Navigation Properties:**
- `Route` (Route)
- `Person` (Person)

#### `Review` - `Entities/Route.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `Username` (string)
- `Rating` (double)
- `Comment` (string)
- `Date` (DateTime)
- `RouteId` (int) - Foreign key

**Navigation Properties:**
- `Route` (Route)

### Event-Related Entities

#### `Event` - `Entities/Event.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `Name` (string)
- `Description` (string?)
- `IsPublic` (bool)
- `RequiresApproval` (bool)
- `IsCancelled` (bool)
- `CreatorId` (int) - Foreign key
- `StartDateTime` (DateTime)
- `EndDateTime` (DateTime?)
- `GroupId` (int?)

**Navigation Properties:**
- `Creator` (Person)
- `Group` (Group)
- `RequiredItems` (ICollection<EventItem>)
- `Participants` (ICollection<EventParticipant>)
- `Stages` (ICollection<EventStage>)
- `EventActivities` (ICollection<EventActivity>)

#### `EventStage` - `Entities/Event.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `EventId` (int) - Foreign key
- `Title` (string)
- `Description` (string?)
- `StageStartTime` (DateTime?)
- `StageEndTime` (DateTime?)
- `RouteId` (int?)
- `StageType` (EventStageType) - Enum
- `Location` (Point?) - NetTopologySuite geometry

**Navigation Properties:**
- `Event` (Event)
- `Route` (Route)
- `StageParticipants` (ICollection<EventStageParticipant>)

#### `EventParticipant` - `Entities/Event.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `EventId` (int) - Foreign key
- `PersonId` (int) - Foreign key
- `IsApproved` (bool)
- `IsActive` (bool)
- `JoinedOn` (DateTime)

**Navigation Properties:**
- `Event` (Event)
- `Person` (Person)
- `StageParticipants` (ICollection<EventStageParticipant>)

#### `EventStageParticipant` - `Entities/Event.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `EventStageId` (int) - Foreign key
- `EventParticipantId` (int) - Foreign key
- `UserRouteId` (int?)
- `StartedAt` (DateTime?)
- `FinishedAt` (DateTime?)
- `IsCompleted` (bool)

**Navigation Properties:**
- `EventStage` (EventStage)
- `EventParticipant` (EventParticipant)
- `UserRoute` (UserRoute)

#### `EventItem` - `Entities/Event.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `EventId` (int) - Foreign key
- `ItemName` (string)
- `Description` (string?)
- `IsAssigned` (bool)

**Navigation Properties:**
- `Event` (Event)

### Group-Related Entities

#### `Group` - `Entities/Event.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `Name` (string)
- `Description` (string?)
- `Location` (string?)
- `IsPublic` (bool)
- `IsApprovalRequired` (bool)
- `CreatorId` (int) - Foreign key

**Navigation Properties:**
- `Creator` (Person)
- `Members` (ICollection<GroupMember>)
- `Posts` (ICollection<GroupPost>)
- `Events` (ICollection<Event>)
- `GroupActivities` (ICollection<GroupActivity>)

#### `GroupMember` - `Entities/Event.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `GroupId` (int) - Foreign key
- `PersonId` (int) - Foreign key
- `IsAdmin` (bool)
- `CanPost` (bool)
- `IsApproved` (bool)
- `JoinedOn` (DateTime)

**Navigation Properties:**
- `Group` (Group)
- `Person` (Person)

#### `GroupPost` - `Entities/Event.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `GroupId` (int) - Foreign key
- `AuthorId` (int) - Foreign key
- `Content` (string)
- `CreatedOn` (DateTime)

**Navigation Properties:**
- `Group` (Group)
- `Author` (Person)
- `Attachments` (ICollection<GroupPostAttachment>)
- `Comments` (ICollection<GroupPostComment>)
- `Reactions` (ICollection<Reaction>) - [NotMapped]

#### `GroupPostComment` - `Entities/Event.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `GroupPostId` (int) - Foreign key
- `AuthorId` (int) - Foreign key
- `Content` (string)
- `CreatedOn` (DateTime)

**Navigation Properties:**
- `GroupPost` (GroupPost)
- `Author` (Person)

#### `GroupPostAttachment` - `Entities/Event.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `GroupPostId` (int) - Foreign key
- `FileUrl` (string)
- `AttachmentType` (string)
- `UploadedOn` (DateTime)

**Navigation Properties:**
- `GroupPost` (GroupPost)

### Social Features Entities

#### `PersonFollow` - `Entities/Event.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `FollowerId` (int) - Foreign key
- `FollowingId` (int) - Foreign key
- `FollowedOn` (DateTime)

**Navigation Properties:**
- `Follower` (Person)
- `Following` (Person)

#### `Notification` - `Entities/Notification.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `RecipientId` (int) - Foreign key
- `ActorId` (int) - Foreign key
- `NotificationType` (NotificationType) - Enum
- `Message` (string?)
- `IsRead` (bool)
- `CreatedOn` (DateTime)
- `ReadOn` (DateTime?)
- `TargetEntityType` (string?)
- `TargetEntityId` (int?)

**Navigation Properties:**
- `Recipient` (Person)
- `Actor` (Person)

#### `Reaction` - `Entities/Notification.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `PersonId` (int) - Foreign key
- `ReactionType` (ReactionType) - Enum
- `CreatedOn` (DateTime)
- `TargetEntityType` (string)
- `TargetEntityId` (int)

**Navigation Properties:**
- `Person` (Person)

#### `Favorite` - `Entities/Notification.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `PersonId` (int) - Foreign key
- `TargetEntityType` (string)
- `TargetEntityId` (int)
- `CreatedOn` (DateTime)

**Navigation Properties:**
- `Person` (Person)

### Lookup/Reference Entities

#### `ActivityType` - `Entities/Event.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `Name` (string)

**Navigation Properties:**
- `EventActivities` (ICollection<EventActivity>)
- `GroupActivities` (ICollection<GroupActivity>)

#### `DifficultyLevel` - `Entities/Route.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `Level` (string)
- `Description` (string)

**Navigation Properties:**
- `OfficialRoutes` (ICollection<Route>)
- `UserRoutes` (ICollection<UserRoute>)

#### `RouteType` - `Entities/Route.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `Name` (string)

**Navigation Properties:**
- `OfficialRoutes` (ICollection<Route>)
- `UserRoutes` (ICollection<UserRoute>)

#### `Tag` - `Entities/Route.cs`
**Inherits:** `EntityBase`
- `Id` (int) - Primary key
- `Name` (string)

**Navigation Properties:**
- `Routes` (ICollection<Route>)

### Junction/Many-to-Many Entities

#### `EventActivity` - `Entities/Event.cs`
**Composite Primary Key:** `(EventId, ActivityTypeId)`
- `EventId` (int) - Foreign key
- `ActivityTypeId` (int) - Foreign key

**Navigation Properties:**
- `Event` (Event)
- `ActivityType` (ActivityType)

#### `GroupActivity` - `Entities/Event.cs`
**Composite Primary Key:** `(GroupId, ActivityTypeId)`
- `GroupId` (int) - Foreign key
- `ActivityTypeId` (int) - Foreign key

**Navigation Properties:**
- `Group` (Group)
- `ActivityType` (ActivityType)

---

## Data Transfer Objects (DTOs)

### `UserDto` - `Entities/Dto.cs`
- `Id` (int)
- `Email` (string?)
- `Username` (string?)
- `FirstName` (string?)
- `LastName` (string?)
- `PhoneNumber` (string?)
- `AddedOn` (DateTime?)
- `ProfilePictureUrl` (string?)
- `Bio` (string?)
- `Address` (string?)
- `TotalDistance` (double)
- `Token` (string?)
- `UserRoutes` (List<RouteDto>)
- `ParticipatedEvents` (List<EventDto>)
- `Groups` (List<GroupDto>)
- `CreatedRoutes` (List<RouteDto>)
- `CreatedEvents` (List<EventDto>)

### `RouteDto` - `Entities/Dto.cs`
- `Id` (int)
- `Name` (string?)
- `Description` (string?)
- `Length` (double)
- `Rating` (double)
- `IsLoop` (bool)

### `EventDto` - `Entities/Dto.cs`
- `Id` (int)
- `Name` (string?)
- `Description` (string?)
- `StartDateTime` (DateTime)
- `EndDateTime` (DateTime?)
- `IsPublic` (bool)

### `GroupDto` - `Entities/Dto.cs`
- `Id` (int)
- `Name` (string?)
- `Description` (string?)

### `UserDto` (Duplicate in User.cs) - `Entities/User.cs`
- `Id` (int)
- `Username` (string?)
- `FirstName` (string?)
- `LastName` (string?)
- `Email` (string?)
- `PhoneNumber` (string?)
- `Bio` (string?)
- `Address` (string?)
- `CountryId` (int?)
- `Token` (string?)

---

## Request Models

### Route Requests - `Entities/Request.cs`

#### `CreateRouteRequest`
- `Name` (string)
- `Description` (string)
- `CreatorId` (int)
- `StartPoint` (GeoPoint)
- `EndPoint` (GeoPoint)
- `RoutePoints` (List<GeoPoint>)
- `Length` (double)
- `Duration` (TimeSpan)
- `RouteTagsIds` (List<int>)
- `DifficultyLevelId` (int?)
- `RouteTypeId` (int?)

#### `CreateUserRouteRequest`
- `PersonId` (int)
- `RouteId` (int)
- `DateTraveled` (DateTime)
- `Duration` (TimeSpan?)
- `Distance` (double?)
- `ElevationGain` (double?)
- `DifficultyLevelId` (int?)
- `RouteTypeId` (int?)
- `UserPoints` (List<GeoPoint>)

### Event Requests - `Entities/Request.cs`

#### `CreateEventRequest`
- `Name` (string) - [Required]
- `Description` (string)
- `IsPublic` (bool)
- `RequiresApproval` (bool)
- `CreatorId` (int) - [Required]
- `StartDateTime` (DateTime)
- `EndDateTime` (DateTime?)
- `GroupId` (int?)
- `Stages` (List<CreateEventStageRequest>)
- `RequiredItems` (List<CreateRequiredItemRequest>)
- `EventActivities` (List<CreateEventActivityRequest>)

#### `CreateEventStageRequest`
- `Title` (string) - [Required]
- `Description` (string)
- `StageStartTime` (DateTime?)
- `RouteId` (int?)
- `StageType` (string)
- `Latitude` (double?)
- `Longitude` (double?)
- `Altitude` (double?)

#### `CreateRequiredItemRequest`
- `ItemName` (string) - [Required]
- `Description` (string)
- `IsRequired` (bool)

#### `CreateEventActivityRequest`
- `ActivityTypeId` (int) - [Required]

#### `JoinEventRequest`
- `PersonId` (int) - [Required]

#### `StageParticipationRequest`
- `EventId` (int)
- `StageId` (int)
- `PersonId` (int)

#### `JoinStageRequest`
- `PersonId` (int) - [Required]
- `StartedAt` (DateTime?)

#### `UpdateEventRequest`
- `RequestingUserId` (int) - [Required]
- `Name` (string)
- `Description` (string)
- `IsPublic` (bool?)
- `RequiresApproval` (bool?)
- `StartDateTime` (DateTime?)
- `EndDateTime` (DateTime?)

#### `EventFilterRequest`
- `IsPublic` (bool?)
- `FromDate` (DateTime?)
- `ToDate` (DateTime?)
- `CreatorId` (int?)
- `EventTypeId` (int?)
- `IncludeCancelled` (bool?)

#### `ApproveParticipantRequest`
- `ParticipantId` (int) - [Required]

---

## Other Data Models

### `RegistrateModel` - `Entities/User.cs`
- `Username` (string?)
- `FirstName` (string?)
- `LastName` (string?)
- `PhoneNumber` (string?)
- `ProfilePictureUrl` (string?)
- `Bio` (string?)
- `Address` (string?)
- `Email` (string?)
- `Password` (string?)
- `CountryId` (int)

### `LoginModel` - `Models/LoginModel.cs`
- `Email` (string)
- `Password` (string)

### `NewRouteModel` - `Entities/Route.cs`
- `Name` (string)
- `Description` (string)
- `AddedBy` (int)
- `StartPointArray` (GeoPoint)
- `StartPoint` (Point?) - Computed property
- `EndPointArray` (GeoPoint)
- `RoutePointsArray` (GeoPoint[])
- `EndPoint` (Point) - Computed property
- `RouteType` (RouteType)
- `Length` (double)
- `Duration` (TimeSpan)
- `StartDate` (DateTime)
- `EndDate` (DateTime?)

### `GeoPoint` - `Entities/Route.cs`
- `latitude` (double)
- `longitude` (double)
- `altitude` (double?)

### `WeatherForecast` - `WeatherForecast.cs`
- `Date` (DateTime)
- `TemperatureC` (int)
- `TemperatureF` (int) - Computed property
- `Summary` (string?)

### `MotoMeetDbContext` - `Entities/User.cs`
**Inherits:** `DbContext`
Contains all DbSet properties for Entity Framework entities and model configuration.

---

## Enums

### `EventStageType` - `Entities/Event.cs`
- `RouteSegment`
- `MeetingPoint`
- `OvernightStop`
- `LunchStop`
- `Activity`

### `NotificationType` - `Entities/Notification.cs`
- `Follow`
- `Unfollow`
- `Like`
- `Comment`
- `Mention`
- `JoinedEvent`
- `JoinedGroup`

### `ReactionType` - `Entities/Notification.cs`
- `Like`
- `Love`
- `Haha`
- `Wow`
- `Sad`
- `Angry`

### `WaypointType` - `Entities/Route.cs`
**Natural:**
- `Lake`, `Cliff`, `Waterfall`, `WaterSpring`, `River`, `MountainPeak`, `Forest`, `Meadow`, `Cave`, `Valley`, `Beach`, `Glacier`, `Volcano`

**Informative:**
- `HistoricalSite`, `VisitorCenter`, `Viewpoint`, `Museum`, `CulturalSite`, `EducationalTrail`, `ParkOffice`

**Warnings:**
- `SteepDrop`, `SlipperyPath`, `HighTide`, `WildlifeSighting`, `FloodingArea`, `Rockfall`, `RestrictedArea`

---

## Notes

- All Entity Framework classes inherit from `EntityBase` which provides the common `Id` property
- The project uses NetTopologySuite for geographic data types (`Point`, `geography` column type)
- Many entities use polymorphic references (e.g., `TargetEntityType` and `TargetEntityId`) for flexible relationships
- The `MotoMeetDbContext` contains comprehensive Entity Framework configuration including relationship mappings, composite keys, and geographic type configurations
- Some properties are marked as `[NotMapped]` indicating they are computed properties not stored in the database