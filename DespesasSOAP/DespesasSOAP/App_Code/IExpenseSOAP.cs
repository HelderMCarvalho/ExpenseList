using System;
using System.ServiceModel;

[ServiceContract]
public interface IExpenseSOAP
{
    /// <summary>
    ///     Add Expense
    /// </summary>
    /// <param name="nome">Expense Name</param>
    /// <param name="descricao">Expense Description</param>
    /// <param name="dataHoraCriacao">Expense DateTime of creation</param>
    /// <param name="valEuro">Expense EUR value</param>
    /// <param name="valUsd">Expense USD value</param>
    /// <param name="hashUser">Hash (ID) of the user that created the Expense</param>
    /// <returns>
    ///     <para>TRUE - Expense added successfully</para>
    ///     <para>FALSE - Expense not added successfully</para>
    /// </returns>
    [OperationContract(Name = "AddExpense")]
    bool AddExpense(string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser);

    /// <summary>
    ///     Update Expense
    /// </summary>
    /// <param name="id">ID of the Expense to be updated</param>
    /// <param name="nome">New Expense Name</param>
    /// <param name="descricao">New Expense Description</param>
    /// <param name="dataHoraCriacao">New Expense DateTime of creation</param>
    /// <param name="valEuro">New Expense EUR value</param>
    /// <param name="valUsd">New Expense USD value</param>
    /// <param name="hashUser">Hash (ID) of the user that own the Expense</param>
    /// <returns>
    ///     <para>TRUE - Expense updated successfully</para>
    ///     <para>FALSE - Expense not updated successfully</para>
    /// </returns>
    [OperationContract(Name = "UpdateExpense")]
    bool UpdateExpense(string id, string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser);

    /// <summary>
    ///     Add User (Register account)
    /// </summary>
    /// <param name="emailSha">Hashed email of the User</param>
    /// <param name="moedaPadrao">Default currency of the User (EUR, USD)</param>
    /// <returns>
    ///     <para>TRUE - User added successfully</para>
    ///     <para>FALSE - User not added successfully</para>
    /// </returns>
    [OperationContract(Name = "AddUser")]
    bool AddUser(string emailSha, string moedaPadrao);

    /// <summary>
    ///     Update User
    /// </summary>
    /// <param name="id">ID of the User to be updated</param>
    /// <param name="emailSha">New hashed email of the User</param>
    /// <param name="moedaPadrao">New default currency of the User (EUR, USD)</param>
    /// <returns>
    ///     <para>TRUE - User updated successfully</para>
    ///     <para>FALSE - User not updated successfully</para>
    /// </returns>
    [OperationContract(Name = "UpdateUser")]
    bool UpdateUser(string emailSha, string moedaPadrao);

}