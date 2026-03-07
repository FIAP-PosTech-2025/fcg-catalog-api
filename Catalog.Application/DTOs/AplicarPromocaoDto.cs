using System.ComponentModel.DataAnnotations;

namespace Catalog.Application.DTOs;

public class AplicarPromocaoDto
{
    [Range(0.01, double.MaxValue)]
    public decimal NovoPreco { get; set; }
}