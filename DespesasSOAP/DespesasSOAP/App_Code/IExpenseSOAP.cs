using System;
using System.ServiceModel;

[ServiceContract]
public interface IExpenseSOAP
{

    [OperationContract(Name = "AddExpense")]
    bool addExpense(string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser);

    [OperationContract(Name = "UpdateExpense")]
    bool updateExpense(string id, string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser);

    [OperationContract(Name = "AddUser")]
    bool addUser(string emailSha, string moedaPadrao);

    [OperationContract(Name = "UpdateUser")]
    bool updateUser(string id, string emailSha, string moedaPadrao);
}