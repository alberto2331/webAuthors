using Microsoft.AspNetCore.Mvc.Filters;

namespace WebApiAuthors.Filters
{
    public class ExceptionFilterAuthor : ExceptionFilterAttribute
    {
        private readonly ILogger<ExceptionFilterAuthor> logger;

        public ExceptionFilterAuthor(ILogger<ExceptionFilterAuthor> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);
            base.OnException(context);
        }
    }
}
