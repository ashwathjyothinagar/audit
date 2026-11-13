using System;
using System.Collections.Generic;

namespace Prelims.Models.Reports
{
    public class ProductionMetricsViewModel
    {
        public List<ProductionMetricsUserRow> UserRows { get; set; }
        public List<string> DynamicTaskColumns { get; set; }

        public ProductionMetricsViewModel()
        {
            UserRows = new List<ProductionMetricsUserRow>();
            DynamicTaskColumns = new List<string>();
        }
    }

    public class ProductionMetricsUserRow
    {
        public int SerialNumber { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Location { get; set; }
        public string AchievedPercentDisplay { get; set; }
        public decimal AchievedPercent { get; set; }

        // Key is TaskName, Value is details about the task output
        public Dictionary<string, ProductionMetricsTaskDetail> TaskCounts { get; set; }

        public ProductionMetricsUserRow()
        {
            TaskCounts = new Dictionary<string, ProductionMetricsTaskDetail>();
        }
    }

    public class ProductionMetricsTaskDetail
    {
        public int Count { get; set; }
        // List of orders with office name to display in a modal if clicked
        public List<OrderWithOffice> OrderNumbers { get; set; }

        public ProductionMetricsTaskDetail()
        {
            OrderNumbers = new List<OrderWithOffice>();
        }
    }

    public class OrderWithOffice
    {
        public string OrderNumber { get; set; }
        public string OfficeName { get; set; }
        public string ProductType { get; set; }
    }

    public class ProductionMetricItem
    {
        public string CrnName { get; set; }
        public string TaskName { get; set; }
        public int TargetMinutes { get; set; }
        public int TimePerFile { get; set; }
        public int TimeTakenMinutes { get; set; }
        public decimal ExpectedOutput { get; set; }
    }
}
