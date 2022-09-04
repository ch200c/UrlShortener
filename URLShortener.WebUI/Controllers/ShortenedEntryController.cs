using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using URLShortener.Application;
using URLShortener.Application.Interfaces;
using URLShortener.Domain;

namespace URLShortener.WebUI.Controllers
{
    // TODO versioning, validation
    [ApiController]
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces(MediaTypeNames.Application.Json)]
    public class ShortenedEntryController : ControllerBase
    {
        private readonly ILogger<ShortenedEntryController> _logger;
        private readonly IShortenedEntryRepository _shortenedEntryRepository;
        private readonly IShortenedEntryCreationService _shortenedEntryCreationService;

        public ShortenedEntryController(
            ILogger<ShortenedEntryController> logger,
            IShortenedEntryRepository shortenedEntryRepository,
            IShortenedEntryCreationService shortenedEntryCreationService)
        {
            _logger = logger;
            _shortenedEntryRepository = shortenedEntryRepository;
            _shortenedEntryCreationService = shortenedEntryCreationService;
        }

        [HttpGet("{alias}")]
        [ProducesResponseType(StatusCodes.Status302Found, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetShortenedEntry([FromRoute] string alias)
        {
            // TODO: Application service + cache
            var optionalEntry = await _shortenedEntryRepository.GetAsync(alias);

            return optionalEntry.Match<IActionResult>(
                entry => Redirect(entry.Url),
                () => NotFound());
        }

        [HttpPut("api/v1/shortenedEntry")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateShortenedEntryResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateShortenedEntry([FromBody] CreateShortenedEntryRequest request)
        {
            var optionalEntry = await _shortenedEntryCreationService.CreateAsync(request);

            return optionalEntry.Match<IActionResult>(
                entry =>
                {
                    var response = MapCreateShortenedEntryResponse(entry);
                    return CreatedAtAction(nameof(GetShortenedEntry), new { alias = response.Alias }, response);
                },
                () =>
                {
                    return BadRequest("Alias already exists");
                });
        }

        private static CreateShortenedEntryResponse MapCreateShortenedEntryResponse(ShortenedEntry entry)
        {
            return new CreateShortenedEntryResponse(entry.Alias, entry.Url, entry.UserId, entry.Creation, entry.Expiration);
        }
    }
}