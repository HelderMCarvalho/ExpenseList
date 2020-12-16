using System;

/// <summary>
/// Summary description for Despesa
/// </summary>
public class Despesa
{
    public string nome { get; set; }
    public string descricao { get; set; }
    public DateTime dataHoraCriacao { get; set; }
    public decimal valEuro { get; set; }
    public decimal valUsd { get; set; }

    public string hashUser { get; set; }

    public Despesa() { }
    public Despesa(string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser) {
        this.nome = nome;
        this.descricao = descricao;
        this.dataHoraCriacao = dataHoraCriacao;
        this.valEuro = valEuro;
        this.valUsd = valUsd;
        this.hashUser = hashUser;
    }
}