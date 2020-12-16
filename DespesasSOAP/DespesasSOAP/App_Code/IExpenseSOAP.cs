using System;
using System.ServiceModel;

[ServiceContract]
public interface IExpenseSOAP
{

    [OperationContract(Name = "AddExpense")]
    bool addExpense(string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser);


    [OperationContract(Name = "UpdateExpense")]
    bool updateExpense(int id, string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser);

}