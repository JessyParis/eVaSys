/// <Propriété>
/// -----------------------------------------------------------------------------------------------------
/// Société Enviromatic sarl (Copyright)
/// 11 rue du Hainaut
/// 78570 Andrésy
/// -----------------------------------------------------------------------------------------------------
/// Projet : e-Valorplast
/// Création : 27/07/2018
/// ----------------------------------------------------------------------------------------------------- 

using eVaSys.Data;
using eVaSys.Utils;
using Microsoft.AspNetCore.Http.Features;

public class DynamicRequestSizeLimitMiddleware
{
    private readonly RequestDelegate _next;

    public DynamicRequestSizeLimitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        var maxRequestBodySizeFeature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();

        if (maxRequestBodySizeFeature != null)
        {
            // Set limit based on the request path by fetching values from the database.
            // Replace '8' and '9' with the actual RefParametre IDs for your limits.
            if (context.Request.Path.StartsWithSegments("/evapi/upload/DocumentEntite")
               || context.Request.Path.StartsWithSegments("/evapi/upload/ActionFichier")
               || context.Request.Path.StartsWithSegments("/evapi/upload/Transport")
               || context.Request.Path.StartsWithSegments("/evapi/upload/CommandeFournisseur")
               || context.Request.Path.StartsWithSegments("/evapi/upload/Email")
               || context.Request.Path.StartsWithSegments("/evapi/upload/NonConformite")
               || context.Request.Path.StartsWithSegments("/evapi/upload/Document")
               || context.Request.Path.StartsWithSegments("/evapi/upload/EquivalentCO2")
               || context.Request.Path.StartsWithSegments("/evapi/upload/parametre")
               )
            {
                // ID for DocumentEntite limit
                var limitParam = Utils.GetParametre(21, dbContext);
                if (limitParam.ValeurNumerique.HasValue)
                {
                    maxRequestBodySizeFeature.MaxRequestBodySize = ((long)limitParam.ValeurNumerique.Value * 1048576);
                }
            }
            else
            {
                //Default value 10Mo
                maxRequestBodySizeFeature.MaxRequestBodySize = 1048576;
            }
        }

        await _next(context);
    }
}