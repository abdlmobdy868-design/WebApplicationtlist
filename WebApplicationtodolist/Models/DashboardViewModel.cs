using System;

namespace WebApplicationtodolist.Models
{
    public class DashboardViewModel
    {
        public int TotalTasks { get; set; }
        public int OverdueTasks { get; set; }
        public int DueSoonTasks { get; set; }
        public int WithFiles { get; set; }
    }
}
