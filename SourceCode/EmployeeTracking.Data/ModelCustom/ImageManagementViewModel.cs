﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmployeeTracking.Data.ModelCustom
{
    public class ImageManagementViewModel
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public Guid MasterStoreId { get; set; }
        public string MasterStoreName { get; set; }
        public string Region { get; set; }
        public DateTime CreateDate { get; set; }
        public bool? StoreStatus { get; set; }
        public IEnumerable<TrackSessionViewModel> TrackSessions { get; set; }
    }

    public class TrackSessionViewModel
    {
        public string Id { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class StoreInfoViewModel
    {
        public string Id { get; set; }
        public string SbvpName { get; set; }
        public string SbvpType { get; set; }
        public string SbvpProvince { get; set; }
        public string SbvpDistrict { get; set; }
        public string SbvpWard { get; set; }
        public string SbvpHouseNumber { get; set; }
        public string SbvpStreetName { get; set; }

        public string DigixName { get; set; }
        public string DigixType { get; set; }
        public string DigixProvince { get; set; }
        public string DigixDistrict { get; set; }
        public string DigixWard { get; set; }
        public string DigixHouseNumber { get; set; }
        public string DigixStreetName { get; set; }

    }

    public class TrackDetailViewModel
    {
        public string MediaTypeId { get; set; }
        public string MediaTypeName { get; set; }
        public sbyte MediaTypeOrder { get; set; }
        public IEnumerable<TrackDetailImageViewModel> TrackDetailImages { get; set; }
    }

    public class TrackDetailImageViewModel
    {
        public string Id { get; set; }
        public string FileName { get; set; }
        public string Url { get; set; }
        public int PosmNumber { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class ChangeDetailImageViewModel
    {
        [Required]
        public string SessionDetailId { get; set; }
        public string UrlFile { get; set; }
    }

    public class TrackExcelViewModel
    {
        public string Id { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public Guid MasterStoreId { get; set; }
        public string Region { get; set; }
        public DateTime CreateDate { get; set; }
        public string Note { get; set; }
        public bool? StoreStatus { get; set; }

        public string SbvpName { get; set; }
        public string SbvpType { get; set; }
        public string SbvpProvince { get; set; }
        public string SbvpDistrict { get; set; }
        public string SbvpWard { get; set; }
        public string SbvpHouseNumber { get; set; }
        public string SbvpStreetName { get; set; }

        public string DigixName { get; set; }
        public string DigixType { get; set; }
        public string DigixProvince { get; set; }
        public string DigixDistrict { get; set; }
        public string DigixWard { get; set; }
        public string DigixHouseNumber { get; set; }
        public string DigixStreetName { get; set; }
    }
}