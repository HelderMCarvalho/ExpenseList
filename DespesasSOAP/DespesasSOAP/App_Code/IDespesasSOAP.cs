using System;
using System.ServiceModel;

[ServiceContract]
public interface IDespesasSOAP
{

    [OperationContract(Name = "AdicionarDespesa")]
    bool addDespesa(string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser);


    [OperationContract(Name = "EditarDespesa")]
    bool updateDespesa(int id, string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser);

}