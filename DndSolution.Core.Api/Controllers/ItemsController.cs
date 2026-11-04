using DndSolution.Application.Models.Models.Items;
using DndSolution.Application.Services.Catalog;
using Microsoft.AspNetCore.Mvc;

namespace Core.Api.Controllers;

/// <summary>
/// Справочник предметов (SRD 5.1)
/// </summary>
[ApiController]
[Route("/items")]
public class ItemsController : ControllerBase
{
    /// <summary>
    /// Все предметы справочника. Вес — в фунтах, цена — в медных монетах за bundle штук
    /// </summary>
    [HttpGet]
    [ResponseCache(Duration = 3600)]
    [ProducesResponseType<List<ItemDefinition>>(StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(ItemCatalog.All.Values.ToList());
}
