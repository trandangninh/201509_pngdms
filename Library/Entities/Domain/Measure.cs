using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Domain.Users;
using RepositoryPattern;

namespace Entities.Domain
{
    public class Measure : BaseEntity
    {
        private ICollection<User> _users;
        public string MeasureName { get; set; }
        public string MeasureCode { get; set; }
        public string Target { get; set; }
        public string Unit { get; set; }
        public string Note { get; set; }
        public int MeasureSystemTypeId { get; set; }
        public MeasureSystemType MeasureSystemType
        {
            get { return (MeasureSystemType)MeasureSystemTypeId; }
            set { MeasureSystemTypeId = (int)value; }
        }
        public int MeasureTypeId { get; set; }
        public MeasureType MeasureType
        {
            get { return (MeasureType) MeasureTypeId; }
            set { MeasureTypeId = (int) value; }
        }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int DmsId { get; set; }
        public virtual Dms Dms { get; set; }
        public bool Active { get; set; }
        public int Order { get; set; }

        public virtual ICollection<User> Users
        {
            get { return _users ?? (_users = new List<User>()); }
            protected set { _users = value; }
        }
    }

    public enum MeasureType
    {
        IP = 1,
        OP = 2,
        IPorOP = 3,
        Null = 4
    }

    public enum MeasureSystemType
    {
        Normal = 0,
        ClScrap = 1,
        ClAmount = 2,
        ClScrapPerAmount = 3,
        PoMissed = 4,
        Mpsa = 5,
        QuanlityAlert = 6,
        TotalPo = 7,
        Pr = 8,
        BosComplete = 9,
        BosUnsafe = 10,
        PrMkLastDay = 11,
        PrMkMtd = 12,
        PrPkLastDay = 13,
        PrPkMtd = 14,
        DeedmacOperation = 15
    }
}
