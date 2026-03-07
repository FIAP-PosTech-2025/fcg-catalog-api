using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Catalog.API.Services.Interfaces;
using Catalog.Application.DTOs;
using Catalog.Application.Interfaces;

namespace Catalog.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/user/biblioteca")]
public class BibliotecaController : ControllerBase
{
    private readonly IBibliotecaJogoService _service;
    private readonly ICurrentUser _currentUser;

    public BibliotecaController(IBibliotecaJogoService service, ICurrentUser currentUser)
    {
        _service = service;
        _currentUser = currentUser;
    }

    /// <summary>
    /// Retorna a biblioteca de jogos do usuário autenticado.
    /// </summary>
    /// <param name="ct">Token para cancelamento da requisição.</param>
    [HttpGet("me")]
    public async Task<ActionResult<IReadOnlyList<BibliotecaJogoDto>>> MinhaBiblioteca(
        CancellationToken ct)
    {

        var jogos = await _service.GetJogosDoUsuarioAsync( _currentUser.UserId.Value , ct);
        return Ok(jogos);
    }
}