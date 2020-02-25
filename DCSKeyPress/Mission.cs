namespace DCSKeyPress
{
    public class MissionData
    {
        public Airfield[] Airfields { get; set; }

        public Flight[] Flight { get; set; }

        public FlightComm[] FlightComms { get; set; }

        public Support[] Support { get; set; }

        public object[] Pkg { get; set; }

        public Waypoint[] Waypoints { get; set; }

        public Poi[] Poi { get; set; }

        public object[] Loadout { get; set; }

        public string[][] Notes { get; set; }

        public string[] Ramrod { get; set; }

        public Settings Settings { get; set; }

        public Mission Mission { get; set; }

        public Theme Theme { get; set; }
    }

    public partial class Waypoint
    {
        public string Desc { get; set; }

        public string Wp { get; set; }

        public long Alt { get; set; }

        public string Agl { get; set; }

        public string Speedtype { get; set; }

        public string Gs { get; set; }

        public string Tas { get; set; }

        public string Cas { get; set; }

        public string Mach { get; set; }

        public double Tot { get; set; }

        public long Act { get; set; }

        public string Lat { get; set; }

        public string Lon { get; set; }

        public long Dist { get; set; }

        public long Brg { get; set; }
    }

    public partial class Airfield
    {
        public string Type { get; set; }

        public string Airbase { get; set; }

        public string Icao { get; set; }

        public string Tcn { get; set; }

        public string Atis { get; set; }

        public string Gnd { get; set; }

        public string Twr { get; set; }

        public string Par { get; set; }

        public string Ctrl { get; set; }

        public string Elev { get; set; }

        public string Rwy { get; set; }

        public TableData TableData { get; set; }
    }

    public partial class TableData
    {
        public long Id { get; set; }
    }

    public partial class Flight
    {
        public string Desc { get; set; }

        public string Callsign { get; set; }

        public string Tcn { get; set; }

        public long Laser { get; set; }

        public long Mode { get; set; }

        public TableData TableData { get; set; }

        public long? Index { get; set; }
    }

    public partial class FlightComm
    {
        public string Type { get; set; }

        public string Chan { get; set; }

        public string Freq { get; set; }

        public TableData TableData { get; set; }
    }

    public partial class Mission
    {
        public string Desc { get; set; }

        public string Callsign { get; set; }

        public string PackageName { get; set; }

        public string Number { get; set; }

        public TableData TableData { get; set; }
    }

    public partial class Poi
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Lat { get; set; }

        public string Lon { get; set; }
    }

    public partial class Settings
    {
        public string CoordFormat { get; set; }

        public long DdPrecision { get; set; }

        public long DmsPrecision { get; set; }

        public long TransitionAlt { get; set; }

        public long MissionStart { get; set; }

        public string Theatre { get; set; }
    }

    public partial class Support
    {
        public string Agency { get; set; }

        public string Chan { get; set; }

        public string Freq { get; set; }

        public string Tcn { get; set; }

        public string Notes { get; set; }

        public TableData TableData { get; set; }
    }

    public partial class Theme
    {
        public string Mode { get; set; }

        public string Template { get; set; }
    }
}
