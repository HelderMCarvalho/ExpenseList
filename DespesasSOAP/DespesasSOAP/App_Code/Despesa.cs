using System;

/// <summary>
/// Summary description for Despesa
/// </summary>
public class Despesa
{
    private string nome { get; set; }
    private string descricao { get; set; }
    private DateTime dataHoraCriacao { get; set; }
    private decimal valEuro { get; set; }
    private decimal valUsd { get; set; }

    public Despesa() { }
    public Despesa(string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd) {
        this.nome = nome;
        this.descricao = descricao;
        this.dataHoraCriacao = dataHoraCriacao;
        this.valEuro = valEuro;
        this.valUsd = valUsd;
    }
}