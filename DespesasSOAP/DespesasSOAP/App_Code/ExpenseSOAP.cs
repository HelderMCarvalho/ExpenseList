using System;

public class ExpenseSOAP : IExpenseSOAP
{
    bool IExpenseSOAP.addExpense(string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser) {
        int op = 1; // Insert
        DbConnect db = new DbConnect();
        return db.runOperation(op, new Expense(nome, descricao, dataHoraCriacao, valEuro, valUsd, hashUser));
    }
    bool IExpenseSOAP.updateExpense(int id, string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser) {
        int op = 2; // Update
        DbConnect db = new DbConnect();
        return db.runOperation(op, new Expense(nome, descricao, dataHoraCriacao, valEuro, valUsd, hashUser), id);
    }
}
