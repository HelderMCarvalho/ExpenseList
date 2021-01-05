using DespesasLibrary;
using System;

public class ExpenseSOAP : IExpenseSOAP
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
    bool IExpenseSOAP.AddExpense(string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser) {
        int op = 1; // Insert
        DbConnect db = new DbConnect();
        return db.RunOperation(op, new Expense("", nome, descricao, dataHoraCriacao, valEuro, valUsd, hashUser));
    }

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
    bool IExpenseSOAP.UpdateExpense(string id, string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser) {
        int op = 2; // Update
        DbConnect db = new DbConnect();
        return db.RunOperation(op, new Expense(id, nome, descricao, dataHoraCriacao, valEuro, valUsd, hashUser), id);
    }

    /// <summary>
    ///     Add User (Register account)
    /// </summary>
    /// <param name="emailSha">Hashed email of the User</param>
    /// <param name="moedaPadrao">Default currency of the User (EUR, USD)</param>
    /// <returns>
    ///     <para>TRUE - User added successfully</para>
    ///     <para>FALSE - User not added successfully</para>
    /// </returns>
    bool IExpenseSOAP.AddUser(string emailSha, string moedaPadrao) {                                        // TODO: COLOCAR NUM FICHEIRO SÓ DE USER
        int op = 1; // Insert
        DbConnect db = new DbConnect();
        return db.RunOperation(op, new User(emailSha, moedaPadrao));
    }

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
    bool IExpenseSOAP.UpdateUser(string emailSha, string moedaPadrao) {                          // TODO: COLOCAR NUM FICHEIRO SÓ DE USER
        int op = 2; // Update
        DbConnect db = new DbConnect();
        return db.RunOperation(op, new User(emailSha, moedaPadrao));
    }
}