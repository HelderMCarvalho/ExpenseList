using System;

public class DespesasSOAP : IDespesasSOAP
{

    bool IDespesasSOAP.addDespesa(string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser) {
        int op = 1; // Criar
        DbConnect db = new DbConnect();
        return db.executeOp(op, new Despesa(nome, descricao, dataHoraCriacao, valEuro, valUsd, hashUser));
    }
    bool IDespesasSOAP.updateDespesa(int id, string nome, string descricao, DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser) {
        int op = 2; // Updatar
        DbConnect db = new DbConnect();
        return db.executeOp(op, new Despesa(nome, descricao, dataHoraCriacao, valEuro, valUsd, hashUser), id);
    }
}
