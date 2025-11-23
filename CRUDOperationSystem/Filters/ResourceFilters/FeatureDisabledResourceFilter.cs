using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUDOperationSystem.Filters.ResourceFilter
{
    public class FeatureDisabledResourceFilter : IAsyncResourceFilter
    {
        private readonly ILogger<FeatureDisabledResourceFilter> _logger;
        private readonly bool _isDisabled;
        public FeatureDisabledResourceFilter(ILogger<FeatureDisabledResourceFilter> logger, bool isDisabled)
        {
            _logger = logger;
            _isDisabled = isDisabled;
        }
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            _logger.LogInformation("{FilterName}.{MethodName}-before", nameof(FeatureDisabledResourceFilter), nameof(OnResourceExecutionAsync));
            if (_isDisabled)
            {
                _logger.LogWarning("Feature is disabled. Short-circuiting the request.");
                context.Result = new Microsoft.AspNetCore.Mvc.StatusCodeResult(StatusCodes.Status503ServiceUnavailable);
                return;
            }
            else
            {
                await next();
            }

            _logger.LogInformation("{FilterName}.{MethodName}-after", nameof(FeatureDisabledResourceFilter), nameof(OnResourceExecutionAsync));
        }
    }
}
