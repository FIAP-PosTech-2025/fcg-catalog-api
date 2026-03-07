namespace Catalog.Application.DTOs;

public class BibliotecaJogoDto : EntidadeBaseDto
{
    public string Descricao { get; set; } = default!;
    public string Genero { get; set; } = default!;
    public decimal Preco { get; set; }
    public DateTime DataCadastro { get; set; }
    public int Status { get; set; }
    public string StatusDescricao { get; set; } = default!;
    public Guid? PayId { get; set; }
}
