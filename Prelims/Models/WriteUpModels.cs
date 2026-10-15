using System;
using System.Collections.Generic;

namespace Prelims.Models
{
    public class WriteUpContainer
    {
        public WriteUp WriteUp { get; set; }
    }

    public class WriteUp
    {
        public string PlantDate { get; set; }
        public string OrderNo { get; set; }
        public string PropertyAddress { get; set; }
        public string Customer { get; set; }
        public int CrnId { get; set; }
        public List<WriteupApn> Apns { get; set; }
        public string LegalCity { get; set; }
        public string Examiner { get; set; }
        public string CountOf { get; set; }
        public string ZipCode { get; set; }
        public string EstateType { get; set; }
        public string Fee { get; set; }
        public string Easement { get; set; }
        public string EasementDescription { get; set; }

        public string Vesting { get; set; }
        public string SubjectTo { get; set; }
        public string PolicyType { get; set; }
        public string LegalDesc { get; set; }
        public string PropertyType { get; set; }
        public string PlottedEasement { get; set; }
        public List<WriteupException> Exceptions1 { get; set; }
        public List<WriteupStarter> Starters1 { get; set; }
        public List<WriteupPigi> Pigis1 { get; set; }
        public List<WriteupTax> Taxs1 { get; set; }
        public List<WriteupRequirement> Requirements { get; set; }
        public List<WriteupMtcMonthDeed> MtcMonthDeeds { get; set; }
        public List<WriteupNote> Notes { get; set; }
        public string SpecialInstruction { get; set; }
        public string ClientInstructions { get; set; }
        public List<string> CwltPolicyTypes { get; set; }
        public string EstatePhrase { get; set; }
        public string LenderSuplemental { get; set; }
        public string MtcPropertyType { get; set; }
        public string TwentyFourMonthsDeed { get; set; }
        public string TwentyFourMonthsDeedRecordingDate { get; set; }
        public string TwentyFourMonthsDeedInstrumentNumber { get; set; }
    }

    public class WriteupMtcMonthDeed
    {
        public int Id { get; set; }
        public int SlNo { get; set; }
        public string InstrumentNo { get; set; }
        public string RecordingDate { get; set; }
    }

    public class WriteupApn
    {
        public long Id { get; set; }
        public string Apn { get; set; }
        public string hashkey { get; set; }
    }

    public class WriteupException
    {
        public int Id { get; set; }
        public int SlNo { get; set; }
        public bool IsSubItem { get; set; }
        public string MtcSlNo { get; set; }
        public string Code { get; set; }
        public string InstrumentNo { get; set; }
        public string ItemNo { get; set; }
        public string RecordingDate { get; set; }
        public string Comments { get; set; }
        public string hashkey { get; set; }
    }

    public class WriteupStarter
    {
        public int Id { get; set; }
        public int? SlNo { get; set; }
        public bool IsSubItem { get; set; }
        public string MtcSlNo { get; set; }
        public string Code { get; set; }
        public string InstrumentNo { get; set; }
        public string ItemNo { get; set; }
        public string RecordingDate { get; set; }
        public string Comments { get; set; }
        public string hashkey { get; set; }
    }

    public class WriteupPigi
    {
        public int Id { get; set; }
        public int SlNo { get; set; }
        public bool IsSubItem { get; set; }
        public string MtcSlNo { get; set; }
        public string Code { get; set; }
        public string InstrumentNo { get; set; }
        public string ItemNo { get; set; }
        public string RecordingDate { get; set; }
        public string Comments { get; set; }
        public string hashkey { get; set; }
    }

    public class WriteupTax
    {
        public int Id { get; set; }
        public int SlNo { get; set; }
        public bool IsSubItem { get; set; }
        public string MtcSlNo { get; set; }
        public string Code { get; set; }
        public string InstrumentNo { get; set; }
        public string ItemNo { get; set; }
        public string RecordingDate { get; set; }
        public string Comments { get; set; }
        public string hashkey { get; set; }
    }

    public class WriteupRequirement
    {
        public int? Id { get; set; }
        public int SlNo { get; set; }
        public bool IsSubItem { get; set; }
        public string MtcSlNo { get; set; }
        public string Code { get; set; }
        public string PageNo { get; set; }
        public string ItemNo { get; set; }
        public string hashkey { get; set; }
    }

    public class WriteupNote
    {
        public int Id { get; set; }
        public int SlNo { get; set; }
        public bool IsSubItem { get; set; }
        public string MtcSlNo { get; set; }
        public string Code { get; set; }
        public string PageNo { get; set; }
        public string ItemNo { get; set; }
        public string hashkey { get; set; }
    }

    public class TitleOrderMirrorDto
    {
        public int Id { get; set; }
        public string OrderNo { get; set; }
        public string SearchPackageOneDriveLink { get; set; }
        public string WorkSheetJson { get; set; }
        public int CrnId { get; set; }
    }
}
