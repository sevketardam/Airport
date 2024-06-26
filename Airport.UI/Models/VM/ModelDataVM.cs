namespace Airport.UI.Models.VM;

public class TableEntry
{
    public int id { get; set; }
    public string name { get; set; }
    public int? start_production_year { get; set; }
    public int? end_production_year { get; set; }
    public int model_id { get; set; }
    public int serie_id { get; set; }
    public string? created_at { get; set; }
    public string? updated_at { get; set; }
}
