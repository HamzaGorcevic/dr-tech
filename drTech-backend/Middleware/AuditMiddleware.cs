namespace drTech_backend.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;

        public AuditMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, Infrastructure.AppDbContext db)
        {
            await _next(context);
            if (HttpMethods.IsPost(context.Request.Method) || HttpMethods.IsPut(context.Request.Method) || HttpMethods.IsDelete(context.Request.Method))
            {
                var entry = new Domain.Entities.AuditLog
                {
                    Id = Guid.NewGuid(),
                    Actor = context.User?.Identity?.Name ?? "anonymous",
                    Action = context.Request.Method,
                    Path = context.Request.Path,
                    Method = context.Request.Method,
                    StatusCode = context.Response.StatusCode,
                    OccurredAtUtc = DateTime.UtcNow
                };
                db.AuditLogs.Add(entry);
                await db.SaveChangesAsync();
            }
        }
    }
}


