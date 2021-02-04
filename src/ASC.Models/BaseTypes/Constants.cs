using System;
using System.Collections.Generic;
using System.Text;

namespace ASC.Models.BaseTypes
{
    /// <summary>
    /// Having a Constants class always helps application development to centralize the commonly used
    /// strings and other settings.A developer should be aware of the practice that all the environment-related
    /// settings should go into appsettings.json, and any setting related to C# code should go into the Constants class.
    /// </summary>
    public static class Constants
    {
        public const string Equal = "eq";
        public const string NotEqual = "ne";
        public const string GreaterThan = "gt";
        public const string GreaterThanOrEqual = "ge";
        public const string LessThan = "lt";
        public const string LessThanOrEqual = "le";
    }

    /// <summary>
    /// https://www.dotnetperls.com/enum-flags
    /// </summary>
    [Flags]
    public enum Roles
    {
        None = 0,
        Admin = 1,
        Engineer = 2,
        User = 4
    }

    public enum MasterKeys
    {
        VehicleName,
        VehicleType
    }

    public enum Status
    {
        New,
        Denied,
        Pending,
        Initiated,
        InProgress,
        PendingCustomerApproval,
        RequestForInformation,
        Completed
    }

}
