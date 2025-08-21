namespace API.Lanchonete.Helpers
{
    public class SlugifyParameterTransformer : IOutboundParameterTransformer
    {
        public string? TransformOutbound(object? value) => value?.ToString()?.ToLowerInvariant();
    }
}
