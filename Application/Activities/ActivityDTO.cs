using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Profiles;

namespace Application.Activities
{
    public class ActivityDTO
    {
         public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime Date {get; set;}
        public string Category {get; set;}
        public string City {get; set;}
         public string Venue {get; set;}
         public string Description {get; set;} 
         public string HostUserName {get; set;}
          public bool IsCancelled {get; set;}
         public ICollection<AttendeeDTO> Attendees {get; set;}
    }
}