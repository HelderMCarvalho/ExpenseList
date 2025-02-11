//------------------------------------------------------------------------------
// <auto-generated>
//     Este código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.42000
//
//     As alterações a este ficheiro poderão provocar um comportamento incorrecto e perder-se-ão se
//     o código for regenerado.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DespesasWPF.ExpenseSOAP
{
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="DespesasWPF.ExpenseSOAP.IExpenseSOAP")]
    public interface IExpenseSOAP
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IExpenseSOAP/AddExpense", ReplyAction="http://tempuri.org/IExpenseSOAP/AddExpenseResponse")]
        bool AddExpense(string nome, string descricao, System.DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IExpenseSOAP/AddExpense", ReplyAction="http://tempuri.org/IExpenseSOAP/AddExpenseResponse")]
        System.Threading.Tasks.Task<bool> AddExpenseAsync(string nome, string descricao, System.DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IExpenseSOAP/UpdateExpense", ReplyAction="http://tempuri.org/IExpenseSOAP/UpdateExpenseResponse")]
        bool UpdateExpense(string id, string nome, string descricao, System.DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IExpenseSOAP/UpdateExpense", ReplyAction="http://tempuri.org/IExpenseSOAP/UpdateExpenseResponse")]
        System.Threading.Tasks.Task<bool> UpdateExpenseAsync(string id, string nome, string descricao, System.DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IExpenseSOAP/AddUser", ReplyAction="http://tempuri.org/IExpenseSOAP/AddUserResponse")]
        bool AddUser(string emailSha, string moedaPadrao);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IExpenseSOAP/AddUser", ReplyAction="http://tempuri.org/IExpenseSOAP/AddUserResponse")]
        System.Threading.Tasks.Task<bool> AddUserAsync(string emailSha, string moedaPadrao);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IExpenseSOAP/UpdateUser", ReplyAction="http://tempuri.org/IExpenseSOAP/UpdateUserResponse")]
        bool UpdateUser(string emailSha, string moedaPadrao);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IExpenseSOAP/UpdateUser", ReplyAction="http://tempuri.org/IExpenseSOAP/UpdateUserResponse")]
        System.Threading.Tasks.Task<bool> UpdateUserAsync(string emailSha, string moedaPadrao);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IExpenseSOAPChannel : DespesasWPF.ExpenseSOAP.IExpenseSOAP, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ExpenseSOAPClient : System.ServiceModel.ClientBase<DespesasWPF.ExpenseSOAP.IExpenseSOAP>, DespesasWPF.ExpenseSOAP.IExpenseSOAP
    {
        
        public ExpenseSOAPClient()
        {
        }
        
        public ExpenseSOAPClient(string endpointConfigurationName) : 
                base(endpointConfigurationName)
        {
        }
        
        public ExpenseSOAPClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public ExpenseSOAPClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress)
        {
        }
        
        public ExpenseSOAPClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public bool AddExpense(string nome, string descricao, System.DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser)
        {
            return base.Channel.AddExpense(nome, descricao, dataHoraCriacao, valEuro, valUsd, hashUser);
        }
        
        public System.Threading.Tasks.Task<bool> AddExpenseAsync(string nome, string descricao, System.DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser)
        {
            return base.Channel.AddExpenseAsync(nome, descricao, dataHoraCriacao, valEuro, valUsd, hashUser);
        }
        
        public bool UpdateExpense(string id, string nome, string descricao, System.DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser)
        {
            return base.Channel.UpdateExpense(id, nome, descricao, dataHoraCriacao, valEuro, valUsd, hashUser);
        }
        
        public System.Threading.Tasks.Task<bool> UpdateExpenseAsync(string id, string nome, string descricao, System.DateTime dataHoraCriacao, decimal valEuro, decimal valUsd, string hashUser)
        {
            return base.Channel.UpdateExpenseAsync(id, nome, descricao, dataHoraCriacao, valEuro, valUsd, hashUser);
        }
        
        public bool AddUser(string emailSha, string moedaPadrao)
        {
            return base.Channel.AddUser(emailSha, moedaPadrao);
        }
        
        public System.Threading.Tasks.Task<bool> AddUserAsync(string emailSha, string moedaPadrao)
        {
            return base.Channel.AddUserAsync(emailSha, moedaPadrao);
        }
        
        public bool UpdateUser(string emailSha, string moedaPadrao)
        {
            return base.Channel.UpdateUser(emailSha, moedaPadrao);
        }
        
        public System.Threading.Tasks.Task<bool> UpdateUserAsync(string emailSha, string moedaPadrao)
        {
            return base.Channel.UpdateUserAsync(emailSha, moedaPadrao);
        }
    }
}
