using System;
using System.Collections.Generic;
using System.Text;

namespace MusicPortal.DAL.Entities
{
    public class Artist
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Biography { get; set; }
        public string? Country { get; set; }
        public string? PhotoUrl { get; set; }

        public ICollection<Record> MusicalRecords { get; set; }
            = new List<Record>();
    }
}
