using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using DespesasLibrary;
using DespesasREST.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DespesasWPF
{
    public class Api
    {
        private HttpWebRequest _request;
        private string _url;
        private readonly string _hashUser;
        private string _token;

        /// <summary>
        ///     Construtor
        /// </summary>
        /// <param name="hashUser"></param>
        public Api(string hashUser)
        {
            _hashUser = hashUser;
            _token = getToken();
        }

        /// <summary>
        ///     Faz reset ao Url para este poder ser usado no pedido
        /// </summary>
        private void Reset()
        {
            const bool isLocal = true;
            // ReSharper disable once UnreachableCode
            _url = isLocal ? "https://localhost:44325" : "https://despesasrest.azurewebsites.net/";
        }

        /// <summary>
        ///     Pede a lista de todas as Despesas do Utilizador
        /// </summary>
        /// <returns>Lista de Despesas</returns>
        public List<Expense> GetExpenses()
        {
            Reset();
            _url += "/Expense/{hashUser}/GetAll";
            _url = _url.Replace("{hashUser}", _hashUser);
            return JsonConvert.DeserializeObject<List<Expense>>(_get());
        }

        /// <summary>
        ///     Faz o pedido de eliminação de uma Despesa
        /// </summary>
        /// <param name="id">Id da Despesa a eliminar</param>
        /// <returns>
        ///     <para>TRUE: Despesa eliminada com sucesso</para>
        ///     <para>FALSE: Despesa não eliminada com sucesso</para>
        /// </returns>
        public bool DeleteExpense(int id)
        {
            Reset();
            _url += "/Expense/{hashUser}/Delete/{id}";
            _url = _url.Replace("{hashUser}", _hashUser);
            _url = _url.Replace("{id}", id.ToString());
            return _delete();
        }

        /// <summary>
        ///     Faz o pedido de verificação de existência de um Utilizador
        /// </summary>
        /// <returns>
        ///     <para>TRUE: Utilizador existe</para>
        ///     <para>FALSE: Utilizador não existe</para>
        /// </returns>
        public bool HasUser()
        {
            Reset();
            _url += "/Expense/{hashUser}/HasUser";
            _url = _url.Replace("{hashUser}", _hashUser);
            return JsonConvert.DeserializeObject<bool>(_get());
        }

        /// <summary>
        ///     Realiza pedidos GET e devolve o JSON que recebe
        /// </summary>
        /// <returns>JSON recebido</returns>
        /// <exception cref="ApplicationException">Erro ao obter dados</exception>
        private string _get()
        {
            // Create request with Updated Url
            _request = WebRequest.Create(_url) as HttpWebRequest;
            _request.Headers.Add("Authorization", "Bearer " + _token);
            try
            {
                using (HttpWebResponse response = _request?.GetResponse() as HttpWebResponse)
                {
                    // Se o StatusCode for positivo
                    if (response?.StatusCode == HttpStatusCode.OK)
                        return new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException())
                            .ReadToEnd();

                    if (response != null)
                    {
                        string message = $"HTTP ERROR CODE: {response.StatusCode}";
                        throw new ApplicationException(message);
                    }
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }

        /// <summary>
        ///     Realiza pedidos DELETE e devolve TRUE ou FALSE mediate o resultado
        /// </summary>
        /// <returns>
        ///     <para>TRUE: Dados apagados com sucesso</para>
        ///     <para>FALSE: Dados não apagados com sucesso</para>
        /// </returns>
        /// <exception cref="ApplicationException">Erro no pedido</exception>
        private bool _delete()
        {
            // Create request with Updated Url
            _request = WebRequest.Create(_url) as HttpWebRequest;
            _request.Headers.Add("Authorization", "Bearer " + _token);

            // Se não existir request devolve falso
            if (_request == null) return false;

            _request.Method = "DELETE";
            try
            {
                using (HttpWebResponse response = _request?.GetResponse() as HttpWebResponse)
                {
                    // Se o status code for positivo devolve true
                    if (response?.StatusCode == HttpStatusCode.OK) return true;

                    if (response != null)
                    {
                        string message = $"HTTP ERROR CODE: {response.StatusCode}";
                        throw new ApplicationException(message);
                    }
                }
            }
            catch (WebException e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }

        /// <summary>
        ///     Realiza o pedido a uma API externa e devolve o valor atual do dolar equivalente a um euro 
        /// </summary>
        /// <returns>Valor do Dolar relativo a 1 Euro</returns>
        public decimal GetUsdRatesToEuro()
        {
            _url = "https://api.exchangeratesapi.io/latest?base=EUR";
            _request = WebRequest.Create(_url) as HttpWebRequest;
            return JObject.Parse(_get()).SelectToken("rates").Value<decimal>("USD");
        }

        /// <summary>
        ///     Realiza o pedido a uma API externa e devolve o valor atual do euro equivalente a um dolar 
        /// </summary>
        /// <returns>Valor do Euro relativo a 1 Dolar</returns>
        public decimal GetEuroRatesToUsd()
        {
            _url = "https://api.exchangeratesapi.io/latest?base=USD";
            _request = WebRequest.Create(_url) as HttpWebRequest;
            return JObject.Parse(_get()).SelectToken("rates").Value<decimal>("EUR");
        }

        /// <summary>
        ///     Realiza o pedido para obter o último Id associado a um Utilizador de uma tabela
        /// </summary>
        /// <param name="nomeTabela">Nome da tabela a obter o último Id</param>
        /// <returns>Último Id</returns>
        public int GetLastIdFromTable(string nomeTabela)
        {
            Reset();
            _url += "/Expense/{hashUser}/GetLastId/{nomeTabela}";
            _url = _url.Replace("{hashUser}", _hashUser);
            _url = _url.Replace("{nomeTabela}", nomeTabela);

            // Create request with Updated Url
            _request = WebRequest.Create(_url) as HttpWebRequest;

            return JsonConvert.DeserializeObject<int>(_get());
        }


        private string getToken()
        {
            Reset();

            AuthenticateResponse res = null;

            var json = JsonConvert.SerializeObject(new AuthenticateRequest(_hashUser), Formatting.Indented);
            _url += "/Security/login";
            _request = WebRequest.Create(_url) as HttpWebRequest;
            _request.Method = "POST";
            _request.ContentType = "application/json";
            _request.ContentLength = json.Length;

            // Preencher o corpo do pedido
            using (var dataStream = _request.GetRequestStream())
            {
                dataStream.Write(Encoding.UTF8.GetBytes(json), 0, json.Length);
            }

            using (HttpWebResponse response = _request?.GetResponse() as HttpWebResponse)
            {
                // Se o status code for positivo devolve true
                if (response?.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<AuthenticateResponse>(
                        new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException())
                            .ReadToEnd()).Token;
                }

                if (response != null)
                {
                    string message = $"HTTP ERROR CODE: {response.StatusCode}";
                    throw new ApplicationException(message);
                }
            }

            return res.Token;
        }
    }
}