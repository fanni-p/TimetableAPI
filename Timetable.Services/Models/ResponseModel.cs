using System;
using System.Runtime.Serialization;

namespace Timetable.Services.Models
{
    [DataContract]
    public class ResponseModel
    {
        [DataMember(Name="id")]
        public int Id { get; set; }
    }
}