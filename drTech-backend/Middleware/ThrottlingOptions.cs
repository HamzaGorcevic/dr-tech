namespace drTech_backend.Middleware
{
    public class ThrottlingOptions
    {
        public int Limit { get; set; } = 100;
        public int Window { get; set; } = 10;
        public int BlockDuration { get; set; } = 5;
        public string StrictPaths { get; set; } = string.Empty;
        public int StrictLimit { get; set; } = 10;
        public int StrictWindow { get; set; } = 1;
    }
}
